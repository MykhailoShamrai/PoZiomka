using backend.Models.User;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    
    // Entities
    public DbSet<StudentAnswers> StudentAnswers;
    public DbSet<Room> Rooms;
    public DbSet<Application> Applications;
    
    // Settings
    public DbSet<DisplayPreferences> DisplayPreferences;
}