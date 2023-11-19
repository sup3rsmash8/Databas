using Databasteknik.Contexts;
using Databasteknik.Entities;

namespace Databasteknik.Repositories;

public interface IAddressRepository : IRepo<AddressEntity> { }

public class AddressRepository : Repo<AddressEntity>, IAddressRepository
{
    public AddressRepository(DataContext context) : base(context) { }
}
