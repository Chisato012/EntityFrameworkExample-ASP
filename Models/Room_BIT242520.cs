using System.ComponentModel.DataAnnotations;

namespace Lesson3_CNLTWeb.Models
{
    public class Room_BIT242520
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100)]
        [Display(Name = "Room name")]
        public string Name { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        [Display(Name = "Price")]
        public decimal Price { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Area must be greater than 0")]
        [Display(Name = "Area")]
        public decimal Area { get; set; }

        [Display(Name = "Available")]
        public bool IsAvailable { get; set; } = true;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Room type is required")]
        [Display(Name = "Room type")]
        public int RoomTypeId { get; set; }

        public RoomType_BIT242520? RoomType { get; set; }

        public ICollection<RoomImage_BIT242520> RoomImages { get; set; } = new List<RoomImage_BIT242520>();
    }
}
