using SMS.Domain.Entities.Academic;
using SMS.Domain.Interfaces.Repositories.Academic;
using SMS.Infrastructure.Persistance.Context;
using SMS.Infrastructure.Repositories.Common;

namespace SMS.Infrastructure.Repositories.Academic
{
    public class AcademicYearRepository(AppDbContext dbContext) 
        : GenericRepository<AcademicYear>(dbContext), IAcademicYearRepository
    {
        
    }
}
