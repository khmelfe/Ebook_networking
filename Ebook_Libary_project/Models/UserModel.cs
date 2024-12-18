using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;

namespace EbookLibraryProject.Models
{
    public class Usermodel
    {
        // Properties
        public int Id { get; set; }
        public bool admin { get; set; }
        public string Name { get; set; }
        public string Mail { get; set; }
        public string Password { get; set; }
        public int Age { get; set; }

        // Constructor
        public Usermodel(int id, string name,string mail, string password, int age)
        {
            Id = id;
            Name = name;
            Mail = mail;
            Password = password;
            Age = age;
        }

        // Methods
      

       
    }
}
