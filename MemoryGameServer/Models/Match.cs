using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemoryGameServer.Models
{
    public class Match
    {
        public Person Player1 { get; set; }
        public Person Player2 { get; set; }
    }
}