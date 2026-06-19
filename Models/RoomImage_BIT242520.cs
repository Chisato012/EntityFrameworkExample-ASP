using System.ComponentModel.DataAnnotations;

namespace Lesson3_CNLTWeb.Models
{
    public class RoomImage_BIT242520
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Image URL is required")]
        [StringLength(1000)]
        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; } = string.Empty;

        [Display(Name = "Thumbnail")]
        public bool IsThumbnail { get; set; }

        public int RoomId { get; set; }

        public Room_BIT242520? Room { get; set; }
    }
}
