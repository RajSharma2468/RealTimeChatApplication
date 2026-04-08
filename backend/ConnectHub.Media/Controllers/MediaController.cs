using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ConnectHub.Media.Services;
using ConnectHub.Media.DTOs;

namespace ConnectHub.Media.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MediaController : ControllerBase
    {
        private readonly IMediaService _mediaService;
        
        public MediaController(IMediaService mediaService)
        {
            _mediaService = mediaService;
        }
        
        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }
        
        // POST: api/media/upload
        // Note: For Swagger, use form-data with key "file"
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadFile(IFormFile file, int? messageId = null, int? roomId = null)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { success = false, message = "No file provided" });
                
                var userId = GetCurrentUserId();
                var result = await _mediaService.UploadFileAsync(userId, file, messageId, roomId);
                return Ok(new { success = true, data = result, message = "File uploaded successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        
        // GET: api/media/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFile(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var file = await _mediaService.GetFileByIdAsync(id, userId);
                return Ok(new { success = true, data = file });
            }
            catch (Exception ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
        }
        
        // GET: api/media/message/{messageId}
        [HttpGet("message/{messageId}")]
        public async Task<IActionResult> GetFilesByMessage(int messageId)
        {
            var userId = GetCurrentUserId();
            var files = await _mediaService.GetFilesByMessageAsync(messageId, userId);
            return Ok(new { success = true, data = files });
        }
        
        // GET: api/media/room/{roomId}
        [HttpGet("room/{roomId}")]
        public async Task<IActionResult> GetFilesByRoom(int roomId)
        {
            var userId = GetCurrentUserId();
            var files = await _mediaService.GetFilesByRoomAsync(roomId, userId);
            return Ok(new { success = true, data = files });
        }
        
        // GET: api/media/my-files
        [HttpGet("my-files")]
        public async Task<IActionResult> GetMyFiles()
        {
            var userId = GetCurrentUserId();
            var files = await _mediaService.GetMyFilesAsync(userId);
            return Ok(new { success = true, data = files });
        }
        
        // DELETE: api/media/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _mediaService.DeleteFileAsync(id, userId);
                return Ok(new { success = true, message = "File deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        
        // GET: api/media/download/{id}
        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadFile(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var file = await _mediaService.GetFileByIdAsync(id, userId);
                
                var physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", file.FileUrl.TrimStart('/'));
                if (!System.IO.File.Exists(physicalPath))
                    return NotFound(new { success = false, message = "File not found on server" });
                
                var memory = new MemoryStream();
                using (var stream = new FileStream(physicalPath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                
                return File(memory, file.ContentType, file.FileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}