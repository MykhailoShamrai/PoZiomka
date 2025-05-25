using backend.Controllers;
using backend.Dto;
using backend.Interfaces;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace backend.Tests.Controllers
{
    public class AdminControllerTests
    {
        private readonly Mock<IFormsInterface> _formsMock;
        private readonly Mock<IAdminInterface> _adminMock;
        private readonly Mock<IRoomInterface> _roomsMock;
        private readonly Mock<IProposalInterface> _proposalMock;
        private readonly Mock<IJudgeInterface> _judgeMock;
        private readonly AdminController _controller;
        private readonly Mock<IApplicationInterface> _applicationMock;

        public AdminControllerTests()
        {
            _formsMock = new Mock<IFormsInterface>();
            _adminMock = new Mock<IAdminInterface>();
            _roomsMock = new Mock<IRoomInterface>();
            _proposalMock = new Mock<IProposalInterface>();
            _judgeMock = new Mock<IJudgeInterface>();
            _controller = new AdminController(_formsMock.Object, _adminMock.Object, _roomsMock.Object, _proposalMock.Object, _judgeMock.Object);
            _applicationMock = new Mock<IApplicationInterface>(); 
            _controller = new AdminController(_formsMock.Object, _adminMock.Object, _roomsMock.Object, _proposalMock.Object, _judgeMock.Object, _applicationMock.Object);
        }

        [Fact]
        public async Task AddNewForm_ReturnsOk_WhenCreationSucceeds()
        {
            // Arrange
            var formDto = new FormCreateDto();
            _formsMock.Setup(f => f.CreateNewForm(formDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.AddNewForm(formDto);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task AddNewForm_ReturnsBadRequest_WhenCreationFails()
        {
            var formDto = new FormCreateDto();
            _formsMock.Setup(f => f.CreateNewForm(formDto)).ReturnsAsync(false);

            var result = await _controller.AddNewForm(formDto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task AddNewObligatoryQuestionTo_ReturnsOk_WhenAdditionSucceeds()
        {
            var dto = new AddQuestionDto
            {
                FormName = "TestForm",
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
                FormName = "TestForm",
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
        public async Task DeleteObligatoryQuestion_ReturnsOk_WhenSuccess()
        {
            var dto = new DeleteQuestionDto
            {
                FormName = "Form1",
                QuestionName = "QuestionA"
            };
            _formsMock
                .Setup(f => f.DeleteQuestion(dto))
                .ReturnsAsync(true);

            var result = await _controller.DeleteObligatoryQuestion(dto);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task DeleteObligatoryQuestion_ReturnsBadRequest_WhenFailure()
        {
            var dto = new DeleteQuestionDto
            {
                FormName = "Form1",
                QuestionName = "QuestionA"
            };
            _formsMock
                .Setup(f => f.DeleteQuestion(dto))
                .ReturnsAsync(false);

            var result = await _controller.DeleteObligatoryQuestion(dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetUsersInformation_ReturnsOk_WhenDataExists()
        {
            var users = new List<UserDto>
            {
                new UserDto { Email = "a@a.com", Id = 1 },
                new UserDto { Email = "b@b.com", Id = 2 }
            };

            _adminMock
                .Setup(a => a.GetInformationAboutUsers())
                .ReturnsAsync(Tuple.Create(users, ErrorCodes.Ok));

            var result = await _controller.GetUsersInformation();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(users, ok.Value);
        }

        [Fact]
        public async Task GetUsersInformation_ReturnsNotFound_WhenDataMissing()
        {
            _adminMock
                .Setup(a => a.GetInformationAboutUsers())
                .ReturnsAsync(Tuple.Create<List<UserDto>, ErrorCodes>(null!, ErrorCodes.NotFound));

            var result = await _controller.GetUsersInformation();

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task AddRoleToUser_ReturnsBadRequest_WhenRoleInvalid()
        {
            var dto = new AddRoleToUserDto { Email = "x@x.com", Role = "Fake" };

            var result = await _controller.AddRoleToUser(dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task AddRoleToUser_ReturnsNotFound_WhenUserNotFound()
        {
            var dto = new AddRoleToUserDto { Email = "x@x.com", Role = "Admin" };
            _adminMock
                .Setup(a => a.SetRoleToUser(dto))
                .ReturnsAsync(ErrorCodes.NotFound);

            var result = await _controller.AddRoleToUser(dto);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task AddRoleToUser_ReturnsOk_WhenRoleAdded()
        {
            var dto = new AddRoleToUserDto { Email = "x@x.com", Role = "Admin" };
            _adminMock
                .Setup(a => a.SetRoleToUser(dto))
                .ReturnsAsync(ErrorCodes.Ok);

            var result = await _controller.AddRoleToUser(dto);

            Assert.IsType<OkResult>(result);
        }


        [Fact]
        public async Task AddNewRoom_ReturnsOk_WhenSuccess()
        {
            var dtos = new List<RoomInDto>();
            _roomsMock
                .Setup(r => r.AddRoom(It.IsAny<List<RoomInDto>>()))
                .Returns(Task.FromResult(ErrorCodes.Ok));

            var result = await _controller.AddNewRoom(dtos);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task AddNewRoom_ReturnsBadRequest_WhenFailure()
        {
            _roomsMock
                .Setup(r => r.AddRoom(It.IsAny<List<RoomInDto>>()))
                .Returns(Task.FromResult(ErrorCodes.BadRequest));

            var result = await _controller.AddNewRoom(new List<RoomInDto>());

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteRoom_ReturnsOk_WhenSuccess()
        {
            _roomsMock
                .Setup(r => r.DeleteRoom(It.IsAny<RoomInDto>()))
                .Returns(Task.FromResult(ErrorCodes.Ok));

            var result = await _controller.DeleteRoom(new RoomInDto());

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task DeleteRoom_ReturnsNotFound_WhenRoomNotExist()
        {
            _roomsMock
                .Setup(r => r.DeleteRoom(It.IsAny<RoomInDto>()))
                .Returns(Task.FromResult(ErrorCodes.NotFound));

            var result = await _controller.DeleteRoom(new RoomInDto());

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteRoom_ReturnsBadRequest_WhenFailure()
        {
            _roomsMock
                .Setup(r => r.DeleteRoom(It.IsAny<RoomInDto>()))
                .Returns(Task.FromResult(ErrorCodes.BadRequest));

            var result = await _controller.DeleteRoom(new RoomInDto());

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetAllProposals_ReturnsOk_WhenSuccess()
        {
            var list = new List<ProposalAdminOutDto>();
            _proposalMock
                .Setup(p => p.ReturnAllProposals())
                .Returns(Task.FromResult(
                    Tuple.Create<List<ProposalAdminOutDto>, ErrorCodes>(list, ErrorCodes.Ok)
                ));

            var result = await _controller.GetAllProposals();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(list, ok.Value);
        }

        [Fact]
        public async Task GetAllProposals_ReturnsNotFound_WhenEmpty()
        {
            _proposalMock
                .Setup(p => p.ReturnAllProposals())
                .Returns(Task.FromResult(
                    Tuple.Create<List<ProposalAdminOutDto>, ErrorCodes>(null!, ErrorCodes.NotFound)
                ));

            var result = await _controller.GetAllProposals();

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetAllProposals_ReturnsBadRequest_WhenFailure()
        {
            _proposalMock
                .Setup(p => p.ReturnAllProposals())
                .Returns(Task.FromResult(
                    Tuple.Create<List<ProposalAdminOutDto>, ErrorCodes>(null!, ErrorCodes.BadRequest)
                ));

            var result = await _controller.GetAllProposals();

            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public async Task SetStatusToRoom_ReturnsOk_WhenSuccess()
        {
            _roomsMock
                .Setup(r => r.ChangeStatusForRoom(It.IsAny<SetStatusToRoomDto>()))
                .Returns(Task.FromResult(ErrorCodes.Ok));

            var result = await _controller.SetStatusToRoom(new SetStatusToRoomDto());

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task SetStatusToRoom_ReturnsNotFound_WhenNoRoom()
        {
            _roomsMock
                .Setup(r => r.ChangeStatusForRoom(It.IsAny<SetStatusToRoomDto>()))
                .Returns(Task.FromResult(ErrorCodes.NotFound));

            var result = await _controller.SetStatusToRoom(new SetStatusToRoomDto());

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task SetStatusToRoom_ReturnsBadRequest_WhenFailure()
        {
            _roomsMock
                .Setup(r => r.ChangeStatusForRoom(It.IsAny<SetStatusToRoomDto>()))
                .Returns(Task.FromResult(ErrorCodes.BadRequest));

            var result = await _controller.SetStatusToRoom(new SetStatusToRoomDto());

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task AddUserToRoom_ReturnsOk_WhenSuccess()
        {
            var dto = new UserRoomDto { RoomId = 1, UserEmail = "a@a.com" };
            _roomsMock
                .Setup(r => r.ApplyUserToRoom(dto))
                .Returns(Task.FromResult(ErrorCodes.Ok));

            var result = await _controller.AddUserToRoom(dto);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task AddUserToRoom_ReturnsNotFound_WhenMissingData()
        {
            var dto = new UserRoomDto { RoomId = 1, UserEmail = "a@a.com" };
            _roomsMock
                .Setup(r => r.ApplyUserToRoom(dto))
                .Returns(Task.FromResult(ErrorCodes.NotFound));

            var result = await _controller.AddUserToRoom(dto);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task AddUserToRoom_ReturnsBadRequest_WhenCapacityExceeded()
        {
            var dto = new UserRoomDto { RoomId = 1, UserEmail = "a@a.com" };
            _roomsMock
                .Setup(r => r.ApplyUserToRoom(dto))
                .Returns(Task.FromResult(ErrorCodes.BadRequest));

            var result = await _controller.AddUserToRoom(dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task RemoveUserFromRoom_ReturnsOk_WhenSuccess()
        {
            var dto = new UserRoomDto { RoomId = 1, UserEmail = "a@a.com" };
            _roomsMock
                .Setup(r => r.RemoveUserFromRoom(dto))
                .Returns(Task.FromResult(ErrorCodes.Ok));

            var result = await _controller.RemoveUserFromRoom(dto);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task RemoveUserFromRoom_ReturnsNotFound_WhenMissingData()
        {
            var dto = new UserRoomDto { RoomId = 1, UserEmail = "a@a.com" };
            _roomsMock
                .Setup(r => r.RemoveUserFromRoom(dto))
                .Returns(Task.FromResult(ErrorCodes.NotFound));

            var result = await _controller.RemoveUserFromRoom(dto);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task RemoveUserFromRoom_ReturnsBadRequest_WhenFailure()
        {
            var dto = new UserRoomDto { RoomId = 1, UserEmail = "a@a.com" };
            _roomsMock
                .Setup(r => r.RemoveUserFromRoom(dto))
                .Returns(Task.FromResult(ErrorCodes.BadRequest));

            var result = await _controller.RemoveUserFromRoom(dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task AddTestProposal_ReturnsOk_WhenSuccess()
        {
            _proposalMock
                .Setup(p => p.AddTestProposal(It.IsAny<ProposalInDto>()))
                .Returns(Task.FromResult(ErrorCodes.Ok));

            var result = await _controller.AddTestProposal(new ProposalInDto());

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task AddTestProposal_ReturnsNotFound_WhenNoRoom()
        {
            _proposalMock
                .Setup(p => p.AddTestProposal(It.IsAny<ProposalInDto>()))
                .Returns(Task.FromResult(ErrorCodes.NotFound));

            var result = await _controller.AddTestProposal(new ProposalInDto());

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task AddTestProposal_ReturnsBadRequest_WhenFailure()
        {
            _proposalMock
                .Setup(p => p.AddTestProposal(It.IsAny<ProposalInDto>()))
                .Returns(Task.FromResult(ErrorCodes.BadRequest));

            var result = await _controller.AddTestProposal(new ProposalInDto());

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ChangeAdminStatus_ReturnsOk_WhenSuccess()
        {
            _proposalMock
                .Setup(p => p.AdminChangesStatusTheProposal(It.IsAny<AdminChangesStatusProposalDto>()))
                .Returns(Task.FromResult(ErrorCodes.Ok));

            var result = await _controller.ChangeAdminStatus(new AdminChangesStatusProposalDto());

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ChangeAdminStatus_ReturnsNotFound_WhenMissingProposal()
        {
            _proposalMock
                .Setup(p => p.AdminChangesStatusTheProposal(It.IsAny<AdminChangesStatusProposalDto>()))
                .Returns(Task.FromResult(ErrorCodes.NotFound));

            var result = await _controller.ChangeAdminStatus(new AdminChangesStatusProposalDto());

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task ChangeAdminStatus_ReturnsBadRequest_WhenBadArgument()
        {
            _proposalMock
                .Setup(p => p.AdminChangesStatusTheProposal(It.IsAny<AdminChangesStatusProposalDto>()))
                .Returns(Task.FromResult(ErrorCodes.BadArgument));

            var result = await _controller.ChangeAdminStatus(new AdminChangesStatusProposalDto());

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ChangeAdminStatus_ReturnsBadRequest_WhenFailure()
        {
            _proposalMock
                .Setup(p => p.AdminChangesStatusTheProposal(It.IsAny<AdminChangesStatusProposalDto>()))
                .Returns(Task.FromResult(ErrorCodes.BadRequest));

            var result = await _controller.ChangeAdminStatus(new AdminChangesStatusProposalDto());

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GenerateProposals_ReturnsOk_WhenSuccess()
        {
            _judgeMock
                .Setup(j => j.GenerateProposals())
                .Returns(Task.FromResult(JudgeError.Ok));

            var result = await _controller.GenerateProposals();

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task GenerateProposals_ReturnsNotFound_WhenStudentsNull()
        {
            _judgeMock
                .Setup(j => j.GenerateProposals())
                .Returns(Task.FromResult(JudgeError.StudentsAreNull));

            var result = await _controller.GenerateProposals();

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GenerateProposals_ReturnsNotFound_WhenRoomsNull()
        {
            _judgeMock
                .Setup(j => j.GenerateProposals())
                .Returns(Task.FromResult(JudgeError.RoomsAreNull));

            var result = await _controller.GenerateProposals();

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GenerateProposals_ReturnsBadRequest_WhenDatabaseError()
        {
            _judgeMock
                .Setup(j => j.GenerateProposals())
                .Returns(Task.FromResult(JudgeError.DatabaseError));

            var result = await _controller.GenerateProposals();

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetRooms_ReturnsOk_WhenSuccess()
        {
            var rooms = new List<RoomOutDto>
            {
                new RoomOutDto
                {
                    Id          = 1,
                    Floor       = 2,
                    Number      = 101,
                    Capacity    = 3,
                    FreePlaces  = 3,
                    Status      = RoomStatus.Available,
                    ResidentsIds= new List<int>()
                }
            };

            _roomsMock
                .Setup(r => r.GetRooms())
                .Returns(Task.FromResult(
                    Tuple.Create<List<RoomOutDto>, ErrorCodes>(rooms, ErrorCodes.Ok)
                ));

            var result = await _controller.GetRooms();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(rooms, ok.Value);
        }

        [Fact]
        public async Task GetRooms_ReturnsBadRequest_WhenFailure()
        {
            _roomsMock
                .Setup(r => r.GetRooms())
                .Returns(Task.FromResult(
                    Tuple.Create<List<RoomOutDto>, ErrorCodes>(null!, ErrorCodes.BadRequest)
                ));

            var result = await _controller.GetRooms();

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }

}
