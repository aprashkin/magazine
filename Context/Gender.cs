using System;
using System.Collections.Generic;

namespace magazine_music.Context;

public partial class Gender
{
    public int GenderId { get; set; }

    public string GenderName { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
