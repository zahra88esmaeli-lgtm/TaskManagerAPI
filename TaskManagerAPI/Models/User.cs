using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;
        [Required]
        [MaxLength(200)]
        public string HashPassword { get; set; } = string.Empty;
        public List<TaskItem> TaskItems { get; set; } = new List<TaskItem>();

    }
   
}
