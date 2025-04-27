using backend.Dto;
using backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace backend.Controllers;

[Route("api/[controller]")]
[Authorize("Admin")]
public class AdminController: ControllerBase
{
    private readonly IFormsInterface _formsInterface;

    public AdminController(IFormsInterface formsInterface)
    {
        _formsInterface = formsInterface;
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
            // Is it okay to send it in such form?
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
}
