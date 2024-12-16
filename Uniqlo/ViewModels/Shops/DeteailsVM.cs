using Uniqlo.Models;

namespace Uniqlo.ViewModels.Shops
{
    public class DeteailsVM
    {
        public Product Product { get; set; }
        public IEnumerable<CommentItem> Comments { get; set; }
    }
}
