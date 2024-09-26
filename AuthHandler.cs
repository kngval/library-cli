

using System.Security.Cryptography;
using System.Text;
using Npgsql;

public class AuthHandler
{

    private readonly string _connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION")!;

    public void Register(string username, string password)
    {
        string hashedPassword = HashPassword(password);
        using (var conn = new NpgsqlConnection(_connectionString))
        {
            conn.Open();

            string userExists = @"
        SELECT 
        FROM users
        WHERE users.username = @Username
        ";
            using (var userExistsCmd = new NpgsqlCommand(userExists, conn))
            {
                userExistsCmd.Parameters.AddWithValue("@Username", username);
                var ex = userExistsCmd.ExecuteScalar();
                if (ex == null)
                {

                    string query = @"INSERT INTO users (username,password) VALUES(@Username,@Password)";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", hashedPassword);
                        cmd.ExecuteNonQuery();

                        Console.WriteLine($"{username} is successfully registered.");
                    }
                } else {
                  Console.WriteLine("User already exists.");
                  return;
                }

            }

        }
    }

    public bool Login(string username,string password)
    {
      using(var conn = new NpgsqlConnection(_connectionString))
      {
        conn.Open();

        string userExistsQuery = @"
          SELECT password  
          FROM users 
          WHERE users.username = @Username
          ";
        using(var cmd = new NpgsqlCommand(userExistsQuery,conn))
        {
          cmd.Parameters.AddWithValue("@Username",username);

          var result = cmd.ExecuteScalar();
          if(result == null)
          {
            Console.WriteLine("User does not exist");
            return false;
          } else {
           string dbHashedPassword = result.ToString()!; 
           string hashedPassword = HashPassword(password);

           if(hashedPassword == dbHashedPassword)
           {
             Console.WriteLine("Login Successful");
             return true;
           } else {
             Console.WriteLine("Wrong password.");
             return false;
           }
          }
        }
      }
    }


    private string HashPassword(string password)
    {
      using(var hmac = new HMACSHA256())
      {
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hash);
      }
    }

    
}
