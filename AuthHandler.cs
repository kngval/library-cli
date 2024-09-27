

using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;
using Npgsql;

public  static class AuthHandler
{

    private static readonly string _connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION")!;

    public static void Register(string username, string password)
    {
        string hashedPassword = HashPassword(password);
        using (var conn = new NpgsqlConnection(_connectionString))
        {
            conn.Open();

            string userExists = @"
        SELECT username  
        FROM users 
        WHERE username = @Username
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

    public static bool Login(string username,string password)
    {
      using(var conn = new NpgsqlConnection(_connectionString))
      {
        conn.Open();

        string userExistsQuery = @"
          SELECT password  
          FROM users 
          WHERE username = @Username
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

           if(BCrypt.Net.BCrypt.Verify(password, dbHashedPassword))
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


    private static string HashPassword(string password)
    {
     return BCrypt.Net.BCrypt.HashPassword(password); 
    }

    
}
