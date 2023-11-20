using Databasteknik.Contexts;
using Databasteknik.Entities;

namespace Databasteknik.Repositories;

public interface IReceiptRepository : IRepo<ReceiptEntity> { }

public class ReceiptRepository : Repo<ReceiptEntity>, IReceiptRepository
{
    public ReceiptRepository(DataContext context) : base(context) { }
}
