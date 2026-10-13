using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.Dtos
{
    public class ChangePasswordDto
    {
       
        [Required]
        [MaxLength(200)]
        public string NewPassword { get; set; } = string.Empty;
    }
}
