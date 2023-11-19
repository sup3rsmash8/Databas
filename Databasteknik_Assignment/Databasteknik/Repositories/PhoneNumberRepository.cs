using Databasteknik.Contexts;
using Databasteknik.Entities;

namespace Databasteknik.Repositories;

public interface IPhoneNumberRepository : IRepo<PhoneNumberEntity> { }

public class PhoneNumberRepository : Repo<PhoneNumberEntity>, IPhoneNumberRepository
{
    public PhoneNumberRepository(DataContext context) : base(context) { }
}
