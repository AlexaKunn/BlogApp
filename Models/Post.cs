using System.ComponentModel.DataAnnotations.Schema;

namespace BlogApp.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Body { get; set; } = "";
        [NotMapped]
        public IFormFile Image { get; set; }

        public string ImagePath { get; set; }

        [NotMapped]
        public IFormFileCollection GalleryFiles { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;

        public List<Gallery> Gallery { get; set; } 

        
    }
}
