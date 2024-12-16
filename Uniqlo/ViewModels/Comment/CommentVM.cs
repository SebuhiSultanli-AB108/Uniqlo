using System.ComponentModel.DataAnnotations;

namespace Uniqlo.ViewModels.Comment
{
    public class CommentVM
    {
        [MaxLength(1028)]
        public string Description { get; set; }
        public int ProductId { get; set; }
    }

}
