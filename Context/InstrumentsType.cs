using System;
using System.Collections.Generic;

namespace magazine_music.Context;

public partial class InstrumentsType
{
    public int TypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<Instrument> Instruments { get; set; } = new List<Instrument>();
}
