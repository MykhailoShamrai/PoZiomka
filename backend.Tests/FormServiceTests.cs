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

        var deleteDto = new DeleteQuestionDto() { FormName = "Nieistniejące", QuestionName = "Nieistniejące" };
        await Assert.ThrowsAsync<ArgumentException>(() => service.DeleteQuestion(deleteDto));
    }

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

    [Fact]
    public async Task DeleteQuestion_RemovesQuestionAndOptions()
    {
        var dbContext = GetInMemoryDbContext();

        var form = new Form
        {
            NameOfForm = "Test Form",
            Questions = new List<Question>()
        };
        var question = new Question
        {
            Name = "Do usunięcia",
            FormForWhichCorrespond = form,
            Options = new List<OptionForQuestion>()
        };
        var option1 = new OptionForQuestion { Name = "Opcja A", Question = question };
        var option2 = new OptionForQuestion { Name = "Opcja B", Question = question };
        question.Options.Add(option1);
        question.Options.Add(option2);
        form.Questions.Add(question);

        dbContext.Forms.Add(form);
        await dbContext.SaveChangesAsync();

        var service = new FormService(dbContext);
        var deleteDto = new DeleteQuestionDto { FormName = "Test Form", QuestionName = "Do usunięcia" };
        var result = await service.DeleteQuestion(deleteDto);
        Assert.True(result);

        Assert.False(await dbContext.OptionsForQuestions.AnyAsync());

        Assert.True(await dbContext.Questions.AnyAsync(q => q.Name == "Do usunięcia"));
    }

    [Fact]
    public async Task GetAnswers_ReturnsAnswerWithChosenOptions()
    {
        var dbContext = GetInMemoryDbContext();

        var form = new Form { NameOfForm = "F", Questions = new List<Question>() };
        var question = new Question { Name = "Q", FormForWhichCorrespond = form, Options = new List<OptionForQuestion>() };
        var option = new OptionForQuestion { Name = "O", Question = question };
        question.Options.Add(option);
        form.Questions.Add(question);
        dbContext.Forms.Add(form);

        var answer = new Answer
        {
            CorrespondingForm = form,
            UserId = 42,
            ChosenOptions = new List<OptionForQuestion> { option },
            Status = AnswerStatus.Saved
        };
        dbContext.Answers.Add(answer);
        await dbContext.SaveChangesAsync();

        var service = new FormService(dbContext);
        var result = await service.GetAnswers(42, form.FormId);

        Assert.NotNull(result);
        Assert.Equal(42, result.UserId);
        Assert.Single(result.ChosenOptions);
        Assert.Equal("Q", result.ChosenOptions.First().Question.Name);
    }

    [Fact]
    public async Task FindFormWithQuestions_ReturnsFormWithQuestions()
    {
        var dbContext = GetInMemoryDbContext();
        var form = new Form
        {
            NameOfForm = "X",
            Questions = new List<Question> { new Question { Name = "Q1", Options = new List<OptionForQuestion>() } }
        };
        dbContext.Forms.Add(form);
        await dbContext.SaveChangesAsync();

        var service = new FormService(dbContext);
        var result = await service.FindFormWithQuestions(form.FormId);

        Assert.NotNull(result);
        Assert.Single(result!.Questions);
    }

    [Fact]
    public async Task FindQuestionsForForm_ReturnsCorrectQuestions()
    {
        var dbContext = GetInMemoryDbContext();
        var form = new Form { NameOfForm = "Y", Questions = new List<Question>() };
        var q1 = new Question { Name = "A", FormForWhichCorrespond = form, Options = new List<OptionForQuestion>() };
        var q2 = new Question { Name = "B", FormForWhichCorrespond = form, Options = new List<OptionForQuestion>() };
        dbContext.Forms.Add(form);
        dbContext.Questions.AddRange(q1, q2);
        await dbContext.SaveChangesAsync();

        var service = new FormService(dbContext);
        var list = await service.FindQuestionsForForm(form.FormId);

        Assert.Contains(list, q => q.Name == "A");
        Assert.Contains(list, q => q.Name == "B");
    }

    [Fact]
    public async Task FindForm_ByNameAndById_ReturnsCorrect()
    {
        var dbContext = GetInMemoryDbContext();
        var form = new Form
        {
            NameOfForm = "Z",
            Questions = new List<Question>()
        };
        dbContext.Forms.Add(form);
        await dbContext.SaveChangesAsync();

        var service = new FormService(dbContext);
        var byName = await service.FindForm("Z");
        var byId = await service.FindForm(form.FormId);

        Assert.NotNull(byName);
        Assert.Equal("Z", byName!.NameOfForm);
        Assert.NotNull(byId);
        Assert.Equal(form.FormId, byId!.FormId);
    }

    [Fact]
    public async Task FindOptions_ReturnsSpecifiedOptions()
    {
        var dbContext = GetInMemoryDbContext();

        var dummyForm = new Form { NameOfForm = "X", Questions = new List<Question>() };
        var question = new Question
        {
            Name = "Q",
            FormForWhichCorrespond = dummyForm,
            Options = new List<OptionForQuestion>()
        };
        dummyForm.Questions.Add(question);
        dbContext.Forms.Add(dummyForm);

        var opt1 = new OptionForQuestion
        {
            OptionForQuestionId = 1,
            Name = "A",
            Question = question
        };
        var opt2 = new OptionForQuestion
        {
            OptionForQuestionId = 2,
            Name = "B",
            Question = question
        };
        dbContext.OptionsForQuestions.AddRange(opt1, opt2);
        await dbContext.SaveChangesAsync();

        var service = new FormService(dbContext);
        var list = await service.FindOptions(new List<int> { 2 });

        Assert.Single(list);
        Assert.Equal(2, list[0].OptionForQuestionId);
    }


    [Fact]
    public async Task FindStatusForAnswer_ReturnsSavedOrEditingOrThrows()
    {
        var dbContext = GetInMemoryDbContext();
        var form = new Form { NameOfForm = "F", Questions = new List<Question>() };
        var q1 = new Question { Name = "Q1", FormForWhichCorrespond = form, Options = new List<OptionForQuestion>() };
        var q2 = new Question { Name = "Q2", FormForWhichCorrespond = form, Options = new List<OptionForQuestion>() };
        form.Questions.Add(q1);
        form.Questions.Add(q2);
        dbContext.Forms.Add(form);
        await dbContext.SaveChangesAsync();

        var service = new FormService(dbContext);
        var opt1 = new OptionForQuestion
        {
            Name = "O1",
            Question = q1
        };
        var opt2 = new OptionForQuestion
        {
            Name = "O2",
            Question = q2
        };
        dbContext.OptionsForQuestions.AddRange(opt1, opt2);
        await dbContext.SaveChangesAsync();

        var otherForm = new Form { NameOfForm = "Other", Questions = new List<Question>() };
        var otherQ = new Question
        {
            Name = "O",
            FormForWhichCorrespond = otherForm,
            Options = new List<OptionForQuestion>()
        };
        dbContext.Forms.Add(otherForm);
        dbContext.Questions.Add(otherQ);

        var badOpt = new OptionForQuestion
        {
            Name = "Bad",
            Question = otherQ
        };
        dbContext.OptionsForQuestions.Add(badOpt);
        await dbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.FindStatusForAnswer(new List<OptionForQuestion> { badOpt }, form)
        );
    }
}
