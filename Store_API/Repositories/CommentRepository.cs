using Microsoft.EntityFrameworkCore;
using Store_API.Data;
using Store_API.DTOs.Comments;
using Store_API.Models;

namespace Store_API.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly StoreContext _db;
        private readonly IDapperService _dapperService;
        public CommentRepository(StoreContext db, IDapperService dapperService)
        {
            _db = db;
            _dapperService = dapperService;
        }
        public async Task Create(int userId, Guid productId, string content)
        {
            var comment = new Comment
            {
                UserId = userId,
                ProductId = productId,
                Content = content
            };

            await _db.AddAsync(comment);
            await _db.SaveChangesAsync();
        }

        public async Task<List<CommentDTO>> GetAll(int productId)
        {
            string query = @"
                            SELECT 
	                            com.Id
	                            , com.Content
	                            , ISNULL(SUBSTRING(CONVERT(varchar, com.Created), 1, 10), '') as Created
	                            , IIF(com.Status = 0, 'In-Active', 'Active') as Status
	                            , com.ProductId
	                            , product.Name as ProductName
	                            , com.UserId
	                            , u.UserName
                            FROM Comments com 

                            INNER JOIN Products product ON com.ProductId = product.Id
                            INNER JOIN AspNetUsers u ON u.Id = com.UserId

                            WHERE com.ProductId = @ProductId

                            ";

            var p = new { ProductId = productId };

            var result = await _dapperService.QueryAsync(query, p);
            if (result == null || result.Count < 0) return null;

            var comments = new List<CommentDTO>();

            foreach (var r in result)
            {
                var comment = new CommentDTO
                {
                    Id = r.Id,
                    Content = r.Content,
                    Created = r.Created,
                    Status = r.Status,
                    ProductId = r.ProductId,
                    ProductName = r.ProductName,
                    UserId = r.UserId,
                    UserName = r.UserName,
                };
                comments.Add(comment);
            }

            return comments;
        }

        public async Task<CommentDTO> GetById(int commentId)
        {
            string query = @"
                            SELECT 
	                            com.Id
	                            , com.Content
	                            , ISNULL(SUBSTRING(CONVERT(varchar, com.Created), 1, 10), '') as Created
	                            , IIF(com.Status = 0, 'In-Active', 'Active') as Status
	                            , com.ProductId
	                            , product.Name as ProductName
	                            , com.UserId
	                            , u.UserName
                            FROM Comments com 

                            INNER JOIN Products product ON com.ProductId = product.Id
                            INNER JOIN AspNetUsers u ON u.Id = com.UserId

                            WHERE com.Id = @Id

                            ";
            var p = new { Id = commentId };
            var result = await _dapperService.QueryFirstOrDefaultAsync(query, p);
            if (result == null) return null;

            var comment = new CommentDTO
            {
                Id = result.Id,
                Content = result.Content,
                Created = result.Created,
                Status = result.Status,
                ProductId = result.ProductId,
                ProductName = result.ProductName,
                UserId = result.UserId,
                UserName = result.UserName,
            };

            return comment;
        }

        public async Task Update(int commentId, string content)
        {
            string query = " UPDATE Comments SET Content = @Content, Created = GETDATE() WHERE Id = @Id ";
            var p = new { Content = content, Id = commentId };
            try
            {
                await _dapperService.Execute(query, p);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task Delete(int commentId)
        {
            try
            {
                var comment = await _db.Comments.FirstOrDefaultAsync(x => x.Id == commentId);
                _db.Comments.Remove(comment);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
