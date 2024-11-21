using System.ComponentModel.DataAnnotations;

namespace Uniqlo.ViewModels.Sliders
{
    public class SliderCreateVM
    {
        [MaxLength(32, ErrorMessage = "The Title must not exceed 32 characters."), Required]
        public string Title { get; set; }
        [Required]
        public string Subtitle { get; set; }
        public string? Link { get; set; }
        [Required(ErrorMessage = "No file has been chosen.")]
        public IFormFile File { get; set; }
    }
}
