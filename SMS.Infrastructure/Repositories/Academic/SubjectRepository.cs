using Microsoft.EntityFrameworkCore;
using SMS.Domain.Entities.Academic;
using SMS.Domain.Interfaces.Repositories.Academic;
using SMS.Infrastructure.Persistance.Context;
using SMS.Infrastructure.Repositories.Common;

namespace SMS.Infrastructure.Repositories.Academic
{
    public class SubjectRepository(AppDbContext context) : GenericRepository<Subject>(context), ISubjectRepository
    {
        public Task<bool> ExistsAsync(string name, string code, CancellationToken cancellationToken)
        {
            string normalizedName = name.Trim().ToLower();
            string normalizedCode = code.Trim().ToLower();

            var isDuplicate = context.Subjects
                .AsNoTracking()
                .AnyAsync(s =>
                    s.Name.ToLower() == normalizedName ||
                    s.Code.ToLower() == normalizedCode,
                    cancellationToken);
            
            return isDuplicate;
        }
    }
}
