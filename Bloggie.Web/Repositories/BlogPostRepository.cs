using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Bloggie.Web.Repositories
{
    public class BlogPostRepository : IBlogPostRepository
    {
        //Cannot use bloggieDbcontext class parameter inside this constructor so
        //I make a private object to get and set it
        private readonly BloggieDbContext _bloggieDbcontext;
        public BlogPostRepository(BloggieDbContext bloggieDbContext)
        {
            _bloggieDbcontext = bloggieDbContext;
        }

        public async Task<BlogPost> AddAsync(BlogPost blogPost)
        {
            await _bloggieDbcontext.BlogPosts.AddAsync(blogPost);

            await _bloggieDbcontext.SaveChangesAsync();

            return blogPost;
        }

        public async Task<BlogPost?> DeleteAsync(Guid id)
        {
            var blogPostfound = await _bloggieDbcontext.BlogPosts.Include(x => x.Tags).FirstOrDefaultAsync(x => x.Id == id);

            if (blogPostfound != null) 
            {
                _bloggieDbcontext.BlogPosts.Remove(blogPostfound);

                await _bloggieDbcontext.SaveChangesAsync();

                return blogPostfound;
            }

            return null; 
        }

        public async Task<IEnumerable<BlogPost>> GetAllAsync()
        {
           return await _bloggieDbcontext.BlogPosts.Include(x => x.Tags).ToListAsync();
        }

        public async Task<BlogPost?> GetAsync(Guid id)
        {
            return await _bloggieDbcontext.BlogPosts.Include(x => x.Tags).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<BlogPost?> GetByUrlHandleAsync(string urlHandle)
        {
            var existing = await _bloggieDbcontext.BlogPosts.Include(x => x.Tags).FirstOrDefaultAsync(x => x.UrlHandle == urlHandle);
        
            if(existing != null)
            {
                return existing;
            }
            return null;
        
        }

        public async Task<BlogPost?> UpdateAsync(BlogPost blogPost)
        {
            var existing = await _bloggieDbcontext.BlogPosts.Include(x => x.Tags).FirstOrDefaultAsync(x => x.Id == blogPost.Id);

            if(existing != null)
            {
                //Adding domain model in database 
                existing.Heading = blogPost.Heading;

                existing.Content = blogPost.Content;

                existing.FeaturedImageUrl = blogPost.FeaturedImageUrl;

                existing.UrlHandle = blogPost.UrlHandle;

                existing.Author = blogPost.Author;

                existing.ShortDescription = blogPost.ShortDescription;

                existing.PublishedDate = blogPost.PublishedDate;

                existing.Visible = blogPost.Visible;

                existing.Tags = blogPost.Tags;

                await _bloggieDbcontext.SaveChangesAsync();

                return existing;
            }
            return null;
        }
    }
}
