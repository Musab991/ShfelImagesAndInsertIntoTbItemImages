using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ImageAssigner
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Server = DARK-HORSE-ATIE; Database=LapShop ; Integrated Security= SSPI;  TrustServerCertificate = True;";

            List<string> imageUrls = new List<string>();
            List<int> itemIds = new List<int>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Step 1: Retrieve image URLs from TempImageUrls table
                //make sure 
                using (SqlCommand command = new SqlCommand("SELECT ImageUrl FROM TempImageUrls", connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        imageUrls.Add(reader["ImageUrl"].ToString());
                    }
                    reader.Close();
                }

                // Step 2: Retrieve item IDs from TbItems table
                using (SqlCommand command = new SqlCommand("SELECT ItemId FROM TbItems", connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        itemIds.Add(Convert.ToInt32(reader["ItemId"]));
                    }
                    reader.Close();
                }

                // Step 3: Randomly assign images to items
                Random random = new Random();

                foreach (int itemId in itemIds)
                {
                    int imageCount = random.Next(1, 5); // Random number between 1 and 4
                    HashSet<string> selectedImages = new HashSet<string>();

                    for (int i = 0; i < imageCount; i++)
                    {
                        string randomImage = imageUrls[random.Next(imageUrls.Count)];
                        if (!selectedImages.Contains(randomImage))
                        {
                            selectedImages.Add(randomImage);

                            // Step 4: Insert into TbItemImages table
                            using (SqlCommand command = new SqlCommand("INSERT INTO TbItemImages (ItemId, ImageName) VALUES (@ItemId, @ImageName)", connection))
                            {
                                command.Parameters.AddWithValue("@ItemId", itemId);
                                command.Parameters.AddWithValue("@ImageName", randomImage);
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }

            Console.WriteLine("Images assigned to items successfully.");
        }
    }
}
