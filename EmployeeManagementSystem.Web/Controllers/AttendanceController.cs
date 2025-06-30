using EmployeeManagementSystem.BusinessLogic.Common;
using EmployeeManagementSystem.BusinessLogic.Dtos;
using EmployeeManagementSystem.BusinessLogic.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController(IAttendanceService attendanceService) : ControllerBase
    {
        private readonly IAttendanceService _attendanceService = attendanceService;

        [HttpPost("MarkAttendance")]
        [ProducesResponseType(typeof(ApiCommonResponse<string>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiCommonResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiCommonResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> MarkAttendance(MarkAttendanceDto markAttendanceDto)
        {
            var result = await _attendanceService.MarkAttendance(markAttendanceDto);
            var response = new ApiCommonResponse<string>
            {
                Success = result.Success,
                StatusCode = result.StatusCode,
                Message = result.Message!,
                Data = result.Success ? "" : null,
                ValidationErrors = result.ValidationErrors
            };
            return StatusCode(response.StatusCode, response);
        }

        [HttpPatch("UpdateAttendance")]
        [ProducesResponseType(typeof(ApiCommonResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiCommonResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiCommonResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAttendance(UpdateAttendanceDto updateAttendanceDto)
        {
            var result = await _attendanceService.UpdateAttendance(updateAttendanceDto);
            var response = new ApiCommonResponse<string>
            {
                Success = result.Success,
                StatusCode = result.StatusCode,
                Message = result.Message!,
                Data = result.Success ? "" : null,
                ValidationErrors = result.ValidationErrors
            };
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetAttendanceList")]
        [ProducesResponseType(typeof(ApiCommonResponse<AttendanceListDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiCommonResponse<AttendanceListDto>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAttendanceList([FromBody]AttendanceListRequestDto attendanceListRequestDto)
        {
            var result = await _attendanceService.GetAttendanceList(attendanceListRequestDto);
            var response = new ApiCommonResponse<AttendanceListDto>
            {
                Success = result.Success,
                StatusCode = result.StatusCode,
                Message = result.Message!,
                Data = result.Success ? result.Data : null,
                ValidationErrors = result.ValidationErrors
            };
            return StatusCode(response.StatusCode, response);
        }
    }
}