namespace Hangfire.Community.Outbox.Xaf.Services;

using Microsoft.EntityFrameworkCore;

public class DbContextAccessor: IDbContextAccessor
{
    private readonly Func<DbContext> _dbContextFactory;

    public DbContextAccessor(Func<DbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }
    
    public DbContext GetDbContext()
    {
        return _dbContextFactory();
    }
}

public interface IDbContextAccessor
{
    DbContext GetDbContext();
}