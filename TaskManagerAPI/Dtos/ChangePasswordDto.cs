using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.Dtos
{
    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; } = string.Empty;
        [Required]
        [MaxLength(200)]
        public string NewPassword { get; set; } = string.Empty;
    }
}
