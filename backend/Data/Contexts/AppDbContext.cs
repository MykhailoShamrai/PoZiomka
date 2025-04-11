using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;

namespace backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<Answer> Answers {get; set;}
    public DbSet<StudentAnswers> StudentAnswersCollections {get; set;}
    public DbSet<Form> Forms {get; set;}
    public DbSet<ChoosablePreference> ChoosablePreferences {get; set;}
    public DbSet<ObligatoryPreference> ObligatoryPreferences {get; set;}
    public DbSet<OptionForObligatoryPreference> OptionsForObligatoryPreferences {get; set;}  


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Answer>().ToTable("Answers");
        modelBuilder.Entity<StudentAnswers>().ToTable("StudentAnswersCollections");
        modelBuilder.Entity<Form>().ToTable("Forms");
        modelBuilder.Entity<ChoosablePreference>().ToTable("ChoosablePreferences");
        modelBuilder.Entity<ObligatoryPreference>().ToTable("ObligatoryPreferences");
        modelBuilder.Entity<OptionForObligatoryPreference>().ToTable("OptionsForObligatoryPreferences");

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