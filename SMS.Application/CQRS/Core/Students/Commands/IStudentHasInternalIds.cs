namespace SMS.Application.CQRS.Core.Students.Commands
{
    // Marker Interface to identify commands that need internal code
    public interface IStudentHasInternalIds
    {
        // Make the property settable only within the assembly or derived classes
        Guid UserId { get; set; }
        string? StudentCode { get; set; }
    }
}
