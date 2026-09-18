using Microsoft.EntityFrameworkCore;


public class AppDb: DbContext
{
    public AppDb(DbContextOptions<AppDb> options) : base(options)
    {
    }
        public DbSet<Users> Users {get;set;}
        public DbSet<Sessions> Sessions {get;set;}
    
} 
