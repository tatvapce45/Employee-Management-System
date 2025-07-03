using Xunit;
using Moq;
using Microsoft.Extensions.Configuration;
using EmployeeManagementSystem.BusinessLogic.Helpers;

namespace EmployeeManagementSystem.Tests.ServiceTests
{
    public class HashHelperTests
    {
        private const string ValidKey = "1234567890123456";
        private const string ValidIV = "6543210987654321";

        private static Mock<IConfiguration> GetMockConfig(string? key = ValidKey, string? iv = ValidIV)
        {
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c["HashGenerator:HashKey"]).Returns(key);
            mockConfig.Setup(c => c["HashGenerator:HashIV"]).Returns(iv);
            return mockConfig;
        }

        [Fact]
        public void Constructor_ValidKeyAndIV_ShouldNotThrow()
        {
            var config = GetMockConfig();
            var helper = new HashHelper(config.Object);
            Assert.NotNull(helper);
        }

        [Theory]
        [InlineData(null, ValidIV)]
        [InlineData(ValidKey, null)]
        [InlineData(null, null)]
        [InlineData("short", ValidIV)]
        [InlineData(ValidKey, "short")]
        public void Constructor_InvalidKeyOrIV_Throws(string? key, string? iv)
        {
            var config = GetMockConfig(key, iv);
            Assert.Throws<ArgumentException>(() => new HashHelper(config.Object));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Encrypt_NullOrEmptyInput_ReturnsEmpty(string? input)
        {
            var helper = new HashHelper(GetMockConfig().Object);
            var result = helper.Encrypt(input);
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void Encrypt_ValidInput_ReturnsEncryptedText()
        {
            var helper = new HashHelper(GetMockConfig().Object);
            var plainText = "Hello World";
            var encrypted = helper.Encrypt(plainText);

            Assert.False(string.IsNullOrEmpty(encrypted));
            Assert.NotEqual(plainText, encrypted);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Decrypt_NullOrEmptyInput_ReturnsEmpty(string? input)
        {
            var helper = new HashHelper(GetMockConfig().Object);
            var result = helper.Decrypt(input);
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void Decrypt_ValidEncryptedInput_ReturnsOriginalText()
        {
            var helper = new HashHelper(GetMockConfig().Object);
            var plainText = "Test123";
            var encrypted = helper.Encrypt(plainText);
            var decrypted = helper.Decrypt(encrypted);
            Assert.Equal(plainText, decrypted);
        }

        [Theory]
        [InlineData("NotBase64@@@")]
        [InlineData("dGVzdA==")]
        public void Decrypt_InvalidEncryptedInput_ReturnsEmpty(string encryptedInput)
        {
            var helper = new HashHelper(GetMockConfig().Object);
            var result = helper.Decrypt(encryptedInput);
            Assert.Equal(string.Empty, result);
        }
    }
}
