using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<StudentAnswers> StudentAnswers;
    public DbSet<Room> Rooms;
    public DbSet<Application> Applications;
}