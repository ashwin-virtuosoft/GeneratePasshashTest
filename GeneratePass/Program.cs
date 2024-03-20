using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace PasswordGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Data Source=PC\\MSSQLSERVER01;Initial Catalog=TestHash;Integrated Security=True;";

            Console.WriteLine("Register - Enter username:");
            string username = Console.ReadLine();

            Console.WriteLine("Register - Enter password:");
            string password = Console.ReadLine();

            // Hash the password before storing it
            string hashedPassword = HashPassword(password);

            // Register the user
            bool registrationSuccessful = RegisterUser(connectionString, username, hashedPassword);

            if (registrationSuccessful)
            {
                Console.WriteLine("Registration successful!");

                Console.WriteLine("\nLogin - Enter username:");
                string loginUsername = Console.ReadLine();

                Console.WriteLine("Login - Enter password:");
                string loginPassword = Console.ReadLine();

                // Verify login credentials
                bool loginSuccessful = VerifyLogin(connectionString, loginUsername, loginPassword);

                if (loginSuccessful)
                {
                    Console.WriteLine("Login successful!");
                }
                else
                {
                    Console.WriteLine("Invalid username or password.");
                }
            }
            else
            {
                Console.WriteLine("Registration failed. Username already exists.");
            }
        }

        static bool RegisterUser(string connectionString, string username, string hashedPassword)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string insertQuery = "INSERT INTO TestHash (Name, Password) VALUES (@Name, @Password)";
                SqlCommand command = new SqlCommand(insertQuery, connection);
                command.Parameters.AddWithValue("@Name", username);
                command.Parameters.AddWithValue("@Password", hashedPassword);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error registering user: " + ex.Message);
                    return false;
                }
            }
        }

        static bool VerifyLogin(string connectionString, string username, string password)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string selectQuery = "SELECT Password FROM TestHash WHERE Name = @Name";
                SqlCommand command = new SqlCommand(selectQuery, connection);
                command.Parameters.AddWithValue("@Name", username);

                connection.Open();
                string hashedPassword = (string)command.ExecuteScalar();

                // Compare the hashed password retrieved from the database with the hashed password of the login input
                return hashedPassword != null && hashedPassword.Equals(HashPassword(password));
            }
        }

        static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashedBytes.Length; i++)
                {
                    builder.Append(hashedBytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }
}
