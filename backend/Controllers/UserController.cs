using System.Security.Claims;
using backend.Data;
using backend.Dto;
using backend.Interfaces;
using backend.Models.User;
using backend.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserInterface _userRepository;
    private readonly IProposalInterface _proposalInterface;
    private readonly IApplicationInterface _applicationService;

    public UserController(IUserInterface userInterface, IProposalInterface proposalInterface, IApplicationInterface applicationInterface)
    {
        _userRepository = userInterface;
        _proposalInterface = proposalInterface;
        _applicationService = applicationInterface;
    }

    [HttpPost]
    [Authorize]
    [Route("preferences")]
    public async Task<IActionResult> ChangeMyPreferences([FromBody] UserPreferences userPreferences)
    {
        var code = await _userRepository.ChangeUserPreferences(userPreferences);
        switch (code)
        {
            case ErrorCodes.NotFound:
                return NotFound("User not found.");
            case ErrorCodes.CannotRetrieveUserFromCookie:
                return Unauthorized("Cookie retrieval failed.");
            case ErrorCodes.UpdateUserDbFailed:
                return BadRequest("Db update failed.");
            case ErrorCodes.Ok:
                return Ok();
            default:
                throw new KeyNotFoundException();
        }
    }

    [HttpGet]
    [Authorize]
    [Route("profile")]
    public async Task<IActionResult> DisplayProfile()
    {
        var (code, profile) = await _userRepository.DisplayUserProfile();
        switch (code)
        {
            case ErrorCodes.Unauthorized:
                return Unauthorized();
            case ErrorCodes.NotFound:
                return NotFound("User not found.");
            case ErrorCodes.Ok:
                return Ok(profile);
            default:
                throw new KeyNotFoundException();
        }
    }

    [HttpPut]
    [Authorize]
    [Route("profile")]
    public async Task<IActionResult> ChangeProfile([FromBody] UpdateUserDto user)
    {
        var code = await _userRepository.ChangeUserProfile(user);
        switch (code)
        {
            case ErrorCodes.Forbidden:
                return NotFound("Forbidden");
            case ErrorCodes.Ok:
                return Ok();
            default:
                throw new KeyNotFoundException();
        }
    }

    [HttpGet]
    [Authorize]
    [Route("forms")]
    public async Task<IActionResult> GetForms()
    {
        var (code, forms) = await _userRepository.GetUserForms();
        switch (code)
        {
            case ErrorCodes.Unauthorized:
                return Unauthorized();
            case ErrorCodes.NotFound:
                return NotFound("Forms not found.");
            case ErrorCodes.Ok:
                return Ok(forms);
            default:
                throw new KeyNotFoundException();
        }
    }

    [HttpPost]
    [Authorize]
    [Route("submit-answer")]
    public async Task<IActionResult> SubmitAnswer([FromBody] AnswerDto answerDto)
    {
        var errorCode = await _userRepository.SubmitAnswerForForms(answerDto);

        switch (errorCode)
        {
            case ErrorCodes.Unauthorized:
                return Unauthorized();
            case ErrorCodes.NotFound:
                return NotFound("Form not found!");
            case ErrorCodes.Ok:
                return Ok();
            case ErrorCodes.BadRequest:
                return BadRequest("Option doesn't correspond to question from form!");
            default:
                throw new KeyNotFoundException();
        }
    }

    [HttpGet]
    [Authorize]
    [Route("get_my_proposals")]
    public async Task<IActionResult> GetMyProposals()
    {
        var (list, errorCode) = await _proposalInterface.ReturnUsersProposals();
        switch (errorCode)
        {
            case ErrorCodes.Unauthorized:
                return Unauthorized();
            case ErrorCodes.BadRequest:
                return BadRequest("Something went wrong while fetching information about proposals!");
            case ErrorCodes.Ok:
                return Ok(list);
        }
        return BadRequest("Something went wrong while fethcing information about proposals!");
    }

    [HttpPut]
    [Authorize]
    [Route("answer_prop")]
    public async Task<IActionResult> AnswerProposal([FromBody] UserChangesStatusProposalDto dto)
    {
        var errorCode = await _proposalInterface.UserAnswersTheProposal(dto);
        switch (errorCode)
        {
            case ErrorCodes.NotFound:
                return NotFound("Something went wrong in fetching proposal information!");
            case ErrorCodes.Unauthorized:
                return Unauthorized();
            case ErrorCodes.BadArgument:
                return BadRequest("it is not Your proposal!");
            case ErrorCodes.Ok:
                return Ok();
            case ErrorCodes.BadRequest:
                return BadRequest("Something went wrong while changing data!");
        }
        return BadRequest("Something went wrong while changing data!");
    }

    [HttpGet]
    [Authorize]
    [Route("get_my_communications")]
    public async Task<IActionResult> GetMyCommunications()
    {
        var (errorCode, list) = await _userRepository.GetCurrentUserCommunications();
        switch (errorCode)
        {
            case ErrorCodes.Unauthorized:
                return Unauthorized();
            case ErrorCodes.Ok:
                return Ok(list);
        }
        return BadRequest("Something went wrong while fethcing information about proposals!");
    }

    [HttpPost]
    [Authorize]
    [Route("send_application")]
    public async Task<IActionResult> SendApplication([FromBody] ApplicationInDto dto)
    {
        var errorCode = await _applicationService.SendAnApplication(dto);
        switch (errorCode)
        {
            case ErrorCodes.Unauthorized:
                return Unauthorized();
            case ErrorCodes.BadRequest:
                return BadRequest("Something went wrong while adding information about application!");
            case ErrorCodes.Ok:
                return Ok();
        }
        return BadRequest("Something went wrong while adding information about application!");
    }

    [HttpGet]
    [Authorize]
    [Route("my_applications")]
    public async Task<IActionResult> GetApplications()
    {
        var res = await _applicationService.ReturnUsersApplications();
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
        var res = await _applicationService.ReturnInformationAboutSpecificApplication(applicationId);
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
        return BadRequest("Error while fetching information about this application!");
    }

    [HttpGet]
    [Authorize]
    [Route("answer_for_application")]
    public async Task<IActionResult> GetApplicationAnswer([FromQuery] int applicationId)
    {
        var res = await _applicationService.ReturnAnswerForSpecificApplication(applicationId);
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
        return BadRequest("Error while fetching information about this asnwer!");
    }

}
