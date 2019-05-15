using System;
using System.Collections.Generic;
using System.Text;

namespace DiceWars.ViewModels
{
    public class Field
    {
        private Player _currentPlayer;
        public int NumberOfDices;

        public Field()
        {
            _currentPlayer = new Player() { Name = "me" };
            NumberOfDices = 4;
        }
    }
}
