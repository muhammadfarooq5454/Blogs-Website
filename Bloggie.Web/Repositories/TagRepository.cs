using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Repositories
{
    public class TagRepository : ITagRepository
    {
        //Cannot use bloggieDbcontext class parameter inside this constructor so
        //I make a private object to get and set it
        private readonly BloggieDbContext _bloggieDbcontext;

        public TagRepository(BloggieDbContext bloggieDbcontext)
        {
            _bloggieDbcontext = bloggieDbcontext;
        }

        //Implement Methods over here. The Code is controlled by the repository not by the controller itself yhi tu
        //Repository Design Pattern kehlata hai 
        public async Task<Tag> AddAsync(Tag tag)
        {
            await _bloggieDbcontext.Tags.AddAsync(tag);

            //So that our context can save change to our database in sql server
            await _bloggieDbcontext.SaveChangesAsync();

            return tag;
        }

        public async Task<Tag?> DeleteAsync(Guid id)
        {
            var existing = await _bloggieDbcontext.Tags.FindAsync(id);
            if (existing != null)
            {
                //Removing from database with ID == existing
                _bloggieDbcontext.Tags.Remove(existing);

                await _bloggieDbcontext.SaveChangesAsync();
                
                return existing; 
            }
            return null;
        }

        public async Task<IEnumerable<Tag>> GetAllAsync()
        {
            return await _bloggieDbcontext.Tags.ToListAsync();   
        }

        public async Task<Tag?> GetAsync(Guid id)
        {
            //1st Method
            //var tag = _bloggieDbcontext.Tags.Find(id);

            //2nd Method 
            //Linq mai use kra tha IPT mai database se uth kr arha hai agr id match hojati hai tu aur ye id yahan views se hi ai hai asp-route-id="@tag.id" se
            return await _bloggieDbcontext.Tags.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Tag?> UpdateAsync(Tag tag)
        {
            var existing = await _bloggieDbcontext.Tags.FindAsync(tag.Id);

            if (existing != null)
            {
                existing.Name = tag.Name;

                existing.DisplayName = tag.DisplayName;

                await _bloggieDbcontext.SaveChangesAsync();

                return existing;
            }    
            return null;
        }   
    }
}
