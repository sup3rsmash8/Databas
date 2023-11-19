using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Databasteknik.Entities;

public class ShoppingCartEntity
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public ICollection<ShoppingCartItemEntity> Items { get; set; } = new List<ShoppingCartItemEntity>();
}

