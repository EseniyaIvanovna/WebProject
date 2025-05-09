using Application.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttachmentsController(IAttachmentService attachmentService)
    : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file, [FromQuery] string category = "attachments")
    {
        if (file.Length == 0)
            return BadRequest("File is required");

        var attachment = await attachmentService.UploadAsync(file, category);

        return Ok(attachment);
    }

    [HttpGet("{id}/download")]
    public async Task<IActionResult> Download(int id)
    {
        var file = await attachmentService.GetMetadataAsync(id);
        if (file == null)
            return NotFound();

        var bytes = await attachmentService.GetFileContentAsync(id);
        return File(bytes, file.ContentType, file.FileName);
    }

    [HttpGet("{id}/meta")]
    public async Task<IActionResult> GetMetadata(int id)
    {
        var attachment = await attachmentService.GetMetadataAsync(id);
        if (attachment == null)
            return NotFound();

        return Ok(attachment);
    }

    [HttpGet("{id}/link")]
    public async Task<IActionResult> GetLink(int id)
    {
         var link = await attachmentService.GetPublicLinkAsync(id);
            return Ok(new { url = link });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await attachmentService.DeleteAsync(id);
        return Ok(new { message = "Attachment deleted" });
    }
}