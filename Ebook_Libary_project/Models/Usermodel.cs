using System;

namespace EbookLibraryProject.Models
{
    public static class Usermodel
    {
        // Static Properties
        public static int Id { get; set; }
        public static bool Admin { get; set; }
        public static string Name { get; set; }
        public static string Mail { get; set; }
        public static string Password { get; set; }
        public static int Age { get; set; }

        // Static Method to Initialize User Data
        public static void Initialize(int id, string name, string mail, string password, int age, bool admin)
        {
            Id = id;
            Name = name;
            Mail = mail;
            Password = password;
            Age = age;
            Admin = admin;
        }
    }
}
