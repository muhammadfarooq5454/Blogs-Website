using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bloggie.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostLikeController : ControllerBase
    {
        //Using data from body that is FromBody

        private readonly IBlogPostLikeRepository _blogPostLikeRepository;

        public BlogPostLikeController(IBlogPostLikeRepository blogPostLikeRepository)
        {
            _blogPostLikeRepository = blogPostLikeRepository;
        }

        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> AddLike([FromBody] AddLikeRequest addLikeRequest)
        {
            //Register using a AddLike method by pressing a like button

            var blogPostLike = new BlogPostLike()
            {
                UserId = addLikeRequest.UserId,
                BlogPostId = addLikeRequest.BlogPostId
            };

            await _blogPostLikeRepository.AddLikeForBlog(blogPostLike);

            return Ok();
        }

        [HttpGet]
        [Route("{blogPostId:Guid}/totalLikes")]
        //Route se mai isko pass krrha api controller ke get method mai jiska naam hai gettotallikesforblog.
        public async Task<IActionResult> GetTotalLikesForBlog([FromRoute] Guid blogPostId)
        {
            var totallikes = await _blogPostLikeRepository.GetTotalLikes(blogPostId);
            
            return Ok(totallikes); //Returns HttpStatusCode 200 if it is successfully called
        }

    }
}