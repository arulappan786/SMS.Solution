using Microsoft.EntityFrameworkCore;
using SMS.Domain.Entities.Academic;
using SMS.Domain.Interfaces.Repositories.Academic;
using SMS.Infrastructure.Persistance.Context;
using SMS.Infrastructure.Repositories.Common;

namespace SMS.Infrastructure.Repositories.Academic
{
    public class ClassesRepository(AppDbContext context) 
        : GenericRepository<Classes>(context), IClassesRepository
    {
        public async Task<bool> ExistsAsync(string name, CancellationToken cancellationToken)
        {
            string lowerName = name.ToLower();

            var isDuplicate = await context.Classes
                .AsNoTracking()
                .AnyAsync(c => c.Name.ToLower() == lowerName, cancellationToken);

            return isDuplicate;
        }
    }
}
