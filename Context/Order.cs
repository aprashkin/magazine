using System;
using System.Collections.Generic;

namespace magazine_music.Context;

public partial class Order
{
    public int OrderId { get; set; }

    public int? UserId { get; set; }

    public int? InstrumentId { get; set; }

    public DateOnly OrderDate { get; set; }

    public int? StatusId { get; set; }

    public decimal? TotalAmount { get; set; }

    public string? Address { get; set; }

    public virtual Instrument? Instrument { get; set; }

    public virtual Status? Status { get; set; }

    public virtual User? User { get; set; }
}
