using CloudinaryDotNet;
using EmployeeManagementSystem.BusinessLogic.Common;
using EmployeeManagementSystem.BusinessLogic.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CloudinaryController : ControllerBase
    {
        private readonly Cloudinary _cloudinary;
        private readonly IConfiguration _config;

        public CloudinaryController(IConfiguration config)
        {
            _config = config;

            var account = new Account(
                _config["CloudinarySettings:CloudName"],
                _config["CloudinarySettings:ApiKey"],
                _config["CloudinarySettings:ApiSecret"]
            );

            _cloudinary = new Cloudinary(account);
        }

        [HttpGet("signature")]
        public IActionResult GetUploadSignature()
        {
            try
            {
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                var parameters = new SortedDictionary<string, object>
        {
            { "timestamp", timestamp },
            { "folder", "secure_uploads" }
        };

                var signature = _cloudinary.Api.SignParameters(parameters);

                CloudinaryResponse responseDto = new CloudinaryResponse
                {
                    Timestamp = timestamp,
                    Signature = signature,
                    ApiKey = _config["CloudinarySettings:ApiKey"]!,
                    CloudName = _config["CloudinarySettings:CloudName"]!,
                    Folder = "secure_uploads"
                };

                var response = new ApiCommonResponse<CloudinaryResponse>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Upload signature generated successfully.",
                    Data = responseDto
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiCommonResponse<CloudinaryResponse>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Failed to generate upload signature.",
                    ValidationErrors = [ex.Message]
                });
            }
        }
    }
}