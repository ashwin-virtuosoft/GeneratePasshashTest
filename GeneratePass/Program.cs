using System;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Data.SqlClient;

namespace PasswordGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Data Source=PC\\MSSQLSERVER01;Initial Catalog=TestHash;Integrated Security=True;";

            Console.WriteLine("Enter username:");
            string username = Console.ReadLine();

            Console.WriteLine("Enter password:");
            string password = Console.ReadLine();

            // Hash the password
            string hashedPassword = HashPassword(password);

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
                    Console.WriteLine("Password inserted successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error inserting password: " + ex.Message);
                }
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