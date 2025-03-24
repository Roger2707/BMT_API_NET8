using Store_API.DTOs.Comments;

namespace Store_API.Repositories
{
    public interface ICommentRepository
    {
        public Task<List<CommentDTO>> GetAll(int productId);
        public Task<CommentDTO> GetById(int commentId);
        public Task Create(int userId, Guid productId, string content);
        public Task Update(int commentId, string content);
        public Task Delete(int commentId);
    }
}
