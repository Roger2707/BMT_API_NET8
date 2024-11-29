using Store_API.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store_API.DTOs.Comments
{
    public class CommentDTO
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string Created { get; set; }
        public string Status { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
    }
}
