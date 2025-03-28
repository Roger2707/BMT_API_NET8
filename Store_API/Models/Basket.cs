using Store_API.Models.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store_API.Models
{
    public class Basket
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        public List<BasketItem> Items { get; set; }
    }
}
