namespace EmployeeManagementSystem.BusinessLogic.Common
{
    public class ApiCommonResponse<T>
    {
        public bool Success { get; set; }

        public string Message { get; set; }=string.Empty;

        public T? Data { get; set; }

        public List<string>? ValidationErrors { get; set; }

        public int StatusCode { get; set; }
    }
}
