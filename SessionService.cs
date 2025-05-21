using magazine_music.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magazine_music
{
    public static class Session
    {
        public static User? CurrentUser { get; set; }
    }
}
