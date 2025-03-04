using CsvHelper.Configuration.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Store_API.Models;
using Store_API.Repositories;

namespace Store_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        public CommentsController(UserManager<User> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager; 
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(int productId, [FromForm] string content)
        {
            if (productId == 0 || productId.ToString() == "") return NotFound();
            int userId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;

            string error = "";
            try
            {
                await _unitOfWork.BeginTransactionDapperAsync();

                await _unitOfWork.Comment.Create(userId, productId, content);

                await _unitOfWork.CommitAsync();
            }
            catch(Exception ex)
            {
                error = ex.Message;
                await _unitOfWork.RollbackAsync();
            }
            if (error != "") return BadRequest(new ProblemDetails { Title = "Comment is not created !" });
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(int commentId, [FromForm] string content)
        {
            if (commentId == 0 || string.IsNullOrEmpty(commentId.ToString())) return NotFound();
            var currentComment = await _unitOfWork.Comment.GetById(commentId);
            if(currentComment == null) return NotFound();

            int userIdComment = currentComment.UserId;
            int userIdCurrent = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;

            if (userIdComment != userIdCurrent) 
                return BadRequest(new ProblemDetails { Title = $"You can not edit this comment, comment id : {commentId}"});

            string error = "";
            try
            {
                await _unitOfWork.BeginTransactionDapperAsync();
                await _unitOfWork.Comment.Update(commentId, content);
                await _unitOfWork.CommitAsync();
            }
            catch(Exception ex)
            {
                error = ex.Message;
                await _unitOfWork.RollbackAsync();
            }
            
            if(error != "") return BadRequest(new ProblemDetails { Title = error });
            return Ok();
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Delete(int commentId)
        {
            if (commentId == 0 || string.IsNullOrEmpty(commentId.ToString())) return NotFound();
            var currentComment = await _unitOfWork.Comment.GetById(commentId);
            if (currentComment == null) return NotFound();

            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            int userIdComment = currentComment.UserId;
            int userIdCurrent = currentUser.Id;
            string roleName = (await _userManager.GetRolesAsync(currentUser)).ToList()[0];

            // Only the customer who create this comment, admin and manager can delete 
            if (userIdComment != userIdCurrent || roleName != "Manager" || roleName != "Admin")
                return BadRequest(new ProblemDetails { Title = $"You can not edit this comment, comment id : {commentId}" });

            string error = "";
            try
            {
                await _unitOfWork.Comment.Delete(commentId);
            }
            catch(Exception ex)
            {
                error = ex.Message;
            }

            if (error != "") return BadRequest(new ProblemDetails { Title = error });
            return Ok();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetComments(int productId)
        {
            if(productId == 0 || string.IsNullOrEmpty(productId.ToString())) return NotFound();
            var comments = await _unitOfWork.Comment.GetAll(productId);       
            return Ok(comments);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetCommentById(int commentId)
        {
            if (commentId == 0 || string.IsNullOrEmpty(commentId.ToString())) return NotFound();
            var comment = await _unitOfWork.Comment.GetById(commentId);
            return Ok(comment);
        }
    }
}
