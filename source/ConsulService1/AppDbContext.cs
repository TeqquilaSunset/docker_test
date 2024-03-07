using ConsulService1.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace ConsulService1
{
    public class AppDbContext : DbContext
    {
        public DbSet<Prediction> Predictions { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.Migrate();
        }

    }
}
