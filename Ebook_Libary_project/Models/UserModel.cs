using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ebook_Libary_project.Models
{
    public class UserModel
    {

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Username must be atleast 2-50 chars")]
        public string Username { get ; set; }

        [Required]
        [RegularExpression("^[0-9]{16}$")]
        public string Password { get; set; }

    }
}





 
