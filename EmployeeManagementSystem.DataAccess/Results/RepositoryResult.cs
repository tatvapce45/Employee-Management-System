namespace EmployeeManagementSystem.DataAccess.Results
{
    public class RepositoryResult<T>
    {
        public bool Success { get; set; }

        public string ErrorMessage { get; set; }=string.Empty;

        public T? Data { get; set; }
    }
}