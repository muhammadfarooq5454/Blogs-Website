using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Repositories
{
    public class BlogPostCommentRepository : IBlogPostCommentRepository
    {
        private readonly BloggieDbContext _bloggieDbContext;
        public BlogPostCommentRepository(BloggieDbContext bloggieDbContext)
        {
            _bloggieDbContext = bloggieDbContext;
        }
        public async Task<BlogPostcomment> AddAsync(BlogPostcomment blogPostcomment)
        {
            await _bloggieDbContext.BlogPostcomment.AddAsync(blogPostcomment);

            await _bloggieDbContext.SaveChangesAsync();

            return blogPostcomment;
        }
        public async Task<IEnumerable<BlogPostcomment>> GetTotalCommentsAsync(Guid blogPostId)
        {
            return await _bloggieDbContext.BlogPostcomment.Where(x => x.BlogPostId == blogPostId).ToListAsync();
        }
    }
}
