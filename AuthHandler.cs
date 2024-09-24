

using System.Security.Cryptography;
using System.Text;
using Npgsql;

public class AuthHandler
{

    private readonly string _connectionString = "Host=localhost;Username=kngval;Password=kngvalarch;Database=library";

    public void Register(string username, string password)
    {
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
                        cmd.Parameters.AddWithValue("@Password", password);
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

    public void Login(string username,string password)
    {
      
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
