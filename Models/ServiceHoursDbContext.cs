using Microsoft.EntityFrameworkCore;

namespace WahsKeyClubSite.Models
{
    public class ServiceHoursDbContext : DbContext
    {
        public ServiceHoursDbContext (DbContextOptions<ServiceHoursDbContext> options)
            : base(options)
        {
        }

        public DbSet<ServiceHours> ServiceHours { get; set; }
    }
}