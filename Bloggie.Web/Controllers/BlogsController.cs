using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Bloggie.Web.Models.Domain;

namespace Bloggie.Web.Controllers
{
    public class BlogsController : Controller
    {

        private readonly IBlogPostRepository _blogPostRepository;
        private readonly IBlogPostLikeRepository _blogPostLikeRepository;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IBlogPostCommentRepository _blogPostCommentRepository;

        public BlogsController(IBlogPostRepository blogPostRepository,
                               IBlogPostLikeRepository blogPostLikeRepository,
                               SignInManager<IdentityUser> signInManager,
                               UserManager<IdentityUser> userManager,
                               IBlogPostCommentRepository blogPostCommentRepository)
        {
            _blogPostRepository = blogPostRepository;
            _blogPostLikeRepository = blogPostLikeRepository;
            _signInManager = signInManager;
            _userManager = userManager;
            _blogPostCommentRepository = blogPostCommentRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string urlHandle)
        {
            var liked = false;
            var blogpost = await _blogPostRepository.GetByUrlHandleAsync(urlHandle);

            if(blogpost != null)
            {
                //Repository apna kaam krrhi hai
                var totallikes = await _blogPostLikeRepository.GetTotalLikes(blogpost.Id);

                if (_signInManager.IsSignedIn(User))
                {
                    // Get Like for this blog for this user
                    // Saray likes utahlo phir us like ko filter kro jo is user ne kra ho
                    var likesForBlog = await _blogPostLikeRepository.GetLikesForBlog(blogpost.Id);

                    var userId = _userManager.GetUserId(User);

                    if(userId != null)
                    {
                       //Filter for the like of the currently signed in user
                       var likedfromthesignedinuser = likesForBlog.FirstOrDefault(x => x.UserId == Guid.Parse(userId));
                       liked = likedfromthesignedinuser != null;
                    }
                }

                //I want all the comments of the currently opened blogpost
                var allCommentsForDomain = await _blogPostCommentRepository.GetTotalCommentsAsync(blogpost.Id);

                //Ye List maine sirf saray comments dikhane ke liye banae hai direct domain model use nhi kra maine
                var blogCommentForView = new List<BlogComment>();

                foreach (var comment in allCommentsForDomain) 
                {
                    blogCommentForView.Add(new BlogComment()
                    {
                        DateAdded = comment.DateAdded,
                        Description = comment.Description,
                        UserName = (await _userManager.FindByIdAsync(comment.UserId.ToString())).UserName
                    });

                }

                //Mapping of blogpost coming from above into a View Model
                var blogDetails = new BlogDetailsViewModel()
                {
                    Id = blogpost.Id,
                    Heading = blogpost.Heading,
                    PageTitle = blogpost.PageTitle,
                    Content = blogpost.Content,
                    ShortDescription = blogpost.ShortDescription,
                    FeaturedImageUrl = blogpost.FeaturedImageUrl,
                    UrlHandle = blogpost.UrlHandle,
                    PublishedDate = blogpost.PublishedDate,
                    Author = blogpost.Author,
                    Visible = blogpost.Visible,
                    Tags = blogpost.Tags,
                    TotalLikes = totallikes,
                    Liked = liked,
                    Comments = blogCommentForView
                };
                return View(blogDetails);        
            }

            return RedirectToAction("Index","Home");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(BlogDetailsViewModel blogDetailsViewModel)
        {
            
            if (_signInManager.IsSignedIn(User))
            {
                var blogPostComment = new BlogPostcomment()
                {
                    Description = blogDetailsViewModel.CommentDescription,
                    BlogPostId = blogDetailsViewModel.Id,
                    UserId = Guid.Parse(_userManager.GetUserId(User)),
                    DateAdded = DateTime.Now
                };
                await _blogPostCommentRepository.AddAsync(blogPostComment);
                return RedirectToAction("Index", "Home", new {urlHandle = blogDetailsViewModel.UrlHandle});
                
            }
            return View();           
        }
    }
}
