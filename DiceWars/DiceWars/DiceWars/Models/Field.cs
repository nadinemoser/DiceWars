using System;
using System.Collections.Generic;
using System.Text;

namespace DiceWars.ViewModels
{
    public class Field
    {
        public Player Owner { get; set; }
        public int NumberOfDices;

        public int X { get; set; }
        public int Y { get; set; }
        public bool IsOption { get; set; }

        public Field()
        {
            NumberOfDices = 4;
        }
    }
}
