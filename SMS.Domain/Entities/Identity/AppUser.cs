using Microsoft.AspNetCore.Identity;
using SMS.Domain.Entities.Core;

namespace SMS.Domain.Entities.Identity
{
    public class AppUser : IdentityUser
    {
        //public AppRole AppRole { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? LastLogin { get; set; }

        public string? DisplayName { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime RefreshTokenExpiry { get; set; }

        public int? StudentProfileId { get; set; }
        public int? TeacherProfileId { get; set; }
        public int? ParentProfileId { get; set; }

        public Student? StudentProfile { get; set; }
        public Teacher? TeacherProfile { get; set; }
        public Parent? ParentProfile { get; set; }
    }
}