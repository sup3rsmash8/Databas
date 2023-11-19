using Databasteknik.Contexts;
using Databasteknik.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Databasteknik.Repositories;

public interface ICompanyRepository : IRepo<CompanyEntity> { }

public class CompanyRepository : Repo<CompanyEntity>, ICompanyRepository
{
    public CompanyRepository(DataContext context) : base(context)
    {
    }

    public override async Task<CompanyEntity> GetAsync(Expression<Func<CompanyEntity, bool>> expression)
    {
        return await Context.Companies
            .Include(x => x.Address)
            .Include(x => x.ContactPhoneNumber)
            .FirstOrDefaultAsync(expression) ?? null!;
    }

    public override async Task<IEnumerable<CompanyEntity>> GetAllAsync()
    {
        return await Context.Companies
            .Include(x => x.Address)
            .Include(x => x.ContactPhoneNumber)
            .ToListAsync();
    }
}
