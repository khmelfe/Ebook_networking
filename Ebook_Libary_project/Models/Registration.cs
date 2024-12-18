using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ebook_Libary_project.Models
{
    //Regstartion.
    public class RegistrationModel
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }

        // Validation logic can go here (optional)
        public bool IsPasswordMatch()
        {
            return Password == ConfirmPassword;
        }
    }
}