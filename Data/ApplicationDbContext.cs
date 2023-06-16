using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Mage.Models;

namespace Mage.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        //DbSet --> what models are, what database is named after
        public DbSet<Category> Categories { get; set; }
        public DbSet<Game> Games { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}