using Bloggie.Web.Models.Domain;
using System.Collections;

namespace Bloggie.Web.Repositories
{
    public interface IBlogPostRepository
    {
        Task<IEnumerable<BlogPost>> GetAllAsync();
        
        Task<BlogPost?> GetAsync(Guid id);
        
        Task<BlogPost> AddAsync(BlogPost blogpost);
        
        Task<BlogPost?> UpdateAsync(BlogPost blogPost);

        Task<BlogPost?> DeleteAsync(Guid id);

        Task<BlogPost?> GetByUrlHandleAsync(string urlHandle);
    }
}
