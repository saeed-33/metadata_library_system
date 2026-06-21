using LibrarySystem.Application.Commands.Media;
using LibrarySystem.Application.DTOs.Media;
using LibrarySystem.Application.DTOs.Values;
using LibrarySystem.Application.Queries.Media;
using LibrarySystem.Domain.common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
namespace LibrarySystem.API.Controllers;

[Authorize]
[ApiController]
[Route("api/media")]
public class MediaController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IWebHostEnvironment _environment;
    public MediaController(IMediator mediator, IWebHostEnvironment environment)
    {
        _mediator = mediator;
        _environment = environment;
    }

    [HttpGet("by-item/{itemId:int}")]
    public async Task<IActionResult> GetByItem(int itemId)
    {
        var mediaList = await _mediator.Send(new GetMediaByItemQuery(itemId));
        return Ok(mediaList);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var media = await _mediator.Send(new GetMediaByIdQuery(id));
        return media == null ? NotFound() : Ok(media);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateMediaCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateMediaCommand command)
    {
        if (id != command.Id)
            return BadRequest("URL id does not match command id.");

        var updated = await _mediator.Send(command);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _mediator.Send(new DeleteMediaCommand(id));
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("upload-with-metadata")]
    public async Task<IActionResult> UploadWithMetadata([FromForm] UploadMediaRequestDto request)
    {
        if (request.File == null || request.File.Length == 0)
            return BadRequest("No file selected for upload.");

        var webRootPath = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
        var uploadsFolder = Path.Combine(webRootPath, "uploads");

        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = $"{Guid.NewGuid()}_{request.File.FileName}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await request.File.CopyToAsync(stream);
        }

        var storagePath = $"/uploads/{uniqueFileName}";

        try
        {
            var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var mediaValues = string.IsNullOrEmpty(request.ValuesJson)
                ? new List<CreateValueRequest>()
                : JsonSerializer.Deserialize<List<CreateValueRequest>>(request.ValuesJson, jsonOptions)
                    ?? new List<CreateValueRequest>();

            var command = new CreateMediaCommand(
                request.ItemId,
                storagePath,
                request.File.FileName,
                mediaValues
            );

            var id = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetById), new { id }, new { id, storagePath });
        }
        catch (Exception)
        {
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            return StatusCode(500, "An error occurred while saving data; file upload was rolled back.");
        }
    }
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var mediaList = await _mediator.Send(new GetAllMediaQuery());
        return Ok(mediaList);
    }

    [HttpGet("WithDeleted")]
    [Authorize(Roles = SystemRoles.Admin)]

    public async Task<IActionResult> GetAllWithDeleted()
    {
        var mediaList = await _mediator.Send(new GetAllMediaWithDeletedQuery());
        return Ok(mediaList);
    }

    [HttpPut("Undelet/{id:int}")]
    [Authorize(Roles = SystemRoles.Admin)]
    public async Task<IActionResult> Undelet(int id, UndeleteMediaCommand command)
    {
        if (id != command.Id)
            return BadRequest("URL id does not match command id.");

        var updated = await _mediator.Send(command);
        return updated ? NoContent() : NotFound();
    }

}
