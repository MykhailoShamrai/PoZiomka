using backend.Models.User;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    
    // TODO: Entities - for now not used
    // public DbSet<StudentAnswers> StudentAnswers;
    // public DbSet<Room> Rooms;
    // public DbSet<Application> Applications;
    
    // Settings
    public DbSet<DisplayPreferences> DisplayPreferences { get; set; }
}