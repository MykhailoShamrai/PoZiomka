using backend.Dto;
using backend.Interfaces;
using backend.Models.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using backend.Data;
using Microsoft.EntityFrameworkCore;
using backend.Repositories;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace backend.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserInterface> _userRepoMock;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<AppDbContext> _dbContextMock;
        private readonly Mock<IProposalInterface> _proposalMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<IApplicationInterface> _applicationInterface;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _userRepoMock = new Mock<IUserInterface>();
            _userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(),
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<User>>().Object,
                Array.Empty<IUserValidator<User>>(),
                Array.Empty<IPasswordValidator<User>>(),
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<User>>>().Object
            );
            _proposalMock = new Mock<IProposalInterface>();
            _dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _applicationInterface = new Mock<IApplicationInterface>();

            _controller = new UserController(
                _userRepoMock.Object, _proposalMock.Object, _applicationInterface.Object);
        }
        [Fact]
        public async Task ChangeMyPreferences_ReturnsOk_WhenSuccessful()
        {
            var prefs = new UserPreferences();
            _userRepoMock.Setup(repo => repo.ChangeUserPreferences(prefs)).ReturnsAsync(ErrorCodes.Ok);

            var result = await _controller.ChangeMyPreferences(prefs);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task DisplayProfile_ReturnsOk_WhenSuccessful()
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Email, "test@example.com") };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = principal };

            _httpContextAccessorMock.Setup(c => c.HttpContext).Returns(httpContext);

            var profileDto = new ProfileDisplayDto
            {
                FirstName = "TestUser",
                Email = "test@example.com"
            };

            _userRepoMock.Setup(repo => repo.DisplayUserProfile())
                .Returns(Task.FromResult(Tuple.Create(ErrorCodes.Ok, (ProfileDisplayDto?)profileDto)));

            var result = await _controller.DisplayProfile();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(profileDto, okResult.Value);
        }


        [Fact]
        public async Task ChangeProfile_ReturnsOk_WhenSuccessful()
        {
            var dto = new UpdateUserDto();
            _userRepoMock.Setup(r => r.ChangeUserProfile(dto)).ReturnsAsync(ErrorCodes.Ok);

            var result = await _controller.ChangeProfile(dto);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ChangeMyPreferences_ReturnsNotFound_WhenUserMissing()
        {
            var prefs = new UserPreferences();
            _userRepoMock.Setup(r => r.ChangeUserPreferences(prefs))
                         .ReturnsAsync(ErrorCodes.NotFound);

            var result = await _controller.ChangeMyPreferences(prefs);

            var nf = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User not found.", nf.Value);
        }

        [Fact]
        public async Task ChangeMyPreferences_ReturnsUnauthorized_WhenCookieFails()
        {
            var prefs = new UserPreferences();
            _userRepoMock.Setup(r => r.ChangeUserPreferences(prefs))
                         .ReturnsAsync(ErrorCodes.CannotRetrieveUserFromCookie);

            var result = await _controller.ChangeMyPreferences(prefs);

            var ua = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Cookie retrieval failed.", ua.Value);
        }

        [Fact]
        public async Task ChangeMyPreferences_ReturnsBadRequest_WhenDbFails()
        {
            var prefs = new UserPreferences();
            _userRepoMock.Setup(r => r.ChangeUserPreferences(prefs))
                         .ReturnsAsync(ErrorCodes.UpdateUserDbFailed);

            var result = await _controller.ChangeMyPreferences(prefs);

            var br = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Db update failed.", br.Value);
        }

        [Fact]
        public async Task DisplayProfile_ReturnsUnauthorized_WhenRepoSaysUnauthorized()
        {
            _userRepoMock.Setup(r => r.DisplayUserProfile())
                         .Returns(Task.FromResult(
                             Tuple.Create<ErrorCodes, ProfileDisplayDto?>(ErrorCodes.Unauthorized, null)
                         ));

            var result = await _controller.DisplayProfile();

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task DisplayProfile_ReturnsNotFound_WhenRepoSaysNotFound()
        {
            _userRepoMock.Setup(r => r.DisplayUserProfile())
                         .Returns(Task.FromResult(
                             Tuple.Create<ErrorCodes, ProfileDisplayDto?>(ErrorCodes.NotFound, null)
                         ));

            var result = await _controller.DisplayProfile();

            var nf = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User not found.", nf.Value);
        }

        [Fact]
        public async Task ChangeProfile_ReturnsNotFound_WhenForbidden()
        {
            var dto = new UpdateUserDto();
            _userRepoMock.Setup(r => r.ChangeUserProfile(dto))
                         .ReturnsAsync(ErrorCodes.Forbidden);

            var result = await _controller.ChangeProfile(dto);

            var nf = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Forbidden", nf.Value);
        }


        [Fact]
        public async Task GetForms_ReturnsUnauthorized_WhenUnauthorized()
        {
            _userRepoMock
                .Setup(r => r.GetUserForms())
                .ReturnsAsync((ErrorCodes.Unauthorized, Array.Empty<FormDto>()));

            Assert.IsType<UnauthorizedResult>(await _controller.GetForms());
        }

        [Fact]
        public async Task GetForms_ReturnsNotFound_WhenEmpty()
        {
            _userRepoMock
                .Setup(r => r.GetUserForms())
                .ReturnsAsync((ErrorCodes.NotFound, Array.Empty<FormDto>()));

            var nf = await _controller.GetForms();
            Assert.IsType<NotFoundObjectResult>(nf);
        }

        [Fact]
        public async Task GetForms_ReturnsOk_WithForms()
        {
            var arr = new[]
            {
        new FormDto
        {
            FormId     = 1,
            NameOfForm = "Pierwszy formularz",
            Questions  = new List<Question>()
        },
        new FormDto
        {
            FormId     = 2,
            NameOfForm = "Drugi formularz",
            Questions  = new List<Question>()
        }
    };
            _userRepoMock
                .Setup(r => r.GetUserForms())
                .ReturnsAsync((ErrorCodes.Ok, arr));

            var ok = await _controller.GetForms() as OkObjectResult;
            Assert.Equal(arr, ok?.Value);
        }

        [Fact]
        public async Task SubmitAnswer_ReturnsNotFound_WhenFormMissing()
        {
            var dto = new AnswerDto();
            _userRepoMock.Setup(r => r.SubmitAnswerForForms(dto))
                         .ReturnsAsync(ErrorCodes.NotFound);

            var result = await _controller.SubmitAnswer(dto);

            var nf = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Form not found!", nf.Value);
        }

        [Fact]
        public async Task SubmitAnswer_ReturnsBadRequest_WhenOptionInvalid()
        {
            var dto = new AnswerDto();
            _userRepoMock.Setup(r => r.SubmitAnswerForForms(dto))
                         .ReturnsAsync(ErrorCodes.BadRequest);

            var result = await _controller.SubmitAnswer(dto);

            var br = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Option doesn't correspond to question from form!", br.Value);
        }

        [Fact]
        public async Task GetMyProposals_ReturnsUnauthorized_WhenUnauthorized()
        {
            _proposalMock
              .Setup(p => p.ReturnUsersProposals())
              .Returns(Task.FromResult(
                  Tuple.Create<List<ProposalUserOutDto>, ErrorCodes>(null!, ErrorCodes.Unauthorized)
              ));

            var result = await _controller.GetMyProposals();

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task GetMyProposals_ReturnsBadRequest_WhenBadRequest()
        {
            _proposalMock
              .Setup(p => p.ReturnUsersProposals())
              .Returns(Task.FromResult(
                  Tuple.Create<List<ProposalUserOutDto>, ErrorCodes>(null!, ErrorCodes.BadRequest)
              ));

            var result = await _controller.GetMyProposals();

            var br = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(
              "Something went wrong while fetching information about proposals!",
              br.Value
            );
        }

        [Fact]
        public async Task AnswerProposal_ReturnsNotFound_WhenMissing()
        {
            var dto = new UserChangesStatusProposalDto();
            _proposalMock.Setup(p => p.UserAnswersTheProposal(dto))
                         .ReturnsAsync(ErrorCodes.NotFound);

            var result = await _controller.AnswerProposal(dto);

            var nf = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Something went wrong in fetching proposal information!", nf.Value);
        }

        [Fact]
        public async Task AnswerProposal_ReturnsBadRequest_WhenBadArgument()
        {
            var dto = new UserChangesStatusProposalDto();
            _proposalMock.Setup(p => p.UserAnswersTheProposal(dto))
                         .ReturnsAsync(ErrorCodes.BadArgument);

            var result = await _controller.AnswerProposal(dto);

            var br = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("it is not Your proposal!", br.Value);
        }
    }
}
