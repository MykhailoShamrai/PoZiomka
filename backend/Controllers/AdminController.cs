using backend.Dto;
using backend.Interfaces;
using backend.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[Route("api/[controller]")]
[Authorize("Admin")]
public class AdminController : ControllerBase
{
    private readonly IFormsInterface _formsInterface;
    private readonly IAdminInterface _adminInterface;
    private readonly IRoomInterface _roomInterface;
    private readonly IProposalInterface _proposalInterface;
    private readonly IJudgeInterface _judgeInterface;
    private readonly IApplicationInterface _applicationInterface;
    public AdminController(IFormsInterface formsInterface,
                            IAdminInterface adminInterface,
                            IRoomInterface roomInterface,
                            IProposalInterface proposalInterface,
                            IJudgeInterface judgeInterface,
                            IApplicationInterface applicationInterface)
    {
        _formsInterface = formsInterface;
        _adminInterface = adminInterface;
        _roomInterface = roomInterface;
        _proposalInterface = proposalInterface;
        _judgeInterface = judgeInterface;
        _applicationInterface = applicationInterface;
    }

    [HttpPost]
    [Route("add_new_form")]
    public async Task<IActionResult> AddNewForm([FromBody] FormCreateDto formDto)
    {
        try
        {
            if (await _formsInterface.CreateNewForm(formDto))
                return Ok();
            else
                return BadRequest("Something went wrong while adding new form!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    [Route("add_question")]
    public async Task<IActionResult> AddNewObligatoryQuestionTo([FromBody] AddQuestionDto preferenceDto)
    {
        try
        {
            if (await _formsInterface.AddNewObligatoryQuestionToForm(preferenceDto))
                return Ok();
            return BadRequest("Something went wrong while adding new preferene!");

        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete]
    [Route("delete_form")]
    public async Task<IActionResult> DeleteForm([FromBody] string formName)
    {
        try
        {
            if (await _formsInterface.DeleteForm(formName))
                return Ok();
            return BadRequest("Something went wrong while deleting a form!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete]
    [Route("delete_question")]
    public async Task<IActionResult> DeleteObligatoryQuestion([FromBody] DeleteQuestionDto deleteQuestionDto)
    {
        try
        {
            if (await _formsInterface.DeleteQuestion(deleteQuestionDto))
                return Ok();
            return BadRequest("Something went wrong while deleting a question!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    [Route("users_information")]
    public async Task<IActionResult> GetUsersInformation()
    {
        var res = await _adminInterface.GetInformationAboutUsers();
        switch (res.Item2)
        {
            case ErrorCodes.Ok:
                return Ok(res.Item1);
            case ErrorCodes.NotFound:
                return NotFound("Information about users not found");
        }
        return BadRequest("Something went wrong during obtaining information about users!   ");
    }

    [HttpPut]
    [Route("add_role_to_user")]
    public async Task<IActionResult> AddRoleToUser([FromBody] AddRoleToUserDto dto)
    {
        if (!AddRoleToUserDto.CheckIfRoleIsProper(dto.Role))
            return BadRequest("There is no role as providen in request!");
        var res = await _adminInterface.SetRoleToUser(dto);
        switch (res)
        {
            case ErrorCodes.NotFound:
                return NotFound("Data about user was not found!");
            case ErrorCodes.BadRequest:
                return BadRequest("Something went wrong while adding a role to user!");
            case ErrorCodes.Ok:
                return Ok();
        }
        return BadRequest("Something went wrong!");
    }

    [HttpPost]
    [Route("add_new_room")]
    public async Task<IActionResult> AddNewRoom([FromBody] List<RoomInDto> dtos)
    {
        var res = await _roomInterface.AddRoom(dtos);
        switch (res)
        {
            case ErrorCodes.Ok:
                return Ok();
            case ErrorCodes.BadRequest:
                return BadRequest("Something went wrong while adding new room information!");
        }
        return BadRequest("Something went wrong while adding new room information!");
    }

    [HttpDelete]
    [Route("delete_room")]
    public async Task<IActionResult> DeleteRoom([FromBody] RoomInDto dto)
    {
        var res = await _roomInterface.DeleteRoom(dto);
        switch (res)
        {
            case ErrorCodes.Ok:
                return Ok();
            case ErrorCodes.BadRequest:
                return BadRequest("Something went wrong while deleting a room!");
            case ErrorCodes.NotFound:
                return NotFound("There is no such a room in a database!");
        }
        return BadRequest("Something went wrong while deleting room!");
    }

    [HttpGet]
    [Route("get_all_rooms")]
    public async Task<IActionResult> GetRooms()
    {
        var res = await _roomInterface.GetRooms();
        switch (res.Item2)
        {
            case ErrorCodes.Ok:
                return Ok(res.Item1);
            case ErrorCodes.BadRequest:
                return BadRequest("Something went wrong in getting information about rooms!");
        }
        return BadRequest("Something went wrong in getting information about rooms!");
    }

    [HttpPut]
    [Route("set_status_to_room")]
    public async Task<IActionResult> SetStatusToRoom([FromBody] SetStatusToRoomDto dto)
    {
        var res = await _roomInterface.ChangeStatusForRoom(dto);
        switch (res)
        {
            case ErrorCodes.Ok:
                return Ok();
            case ErrorCodes.NotFound:
                return NotFound("There is no room with such address");
            case ErrorCodes.BadRequest:
                return BadRequest("Something went wrong while changing status to room!");
        }
        return BadRequest("Something went wrong while changing status to room!");
    }

    [HttpPut]
    [Route("add_user_to_room")]
    public async Task<IActionResult> AddUserToRoom([FromBody] UserRoomDto dto)
    {
        var res = await _roomInterface.ApplyUserToRoom(dto);
        switch (res)
        {
            case ErrorCodes.Ok:
                return Ok();
            case ErrorCodes.NotFound:
                return NotFound("There is no user or room provided in request!");
            case ErrorCodes.BadRequest:
                return BadRequest("Capacity of a room is not enough!");
        }
        return BadRequest("Something went wrong while adding user to room!");
    }

    [HttpPut]
    [Route("remove_user_from_room")]
    public async Task<IActionResult> RemoveUserFromRoom([FromBody] UserRoomDto dto)
    {
        var res = await _roomInterface.RemoveUserFromRoom(dto);
        switch (res)
        {
            case ErrorCodes.Ok:
                return Ok();
            case ErrorCodes.NotFound:
                return NotFound("There is no such user or such room!");
            case ErrorCodes.BadRequest:
                return BadRequest("Something went wrong while changing in database!");
        }
        return BadRequest("Something went wrong while removing user from a room!");
    }

    [HttpPost]
    [Route("add_test_proposal")]
    public async Task<IActionResult> AddTestProposal([FromBody] ProposalInDto dto)
    {
        var res = await _proposalInterface.AddTestProposal(dto);

        switch (res)
        {
            case ErrorCodes.Ok:
                return Ok();
            case ErrorCodes.NotFound:
                return NotFound("There is no such room in database!");
            case ErrorCodes.BadRequest:
                return BadRequest("Something went wrong while changing in database!");
        }
        return BadRequest("Something went wrong while removing user from a room!");
    }

    [HttpGet]
    [Route("get_all_proposals")]
    public async Task<IActionResult> GetAllProposals()
    {
        var res = await _proposalInterface.ReturnAllProposals();

        switch (res.Item2)
        {
            case ErrorCodes.Ok:
                return Ok(res.Item1);
            case ErrorCodes.NotFound:
                return NotFound("There are no any proposals in database");
            case ErrorCodes.BadRequest:
                return BadRequest("Something went wrong fetching data about proposals!");
        }
        return BadRequest("Something went wrong fetching data about proposals!");
    }

    [HttpPut]
    [Route("change_admin_status")]
    public async Task<IActionResult> ChangeAdminStatus([FromBody] AdminChangesStatusProposalDto dto)
    {
        var res = await _proposalInterface.AdminChangesStatusTheProposal(dto);

        switch (res)
        {
            case ErrorCodes.Ok:
                return Ok();
            case ErrorCodes.NotFound:
                return NotFound("There is no such a proposal!");
            case ErrorCodes.BadArgument:
                return BadRequest("Status of proposal isn't proper for changing it!");
            case ErrorCodes.BadRequest:
                return BadRequest("Something went wrong while changing data in database!");
        }
        return BadRequest("Something went wrong while changing data about proposal!");
    }

    [HttpGet]
    [Route("generate_proposals")]
    public async Task<IActionResult> GenerateProposals()
    {
        var res = await _judgeInterface.GenerateProposals();

        switch (res)
        {
            case JudgeError.StudentsAreNull:
                return NotFound("Something went wrong while fetching information about students!");
            case JudgeError.RoomsAreNull:
                return NotFound("Something went wrong while fetching information about rooms!");
            case JudgeError.DatabaseError:
                return BadRequest("Something went wrong while generating proposals for students");
            case JudgeError.Ok:
                return Ok();
        }
        return BadRequest("Something went wrong while generating proposals for students");
    }

    [HttpPost]
    [Route("answer_for_application")]
    public async Task<IActionResult> AnswerForApplication([FromBody] ApplicationAnswerInDto dto)
    {
        var errorCode = await _applicationInterface.AnswerToApplication(dto);
        switch (errorCode)
        {
            case ErrorCodes.Unauthorized:
                return Unauthorized();
            case ErrorCodes.NotFound:
                return NotFound("No such an application in database!");
            case ErrorCodes.BadRequest:
                return BadRequest("Something went wrong while fetching information!");
            case ErrorCodes.Ok:
                return Ok();
        }
        return BadRequest("Something went wrong while fetching information!");
    }

    [HttpGet]
    [Route("my_application_answers")]
    public async Task<IActionResult> GetApplicationAnswers()
    {
        var res = await _applicationInterface.ReturnAdminsAnswers();
        var errorCode = res.Item1;
        switch (errorCode)
        {
            case ErrorCodes.Unauthorized:
                return Unauthorized();
            case ErrorCodes.NotFound:
                return NotFound();
            case ErrorCodes.Ok:
                return Ok(res.Item2);
        }
        return BadRequest("Something went wrong while fetching information about Application Answers!");
    }

    [HttpGet]
    [Route("specific_application_answer")]
    public async Task<IActionResult> GetSpecificAnswerInformation([FromQuery] int applicationAnswerId)
    {
        var res = await _applicationInterface.ReturnInformationAboutSpecificAnswer(applicationAnswerId);
        var errorCode = res.Item1;
        switch (errorCode)
        {
            case ErrorCodes.Unauthorized:
                return Unauthorized();
            case ErrorCodes.NotFound:
                return NotFound();
            case ErrorCodes.Forbidden:
                return Forbid();
            case ErrorCodes.Ok:
                return Ok(res.Item2);
        }
        return BadRequest("Something went wrong while fetching information about specific Application Answer!");
    }

    [HttpGet]
    [Route("all_applications")]
    public async Task<IActionResult> GetAllApplications()
    {
        var res = await _applicationInterface.ReturnUsersApplications();
        var errorCode = res.Item1;
        switch (errorCode)
        {
            case ErrorCodes.Unauthorized:
                return Unauthorized();
            case ErrorCodes.NotFound:
                return NotFound("Some problems occured while fetching database information!");
            case ErrorCodes.Ok:
                return Ok(res.Item2);
        }
        return BadRequest("Error while fetching ingormation about users applications!");
    }

    [HttpGet]
    [Authorize]
    [Route("application_spicific_info")]
    public async Task<IActionResult> GetApplicationSpecificInfo([FromQuery] int applicationId)
    {
        var res = await _applicationInterface.ReturnInformationAboutSpecificApplication(applicationId);
        var errorCode = res.Item1;
        switch (errorCode)
        {
            case ErrorCodes.Unauthorized:
                return Unauthorized();
            case ErrorCodes.NotFound:
                return NotFound("There is no application with such id!");
            case ErrorCodes.Forbidden:
                return Forbid();
            case ErrorCodes.Ok:
                return Ok(res.Item2);
        }
        return BadRequest("Error while fetching ingormation about this application!");
    }
    
    [HttpGet]
    [Route("get_all_applications")]
    public async Task<IActionResult> GetAllApplicationsForAdmin()
    {
        var res = await _applicationInterface.GetAllApplications();
        var errorCode = res.Item1;
        switch (errorCode)
        {
            case ErrorCodes.Unauthorized:
                return Unauthorized();
            case ErrorCodes.Forbidden:
                return Forbid();
            case ErrorCodes.NotFound:
                return NotFound("No applications found in the database!");
            case ErrorCodes.Ok:
                return Ok(res.Item2);
        }
        return BadRequest("Error while fetching information about applications!");
    }
    
    [HttpPut]
    [Route("update_application_status")]
    public async Task<IActionResult> UpdateApplicationStatus([FromBody] UpdateApplicationStatusDto dto)
    {
        var errorCode = await _applicationInterface.UpdateApplicationStatus(dto);
        switch (errorCode)
        {
            case ErrorCodes.Unauthorized:
                return Unauthorized();
            case ErrorCodes.Forbidden:
                return Forbid();
            case ErrorCodes.NotFound:
                return NotFound("Application not found!");
            case ErrorCodes.BadArgument:
                return BadRequest("Invalid status value provided!");
            case ErrorCodes.BadRequest:
                return BadRequest("Error while updating application status!");
            case ErrorCodes.Ok:
                return Ok();
        }
        return BadRequest("Error while updating application status!");
    }
}
