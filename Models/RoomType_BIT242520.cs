using System.ComponentModel.DataAnnotations;

namespace Lesson3_CNLTWeb.Models
{
    public class RoomType_BIT242520
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100)]
        [Display(Name = "Room type name")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public ICollection<Room_BIT242520> Rooms { get; set; } = new List<Room_BIT242520>();
    }
}
