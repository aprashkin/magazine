using System;
using System.Collections.Generic;

namespace magazine_music.Context;

public partial class Order
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public int InstrumentId { get; set; }

    public DateTime? OrderDate { get; set; }

    public int StatusId { get; set; }

    public decimal TotalAmount { get; set; }

    public string Address { get; set; } = null!;

    public virtual Instrument Instrument { get; set; } = null!;

    public virtual Status Status { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
