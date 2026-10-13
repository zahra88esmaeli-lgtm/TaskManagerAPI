using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.Dtos
{
    public class CreateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        [MaxLength(1000)]
        public string? Description { get; set; }
        public bool IsDone { get; set; } = false;
    }
}
