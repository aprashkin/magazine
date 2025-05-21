using System;
using System.Collections.Generic;

namespace magazine_music.Context;

public partial class OrderItem
{
    public int OrderItemId { get; set; }

    public int OrderId { get; set; }

    public int InstrumentId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public virtual Instrument Instrument { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
