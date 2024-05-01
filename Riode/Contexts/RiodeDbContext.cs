using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Riode.Models;

namespace Riode.Contexts
{
    public class RiodeDbContext : IdentityDbContext<AppUser>
    {
        public RiodeDbContext(DbContextOptions<RiodeDbContext> options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Blog> Blogs { get; set; }

        public DbSet<BlogImage> BlogImages { get; set; }
        public DbSet<BlogTopic> BlogTopics { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<ContactComment> ContactComments { get; set; }
        
    }
}
