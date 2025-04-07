using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;

namespace backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<Answer> Answers;
    public DbSet<StudentAnswers> StudentAnswersCollections;
    public DbSet<Form> Forms;
    public DbSet<ChoosablePreference> ChoosablePreferences;
    public DbSet<ObligatoryPreference> ObligatoryPreferences;
    public DbSet<OptionForObligatoryPreference> OptionForObligatoryPreferences;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OptionForObligatoryPreference>()
        .HasMany(opt => opt.AnswersWhichContains)
        .WithMany(answ => answ.ObligatoryAnswers)
        .UsingEntity<Dictionary<string, object>>(
            "OptionForObligatoryPreference_Answer",
            j => j.HasOne<Answer>()
                  .WithMany()
                  .HasForeignKey("AnswerId")
                  .OnDelete(DeleteBehavior.ClientCascade),
            j => j.HasOne<OptionForObligatoryPreference>()
                  .WithMany()
                  .HasForeignKey("OptionForObligatoryPreferenceId")
                  .OnDelete(DeleteBehavior.ClientCascade),
            j => 
            {
                j.HasKey("AnswerId", "OptionForObligatoryPreferenceId");
            });
    }
}