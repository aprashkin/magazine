using System;
using System.Collections.Generic;

namespace magazine_music.Context;

public partial class User
{
    public int UserId { get; set; }

    public string UserFirstname { get; set; } = null!;

    public string UserLastname { get; set; } = null!;

    public DateOnly UserBirthday { get; set; }

    public int GenderId { get; set; }

    public byte[] UserPassword { get; set; } = null!;

    public int RoleId { get; set; }

    public string UserEmail { get; set; } = null!;

    public virtual Gender Gender { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Role Role { get; set; } = null!;
}
