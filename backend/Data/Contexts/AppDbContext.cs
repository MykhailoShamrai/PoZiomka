using backend.Models.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using backend.Models;

namespace backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<Application> Applications { get; set; }
    public DbSet<ApplicationAnswer> ApplicationAnswers { get; set; } 
    public DbSet<Answer> Answers {get; set;}
    public DbSet<Form> Forms {get; set;}
    public DbSet<Question> Questions {get; set;}
    public DbSet<OptionForQuestion> OptionsForQuestions {get; set;}
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Proposal> Proposals { get; set; }
    public DbSet<Communication> Communications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Application>().ToTable("Applications");
        modelBuilder.Entity<ApplicationAnswer>().ToTable("ApplicationAnswers");
        modelBuilder.Entity<Communication>().ToTable("Communication");
        modelBuilder.Entity<Proposal>().ToTable("Proposals");
        modelBuilder.Entity<Room>().ToTable("Rooms");
        modelBuilder.Entity<Answer>().ToTable("Answers");
        modelBuilder.Entity<Form>().ToTable("Forms");
        modelBuilder.Entity<Question>().ToTable("Questions");
        modelBuilder.Entity<OptionForQuestion>().ToTable("OptionsForQuestions");

        modelBuilder.Entity<OptionForQuestion>()
        .HasMany(opt => opt.AnswersWhichContains)
        .WithMany(answ => answ.ChosenOptions)
        .UsingEntity<Dictionary<string, object>>(
            "OptionForQuestion_Answer",
            j => j.HasOne<Answer>()
                  .WithMany()
                  .HasForeignKey("AnswerId")
                  .OnDelete(DeleteBehavior.Restrict),
            j => j.HasOne<OptionForQuestion>()
                  .WithMany()
                  .HasForeignKey("OptionForQuestionId")
                  .OnDelete(DeleteBehavior.Cascade),
            j => 
            {
                j.HasKey("AnswerId", "OptionForQuestionId");
            });

        modelBuilder.Entity<Application>()
            .HasOne(a => a.Answer)
            .WithOne(aa => aa.Application)
            .HasForeignKey<ApplicationAnswer>(aa => aa.ApplicationAnswerId);
        
    }
}