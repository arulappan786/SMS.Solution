namespace SMS.Domain.Interfaces.Repositories.Academic
{
    public interface ISubjectRepository
    {
        Task<bool> ExistsAsync(string name, string code, CancellationToken cancellationToken);
    }
}
