using Microsoft.AspNetCore.Identity;
using SMS.Domain.Entities.Core;

namespace SMS.Domain.Entities.Identity
{
    public class AppUser : IdentityUser
    {
        public bool IsActive { get; set; } = true;
        public DateTime? LastLogin { get; set; }

        public string? DisplayName { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime RefreshTokenExpiry { get; set; }

        public Guid? StudentProfileId { get; set; }
        public Guid? TeacherProfileId { get; set; }
        public Guid? ParentProfileId { get; set; }

        public Student? StudentProfile { get; set; }
        public Teacher? TeacherProfile { get; set; }
        public Parent? ParentProfile { get; set; }
    }
}