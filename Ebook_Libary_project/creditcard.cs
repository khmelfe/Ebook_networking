using System;
using System.Collections.Generic;
using Ebook_Library_Project; // Import your Userdatabase!

namespace Ebook_Library_Project
{
    public static class creditcard
    {
        public static void FillDatabase()
        {
            DeleteAllUsersAndCards();
            InsertUsersAndCards();
        }

        private static void DeleteAllUsersAndCards()
        {
            var allUserIds = Userdatabase.getallusersid();

            foreach (var userId in allUserIds)
            {
                Userdatabase.RemoveUser(userId); // Use your existing remove method!
            }

            using (var connection = new System.Data.SqlClient.SqlConnection(Userdatabase.connectionString))
            {
                connection.Open();
                // Also clean CreditCards and WebReviews separately if they are not removed inside RemoveUser
                string[] extraClean = {
                    "DELETE FROM CreditCards",
                    "DELETE FROM WebReviews"
                };

                foreach (var query in extraClean)
                {
                    using (var cmd = new System.Data.SqlClient.SqlCommand(query, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            Console.WriteLine("All users and related records deleted successfully.");
        }

        private static void InsertUsersAndCards()
        {
            var users = new List<(string Name, string Mail, string Password, int Age, bool Admin)>
            {
                ("AdminUser", "admin@example.com","Password123", 30, true),
                ("User1", "user1@example.com", "Password123", 25, false),
                ("User2", "user2@example.com", "Password123", 24, false),
                ("User3", "user3@example.com", "Password123", 22, false),
                ("User4", "user4@example.com", "Password123", 28, false),
                ("User5", "user5@example.com", "Password123", 31, false),
                ("User6", "user6@example.com", "Password123", 27, false),
                ("User7", "user7@example.com", "Password123", 23, false),
                ("User8", "user8@example.com", "Password123", 29, false),
                ("User9", "user9@example.com", "Password123", 26, false)
            };

            foreach (var user in users)
            {
                // Add user
                Userdatabase.AddUser(user.Name, user.Mail, user.Password, user.Age, user.Admin);

                // Get the new user's ID
                int newUserId = Userdatabase.Getuseridbyname(user.Name);

                // Create fake credit card
                string firstName = user.Name;
                string lastName = "LastName";
                string fakeID = (123456789 + newUserId).ToString();
                string fakeCardNumber = (4000000000000000 + newUserId).ToString();
                string validDate = "12/32";
                string cvc = (100 + newUserId).ToString();

                // Add credit card
                Userdatabase.AddCreditCard(newUserId, firstName, lastName, fakeID, fakeCardNumber, validDate, cvc);
            }

            Console.WriteLine("10 users and 10 credit cards inserted successfully.");
        }
    }
}
