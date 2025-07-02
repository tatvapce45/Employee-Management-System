namespace EmployeeManagementSystem.BusinessLogic.Services.Interfaces
{
    public interface IHashHelper
    {
        string Encrypt(string? plainText);

        string Decrypt(string? cipherText);
    }
}