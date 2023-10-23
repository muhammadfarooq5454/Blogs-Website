using Bloggie.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AuthDbContext _authDbContext;
        public UserRepository(AuthDbContext authDbContext)
        {
            _authDbContext = authDbContext;
        }
        public async Task<IEnumerable<IdentityUser>> GetAll()
        {
            var users = await _authDbContext.Users.ToListAsync();

            var superAdminUser = _authDbContext.Users.FirstOrDefault(x => x.Email == "superadmin@bloggie.com");
            
            if(superAdminUser != null)
            {
                users.Remove(superAdminUser);
            }    
            return users;
        }
        public async Task<IdentityUser> GetById(Guid id)
        {
            var userfromId =  await _authDbContext.Users.FirstOrDefaultAsync(x => x.Id == id.ToString());
            if(userfromId != null)
            {
                return userfromId;
            }
            return null;
        }
    }
}
