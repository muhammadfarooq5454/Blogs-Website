using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Repositories
{
    public class BlogPostLikeRepository : IBlogPostLikeRepository
    {
        private readonly BloggieDbContext _bloggieDbcontext;

        public BlogPostLikeRepository(BloggieDbContext bloggieDbContext)
        {
            _bloggieDbcontext = bloggieDbContext;
        }

        public async Task<BlogPostLike> AddLikeForBlog(BlogPostLike blogPostLike)
        {
            await _bloggieDbcontext.BlogPostLike.AddAsync(blogPostLike);
            
            await _bloggieDbcontext.SaveChangesAsync();

            return blogPostLike;
        }


        public async Task<int> GetTotalLikes(Guid blogPostId)
        {
            //agr mere database mai suppose kro ke 3 id hain ek hi blogpostid pe tu mai jo 
            //bhi blogpostid pass krrha woh bhi 3 hi hongi.

            // like :   blogpostid | id
            //           03040404  | 39393      
            //           03040404  | 30340      1 to many relation (1 to ∞)
            //           03040404  | 33456

            return await _bloggieDbcontext.BlogPostLike.CountAsync(x => x.BlogPostId == blogPostId);
        }
        public async Task<IEnumerable<BlogPostLike>> GetLikesForBlog(Guid blogPostId)
        {
            return await _bloggieDbcontext.BlogPostLike.Where(x => x.BlogPostId == blogPostId).ToListAsync();
        }

    }
}
