using Bloggie.Web.Models.Domain;

namespace Bloggie.Web.Repositories
{
    public interface IBlogPostCommentRepository
    {
        Task<BlogPostcomment> AddAsync(BlogPostcomment blogPostcomment);

        Task<IEnumerable<BlogPostcomment>> GetTotalCommentsAsync(Guid blogPostId);
    }
}
