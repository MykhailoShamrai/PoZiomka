using System.ComponentModel;
using backend.Dto;
using backend.Interfaces;
using backend.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[Route("api/[controller]")]
[Authorize("Admin")]
public class AdminController: ControllerBase
{
    private readonly IFormsInterface _formsInterface;
    private readonly IAdminInterface _adminInterface;
    private readonly IRoomInterface _roomInterface;

    public AdminController(IFormsInterface formsInterface,
                            IAdminInterface adminInterface,
                            IRoomInterface roomInterface)
    {
        _formsInterface = formsInterface;
        _adminInterface = adminInterface;
        _roomInterface = roomInterface;
    }

    [HttpPost]
    [Route("add_new_form")]
    public async Task<IActionResult> AddNewForm([FromBody] FormDto formDto)
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
    public async Task<IActionResult> DeleteForm([FromBody] string nameOfForm)
    {
        try
        {
            if (await _formsInterface.DeleteForm(nameOfForm))
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
    public async Task<IActionResult> DeleteObligatoryQuestion([FromBody] string nameOfQuestion)
    {
        try
        {
            if (await _formsInterface.DeleteQuestion(nameOfQuestion))
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
        switch(res.Item2)
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
    public async Task<IActionResult> AddNewRoom([FromBody] RoomInDto dto)
    {
        var res = await _roomInterface.AddRoom(dto);
        switch (res)
        {
            case ErrorCodes.Ok:
                return Ok();
            case ErrorCodes.BadRequest:
                return BadRequest("Something went wrong while adding new room information!");
        }
        return BadRequest("Something went wrong while adding new room information!");
    }
}
