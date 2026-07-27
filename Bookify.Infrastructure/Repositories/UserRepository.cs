using Bookify.Domain.Users;

namespace Bookify.Infrastructure.Repositories;

internal sealed class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public override void Add(User user)
    {
        foreach (var role in user.Roles)
            // اذا فعلا كان موجود فماراح يضيف اللي موجود ثاني
            DbContext.Attach(role);

        DbContext.Add(user);
    }
}