﻿using System;
using System.Collections.Generic;

namespace magazine_music.Context;

public partial class Instrument
{
    public int InstrumentId { get; set; }

    public string InstrumentName { get; set; } = null!;

    public int? TypeId { get; set; }

    public int? BrandId { get; set; }

    public decimal? InstrumentPrice { get; set; }

    public int? InstrumentQuantity { get; set; }

    public string? InstrumentDescription { get; set; }

    public virtual Brand? Brand { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual InstrumentsType? Type { get; set; }
}
