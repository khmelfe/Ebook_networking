using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

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

        public static bool ValitdateUser(string name, string password, string con_pass, string mail, string age,bool isadmin)
        {
            return checkname(name) && IsPasswordMatch(password, con_pass) && checkage(age) && Checkmail(mail);
        }

        private static bool Checkmail(string mail)
        {
            
                var emailAttribute = new EmailAddressAttribute();

                if (emailAttribute.IsValid(mail))
                {
                    return true;
                }
                else
                {
                    throw new Exception("Please check the Email again\n");
                }
            
            //will catch our excption
         
          
        }

        private static bool checkage(string age)
        {

            if (age != null && int.TryParse(age, out int parsedAge))
            {
                return true;

            }
            else
            {
                throw new Exception("Age must be a valid number.");

            }
        }
            

        public static bool checkname(string name)
        {
            if (!string.IsNullOrEmpty(name) && name.Length < 100)
            {
                return true;
            }
            return false;
        }
       
        public static bool IsPasswordMatch(string password,string con_pass)
        {
            string pattern = @"^(?=.*[A-Z])(?=.*[!@#$%^&*(),.?""{}|<>])[A-Za-z\d!@#$%^&*(),.?""{}|<>]{8,16}$";

            if (Regex.IsMatch(password, pattern))
            {
                if( password == con_pass)
                    return true;
                throw new Exception("passwords do not match. ");
            }
            throw new Exception("Password must be 8-16 characters long,\n contain at least one uppercase letter,\n" +
                " and one special character.");
        }
        
    }
}