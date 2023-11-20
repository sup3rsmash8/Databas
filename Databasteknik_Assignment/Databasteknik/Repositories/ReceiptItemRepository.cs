using Databasteknik.Contexts;
using Databasteknik.Entities;

namespace Databasteknik.Repositories;

public interface IReceiptItemRepository : IRepo<ReceiptItemEntity> { }

public class ReceiptItemRepository : Repo<ReceiptItemEntity>, IReceiptItemRepository
{
    public ReceiptItemRepository(DataContext context) : base(context) { }
}
