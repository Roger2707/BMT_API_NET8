using System.ComponentModel.DataAnnotations.Schema;

namespace Store_API.Models
{
    public class Reply
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public bool Status { get; set; } = true;
        public int UserId { get; set; }
        public int CommentId { get; set; }
        [ForeignKey("CommentId")]
        public Comment Comment { get; set; }
    }
}
