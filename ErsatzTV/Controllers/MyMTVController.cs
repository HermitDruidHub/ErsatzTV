using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace ErsatzTV.Controllers
{
    [ApiController]
    [Route("api/mymtv")]
    public class MyMTVController : ControllerBase
    {
        private readonly string _channelConfigPath = "/config/channels";

        public class SyncRequest
        {
            public string ChannelName { get; set; }
            public string PlaylistPath { get; set; }
        }

        [HttpPost("sync")]
        public async Task<IActionResult> SyncChannel([FromBody] SyncRequest request)
        {
            if (string.IsNullOrEmpty(request.ChannelName) || string.IsNullOrEmpty(request.PlaylistPath))
            {
                return BadRequest(new { error = "ChannelName and PlaylistPath are required." });
            }

            string safeName = request.ChannelName.Replace(" ", "_").ToLowerInvariant();
            string channelFile = Path.Combine(_channelConfigPath, $"{safeName}.json");

            var channelData = new
            {
                name = request.ChannelName,
                playlist = request.PlaylistPath,
                ffmpegProfile = "copy" // Placeholder for now
            };

            Directory.CreateDirectory(_channelConfigPath);

            await System.IO.File.WriteAllTextAsync(channelFile, JsonSerializer.Serialize(channelData, new JsonSerializerOptions
            {
                WriteIndented = true
            }));

            return Ok(new { message = $"Channel '{request.ChannelName}' synced." });
        }
    }
}
