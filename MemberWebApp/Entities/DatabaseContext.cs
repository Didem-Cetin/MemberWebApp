using Microsoft.EntityFrameworkCore;

namespace MemberWebApp.Entities
{
    public class DatabaseContext:DbContext
    {
       
            public DatabaseContext(DbContextOptions options) : base(options)
            {
                //Newwlememek için yaptık...
            }

            public DbSet<User> Users { get; set; }
        
    }
}
