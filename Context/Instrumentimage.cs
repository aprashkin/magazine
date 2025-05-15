using System;
using System.Collections.Generic;

namespace magazine_music.Context;

public partial class Instrumentimage
{
    public int ImageId { get; set; }

    public int? InstrumentId { get; set; }

    public string ImagePath { get; set; } = null!;

    public virtual Instrument? Instrument { get; set; }
}
