namespace EbookLibraryProject.Models
{
    public class Review
    {

        public int UserId { get; set; }
        public int BookId { get; set; }
        public string ReviewText { get; set; }
    }
}
