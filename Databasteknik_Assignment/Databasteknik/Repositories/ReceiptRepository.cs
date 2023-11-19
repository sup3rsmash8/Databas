using Databasteknik.Contexts;
using Databasteknik.Entities;

namespace Databasteknik.Repositories;

public class ReceiptRepository : Repo<ReceiptEntity>
{
    public ReceiptRepository(DataContext context) : base(context) { }
}
