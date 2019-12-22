using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemoryGameServer.Models
{
    public class Person
    {
        public String name { get; set; }

        public override bool Equals(object p)
        {

            Person op = (Person)p;
            return this.name.Equals(op.name);
        }
    }
}