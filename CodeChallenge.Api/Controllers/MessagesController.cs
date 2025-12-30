using CodeChallenge.Api.Logic;
using CodeChallenge.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace CodeChallenge.Api.Controllers;

[ApiController]
[Route("api/v1/organizations/{organizationId}/messages")]
public class MessagesController : ControllerBase
{
    private readonly IMessageLogic _logic;
    private readonly ILogger<MessagesController> _logger;

    public MessagesController(IMessageLogic logic, ILogger<MessagesController> logger)
    {
        _logic = logic;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Message>>> GetAll(Guid organizationId)
    {
        var result = await _logic.GetAllMessagesAsync(organizationId);

        if (result is Created<IEnumerable<Message>> created)
        {
            return Ok(created.Value);
        }

        if (result is ValidationError validationError)
        {
            return BadRequest(validationError.Errors);
        }

        return StatusCode(500);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Message>> GetById(Guid organizationId, Guid id)
    {
        var result = await _logic.GetMessageAsync(organizationId, id);

        if (result is Created<Message> created)
        {
            return Ok(created.Value);
        }

        if (result is NotFound notFound)
        {
            return NotFound(notFound.Message);
        }

        if (result is ValidationError validationError)
        {
            return BadRequest(validationError.Errors);
        }

        return StatusCode(500);
    }

    [HttpPost]
    public async Task<ActionResult<Message>> Create(
        Guid organizationId,
        [FromBody] CreateMessageRequest request)
    {
        var result = await _logic.CreateMessageAsync(organizationId, request);

        if (result is Created<Message> created)
        {
            return CreatedAtAction(
                nameof(GetById),
                new { organizationId, id = created.Value.Id },
                created.Value
            );
        }

        if (result is ValidationError validationError)
        {
            return BadRequest(validationError.Errors);
        }

        if (result is Conflict conflict)
        {
            return Conflict(conflict.Message);
        }

        return StatusCode(500);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(
        Guid organizationId,
        Guid id,
        [FromBody] UpdateMessageRequest request)
    {
        var result = await _logic.UpdateMessageAsync(organizationId, id, request);

        if (result is Updated)
        {
            return NoContent();
        }

        if (result is NotFound notFound)
        {
            return NotFound(notFound.Message);
        }

        if (result is ValidationError validationError)
        {
            return BadRequest(validationError.Errors);
        }

        return StatusCode(500);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid organizationId, Guid id)
    {
        var result = await _logic.DeleteMessageAsync(organizationId, id);

        if (result is Deleted)
        {
            return NoContent();
        }

        if (result is NotFound notFound)
        {
            return NotFound(notFound.Message);
        }

        if (result is ValidationError validationError)
        {
            return BadRequest(validationError.Errors);
        }

        return StatusCode(500);
    }
}
