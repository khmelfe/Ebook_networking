using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ebook_Libary_project.Models { 

    public class DashboardViewModel
    {
        public List<int> Userids { get; set; }  
        

        public int UserCount { get; set; }
        public int BookAmount { get; set; }
    }
}
