using backend.Data;
using backend.Dto;
using backend.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace backend.Tests.Services;

public class FormServiceTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateNewForm_ReturnsFalse_WhenFormExists()
    {
        var dbContext = GetInMemoryDbContext();
        dbContext.Forms.Add(new Form { NameOfForm = "ExistingForm", Questions = new List<Question>() });
        await dbContext.SaveChangesAsync();

        var service = new FormService(dbContext);

        var dto = new FormDto
        {
            FormName = "ExistingForm",
            Questions = [],
            Obligatoriness = [],
            NumberOfOptions = [],
            Options = []
        };

        var result = await service.CreateNewForm(dto);
        Assert.False(result);
    }

    [Fact]
    public async Task CreateNewForm_ReturnsTrue_WhenNewFormCreated()
    {
        var dbContext = GetInMemoryDbContext();
        var service = new FormService(dbContext);

        var dto = new FormDto
        {
            FormName = "NewForm",
            Questions = [],
            Obligatoriness = [],
            NumberOfOptions = [],
            Options = []
        };

        var result = await service.CreateNewForm(dto);
        Assert.True(result);

        var saved = await dbContext.Forms.FirstOrDefaultAsync(f => f.NameOfForm == "NewForm");
        Assert.NotNull(saved);
    }

    [Fact]
    public async Task AddNewObligatoryQuestionToForm_Throws_WhenFormNotFound()
    {
        var dbContext = GetInMemoryDbContext();
        var service = new FormService(dbContext);

        var dto = new AddQuestionDto
        {
            NameOfForm = "Nonexistent",
            Name = "Pytanie?",
            IsObligatory = true,
            Answers = new[] { "Tak", "Nie" }
        };

        await Assert.ThrowsAsync<ArgumentException>(() => service.AddNewObligatoryQuestionToForm(dto));
    }

    [Fact]
    public async Task AddNewObligatoryQuestionToForm_AddsQuestion_WhenFormExists()
    {
        var dbContext = GetInMemoryDbContext();
        var form = new Form
        {
            NameOfForm = "Form1",
            Questions = new List<Question>()
        };
        dbContext.Forms.Add(form);
        await dbContext.SaveChangesAsync();

        var service = new FormService(dbContext);

        var dto = new AddQuestionDto
        {
            NameOfForm = "Form1",
            Name = "Czy masz kota?",
            IsObligatory = true,
            Answers = new[] { "Tak", "Nie" }
        };

        var result = await service.AddNewObligatoryQuestionToForm(dto);
        Assert.True(result);

        var saved = await dbContext.Questions.Include(q => q.Options)
            .FirstOrDefaultAsync(q => q.Name == "Czy masz kota?");
        Assert.NotNull(saved);
        Assert.Equal(2, saved.Options.Count);
    }

    [Fact]
    public async Task DeleteForm_DeletesFormAndQuestions()
    {
        var dbContext = GetInMemoryDbContext();
        var form = new Form
        {
            NameOfForm = "Usuwalny",
            Questions = new List<Question>
            {
                new Question { Name = "Pyt1", Options = new List<OptionForQuestion>() }
            }
        };
        dbContext.Forms.Add(form);
        await dbContext.SaveChangesAsync();

        var service = new FormService(dbContext);
        var result = await service.DeleteForm("Usuwalny");

        Assert.True(result);
        Assert.False(await dbContext.Forms.AnyAsync());
        Assert.False(await dbContext.Questions.AnyAsync());
    }

    [Fact]
    public async Task DeleteQuestion_Throws_WhenNotExists()
    {
        var dbContext = GetInMemoryDbContext();
        var service = new FormService(dbContext);

        await Assert.ThrowsAsync<ArgumentException>(() => service.DeleteQuestion("Nieistniejące"));
    }

    [Fact]
    public async Task DeleteQuestion_RemovesQuestionAndOptions()
    {
        var dbContext = GetInMemoryDbContext();

        var question = new Question
        {
            Name = "Do usunięcia",
            Options = new List<OptionForQuestion>()
        };

        var option1 = new OptionForQuestion { Name = "Opcja A", Question = question };
        var option2 = new OptionForQuestion { Name = "Opcja B", Question = question };

        question.Options.Add(option1);
        question.Options.Add(option2);

        dbContext.Questions.Add(question);
        await dbContext.SaveChangesAsync();

        var service = new FormService(dbContext);
        var result = await service.DeleteQuestion("Do usunięcia");

        Assert.True(result);
        Assert.False(await dbContext.Questions.AnyAsync());
        Assert.False(await dbContext.OptionsForQuestions.AnyAsync());
    }

    [Fact]
    public async Task GetAll_ReturnsFormsWithQuestionsAndOptions()
    {
        var dbContext = GetInMemoryDbContext();

        // Utwórz pytanie bez opcji na początku
        var question = new Question
        {
            Name = "Jak się masz?",
            Options = new List<OptionForQuestion>()
        };

        // Dodaj opcje i przypisz do pytania
        var option1 = new OptionForQuestion { Name = "Dobrze", Question = question };
        var option2 = new OptionForQuestion { Name = "Źle", Question = question };

        question.Options.Add(option1);
        question.Options.Add(option2);

        // Utwórz formularz i przypisz pytanie
        var form = new Form
        {
            NameOfForm = "Form z pytaniem",
            Questions = new List<Question> { question }
        };

        dbContext.Forms.Add(form);
        await dbContext.SaveChangesAsync();

        var service = new FormService(dbContext);
        var result = await service.GetAll();

        Assert.Single(result);
        Assert.Single(result[0].Questions);
        Assert.Equal(2, result[0].Questions.First().Options.Count);
    }

}
