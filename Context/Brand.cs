using System;
using System.Collections.Generic;

namespace magazine_music.Context;

public partial class Brand
{
    public int BrandId { get; set; }

    public string BrandName { get; set; } = null!;

    public string? BrandCountry { get; set; }

    public string? BrandDescription { get; set; }

    public virtual ICollection<Instrument> Instruments { get; set; } = new List<Instrument>();
}
