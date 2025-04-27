using backend.Data;
using backend.Dto;
using backend.Services;
using Microsoft.EntityFrameworkCore;

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

        var dto = new FormCreateDto
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

        var dto = new FormCreateDto
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
            FormName = "Nonexistent",
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
            FormName = "Form1",
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

        var deleteDto = new DeleteQuestionDto() { FormName="Nieistniejące", QuestionName="Nieistniejące" };
        await Assert.ThrowsAsync<ArgumentException>(() => service.DeleteQuestion(deleteDto));
    }

    // [Fact]
    // public async Task DeleteQuestion_RemovesQuestionAndOptions()
    // {
    //     var dbContext = GetInMemoryDbContext();

    //     var form = new Form
    //     {
    //         NameOfForm = "Test Form",
    //         Questions = new List<Question>()
    //     };

    //     var question = new Question
    //     {
    //         Name = "Do usunięcia",
    //         FormForWhichCorrespond = form,
    //         Options = new List<OptionForQuestion>()
    //     };

    //     var option1 = new OptionForQuestion { Name = "Opcja A", Question = question };
    //     var option2 = new OptionForQuestion { Name = "Opcja B", Question = question };

    //     question.Options.Add(option1);
    //     question.Options.Add(option2);
    //     form.Questions.Add(question);

    //     var answer = new Answer
    //     {
    //         CorrespondingForm = form,
    //         UserId = 1,
    //         ChosenOptions = new List<OptionForQuestion> { option1 },
    //         Status = AnswerStatus.Saved
    //     };

    //     dbContext.Forms.Add(form);
    //     dbContext.Answers.Add(answer);
    //     await dbContext.SaveChangesAsync();

    //     var service = new FormService(dbContext);
    //     var deleteDto = new DeleteQuestionDto { FormName = "Test Form", QuestionName = "Do usunięcia" };
    //     var result = await service.DeleteQuestion(deleteDto);

    //     Assert.True(result);
    //     Assert.False(await dbContext.Questions.AnyAsync(q => q.Name == "Do usunięcia"));
    //     Assert.False(await dbContext.OptionsForQuestions.AnyAsync(o => o.Question.Name == "Do usunięcia"));
    // }

    [Fact]
    public async Task GetAll_ReturnsFormsWithQuestionsAndOptions()
    {
        var dbContext = GetInMemoryDbContext();

        var question = new Question
        {
            Name = "Jak się masz?",
            Options = new List<OptionForQuestion>()
        };

        var option1 = new OptionForQuestion { Name = "Dobrze", Question = question };
        var option2 = new OptionForQuestion { Name = "Źle", Question = question };

        question.Options.Add(option1);
        question.Options.Add(option2);

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
