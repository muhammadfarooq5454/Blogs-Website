using Bloggie.Web.Models.Domain;
using System.Collections;

namespace Bloggie.Web.Repositories
{
    //Working krrhe hain hum Repository Pattern banane ki koshish krrhe hain hum
    public interface ITagRepository
    {
        //Interface for Tags Method CRUD

        Task<IEnumerable<Tag>> GetAllAsync();

        //Single Tag back from database
        Task<Tag?> GetAsync(Guid id);

        //Add to the database
        Task<Tag> AddAsync(Tag tag);

        //Nullable return so the tag can be null or found
        Task<Tag?> UpdateAsync(Tag tag);

        //Nullable return so the tag can be null or found
        Task<Tag?> DeleteAsync(Guid id);
    }
}
