using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_RPG.models
{
    public class Character
    {
        public int Id { get; set; }
        public string Name { get; set; } = "Cage";
        public int hitPoints { get; set; } = 100;
        public int strength { get; set; } = 10;
        public int defense { get; set; } = 10;
        public int intelligence  { get; set; } = 10;
        public RPGClass Class { get; set; } = RPGClass.Knight;
        public User? User { get; set; }

    }
}