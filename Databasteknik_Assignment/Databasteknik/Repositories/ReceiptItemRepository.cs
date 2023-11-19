using Databasteknik.Contexts;
using Databasteknik.Entities;

namespace Databasteknik.Repositories;

public class ReceiptItemRepository : Repo<ReceiptItemEntity>
{
    public ReceiptItemRepository(DataContext context) : base(context) { }
}
