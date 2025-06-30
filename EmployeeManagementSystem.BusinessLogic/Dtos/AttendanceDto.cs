namespace EmployeeManagementSystem.BusinessLogic.Dtos
{
    public class AttendanceDto
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public DateOnly Date { get; set; }

        public string? Status { get; set; }

        public DateTime? CheckInTime { get; set; }

        public DateTime? CheckOutTime { get; set; }

        public string? Remarks { get; set; }
    }

    public class AttendanceListDto
    {
        public List<AttendanceDto> AttendanceDtos{get;set;}=[];

        public int Month{get;set;}

        public int Year{get;set;}
    }

    public class MarkAttendanceDto
    {
        public int EmployeeId { get; set; }

        public string Status{get;set;}=string.Empty;
    }

    public class UpdateAttendanceDto
    {
        public int Id { get; set; }

        public DateTime? CheckInTime { get; set; }

        public string Status { get; set; }=string.Empty;

        public DateTime? CheckOutTime { get;set;}

        public string? Remarks { get; set;}
    }

    public class AttendanceListRequestDto
    {
        public int Month{get;set;}

        public int Year{get;set;}

        public int EmployeeId{get;set;}
    }
}