using backend.Controllers;
using backend.Dto;
using backend.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace backend.Tests.Controllers
{
    public class AdminControllerTests
    {
        private readonly Mock<IFormsInterface> _formsMock;
        private readonly AdminController _controller;

        public AdminControllerTests()
        {
            _formsMock = new Mock<IFormsInterface>();
            _controller = new AdminController(_formsMock.Object);
        }

        [Fact]
        public async Task AddNewForm_ReturnsOk_WhenCreationSucceeds()
        {
            // Arrange
            var formDto = new FormDto();
            _formsMock.Setup(f => f.CreateNewForm(formDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.AddNewForm(formDto);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task AddNewForm_ReturnsBadRequest_WhenCreationFails()
        {
            var formDto = new FormDto();
            _formsMock.Setup(f => f.CreateNewForm(formDto)).ReturnsAsync(false);

            var result = await _controller.AddNewForm(formDto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task AddNewObligatoryQuestionTo_ReturnsOk_WhenAdditionSucceeds()
        {
            var dto = new AddQuestionDto
            {
                NameOfForm = "TestForm",
                Name = "TestQuestion",
                Answers = new List<string> { "Answer1", "Answer2" },
                IsObligatory = true
            };
            _formsMock.Setup(f => f.AddNewObligatoryQuestionToForm(dto)).ReturnsAsync(true);

            var result = await _controller.AddNewObligatoryQuestionTo(dto);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task AddNewObligatoryQuestionTo_ReturnsBadRequest_WhenAdditionFails()
        {
            var dto = new AddQuestionDto
            {
                NameOfForm = "TestForm",
                Name = "TestQuestion",
                Answers = new List<string> { "Answer1", "Answer2" },
                IsObligatory = true
            };

            _formsMock.Setup(f => f.AddNewObligatoryQuestionToForm(dto)).ReturnsAsync(false);

            var result = await _controller.AddNewObligatoryQuestionTo(dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteForm_ReturnsOk_WhenDeletionSucceeds()
        {
            string formName = "Form1";
            _formsMock.Setup(f => f.DeleteForm(formName)).ReturnsAsync(true);

            var result = await _controller.DeleteForm(formName);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task DeleteForm_ReturnsBadRequest_WhenDeletionFails()
        {
            string formName = "Form1";
            _formsMock.Setup(f => f.DeleteForm(formName)).ReturnsAsync(false);

            var result = await _controller.DeleteForm(formName);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteObligatoryQuestion_ReturnsOk_WhenDeletionSucceeds()
        {
            string questionName = "Question1";
            _formsMock.Setup(f => f.DeleteQuestion(questionName)).ReturnsAsync(true);

            var result = await _controller.DeleteObligatoryQuestion(questionName);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task DeleteObligatoryQuestion_ReturnsBadRequest_WhenDeletionFails()
        {
            string questionName = "Question1";
            _formsMock.Setup(f => f.DeleteQuestion(questionName)).ReturnsAsync(false);

            var result = await _controller.DeleteObligatoryQuestion(questionName);

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
