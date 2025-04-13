using Xunit;
using backend.Mappers;

namespace backend.Tests
{
    public class FormServiceTests
    {
        [Fact]
        public void CheckIfNumberOfAnswersIsSameAsDeclared_ReturnsTrue_WhenCountsAreCorrect()
        {
            var numberOfAnswers = new List<int> { 2, 3 };
            var answers = new List<string> { "Answer1", "Answer2", "Answer3", "Answer4", "Answer5" };

            var result = FormDtoMapper.CheckIfNumberOfAnswersIsSameAsDeclared(numberOfAnswers, answers);

            Assert.True(result);
        }

        [Fact]
        public void CheckIfNumberOfAnswersIsSameAsDeclared_ReturnsFalse_WhenCountsAreNotCorrect()
        {
            var numberOfAnswers = new List<int> { 1, 5 };
            var answers = new List<string> { "Answer1", "Answer2", "Answer3", "Answer4", "Answer5" };

            var result = FormDtoMapper.CheckIfNumberOfAnswersIsSameAsDeclared(numberOfAnswers, answers);

            Assert.False(result);
        }

    }
}