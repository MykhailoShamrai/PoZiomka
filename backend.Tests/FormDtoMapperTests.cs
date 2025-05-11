using backend.Mappers;
using backend.Dto;

public class FormDtoMapperTests
{
    [Fact]
    public void DtoToForm_CreatesCorrectForm_ForValidInput()
    {
        var dto = new FormCreateDto
        {
            FormName = "F1",
            Questions = new[] { "Q1", "Q2" }.ToList(),
            NumberOfOptions = new[] { 2, 1 }.ToList(),
            Obligatoriness = new[] { true, false }.ToList(),
            Options = new[] { "A1", "A2", "B1" }.ToList()
        };

        var form = dto.DtoToForm();

        Assert.Equal("F1", form.NameOfForm);
        Assert.Equal(2, form.Questions.Count);
        Assert.Equal("Q1", form.Questions[0].Name);
        Assert.True(form.Questions[0].IsObligatory);
        Assert.Equal(2, form.Questions[0].Options.Count);
        Assert.Equal("A1", form.Questions[0].Options[0].Name);
        Assert.Equal("B1", form.Questions[1].Options.Single().Name);
    }

    [Fact]
    public void DtoToForm_Throws_WhenQuestionsCountMismatchOptionsCount()
    {
        var dto = new FormCreateDto
        {
            FormName = "X",
            Questions = new[] { "Q1" }.ToList(),
            NumberOfOptions = new[] { 1, 2 }.ToList(),
            Obligatoriness = new[] { true, false }.ToList(),
            Options = new[] { "A" }.ToList()
        };

        Assert.Throws<InvalidDataException>(() => dto.DtoToForm());
    }

    [Fact]
    public void CheckIfNumberOfAnswersIsSameAsDeclared_WorksCorrectly()
    {
        Assert.True(FormDtoMapper
            .CheckIfNumberOfAnswersIsSameAsDeclared(new[] { 1, 2 }, new[] { "a", "b", "c" }));
        Assert.False(FormDtoMapper
            .CheckIfNumberOfAnswersIsSameAsDeclared(new[] { 2, 2 }, new[] { "a", "b", "c" }));
    }
}
