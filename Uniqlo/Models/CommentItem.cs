using System.ComponentModel.DataAnnotations;

namespace Uniqlo.Models;

public class CommentItem : BaseEntity
{
    [Required, MaxLength(32)]
    public string UserName { get; set; }
    [Required, MaxLength(1028)]
    public string Description { get; set; }
    DateTime CreatedDate { get; set; } = DateTime.Now;
    public string UserId { get; set; }
    public User User { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }
}
