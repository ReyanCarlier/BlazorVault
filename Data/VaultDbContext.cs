using Microsoft.EntityFrameworkCore;
using BlazorVault.Data.Models;

namespace BlazorVault.Data
{
    /** 
     * The database context for the application.
     *      Users - The users in the database.
     *      Passwords - The passwords in the database.
     *      Categories - The categories in the database.
     *      Roles - The roles in the database.
     *      WebsiteIcons - The website icons in the database.
     *      Groups - The groups in the database.
     */
    public class VaultDbContext(DbContextOptions<VaultDbContext> options) : DbContext(options)
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Password> Passwords { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<WebsiteIcon> WebsiteIcons { get; set; }
        public DbSet<Group> Groups { get; set; }
    }
}
