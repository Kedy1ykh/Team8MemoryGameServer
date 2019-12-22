using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemoryGameServer.Models
{
    public class Game
    {
        public Match match { get; set; }
        public Person winner { get; set; }
    }
}