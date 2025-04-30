using System;

namespace Ebook_Libary_project.Models
{
    public class CreditCardModel
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IsraeliID { get; set; }
        public string CreditCardNumber { get; set; }
        public string ValidDate { get; set; }
        public string CVC { get; set; }
    }
}
