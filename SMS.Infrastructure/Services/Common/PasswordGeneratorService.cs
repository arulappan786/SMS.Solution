using SMS.Application.Services.Interfaces.Common;
using SMS.Infrastructure.Services.Common.Utilities;

namespace SMS.Infrastructure.Services.Common
{
    public class PasswordGeneratorService : IPasswordGeneratorService
    {
        public string GenerateSecurePassword()
        {
            return PasswordGenerator.GenerateSecurePassword();
        }
    }
}
