using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MyIdentity.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category>? Category { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // cắt bỏ chữ AspNet trong tên của table
            var entities = builder.Model.GetEntityTypes();
            foreach (var entityType in entities)
            {
                string? name = entityType.GetTableName();
                if (name!.StartsWith("AspNet"))
                {
                    entityType.SetTableName(name.Substring(6));
                }
            }
        }
    }
}