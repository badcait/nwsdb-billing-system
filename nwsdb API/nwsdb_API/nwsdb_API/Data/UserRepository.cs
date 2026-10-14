using Microsoft.EntityFrameworkCore;
using nwsdb_API.Model;
namespace nwsdb_API.Data;
public class UserRepository(AppDbContext context)
{
    public Task<AppUser?> FindByUsernameAsync(string username) => context.Users.AsNoTracking()
        .Include(x => x.Partner).SingleOrDefaultAsync(x => x.Username == username);
}
