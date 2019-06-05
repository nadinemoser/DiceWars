using System;
using Xamarin.Forms;

namespace DiceWars.Models
{
    public class GameBoard
    {
        private const int MAX_HEIGHT = 4;
        private const int MAX_WIDTH = 4;

        private static readonly Random Random = new Random();

        public GameBoard()
        {
            Computer = new Player { Name = "User", FavoriteColor = Color.Yellow };
            User = new Player { Name = "Computer", FavoriteColor = Color.Blue };
            GenerateBoard();
        }

        public Player Computer { get; set; }
        public Player User { get; set; }
        public Field[,] Board { get; set; }

        private void GenerateBoard()
        {
            Board = new Field[MAX_WIDTH, MAX_HEIGHT];

            var countUserFields = 0;
            var countComputerFields = 0;

            // generate fields
            for (int x = 0; x < MAX_WIDTH; x++)
            {
                for (int y = 0; y < MAX_HEIGHT; y++)
                {
                    var player = GetRandomPlayer(countUserFields, countComputerFields);

                    if (player == User)
                    {
                        countUserFields++;
                    }
                    else
                    {
                        countComputerFields++;
                    }

                    Board[x, y] = new Field()
                    {
                        X = x,
                        Y = y,
                        Owner =  player,
                        IsOption = true,
                        NumberOfDices = Random.Next(1, 4)
                    };
                }
            }
        }

        private Player GetRandomPlayer(int countUserFields, int countComputerFields)
        {
            var maxFieldsForPlayer = MAX_HEIGHT * MAX_WIDTH / 2;
         
            if (countUserFields == maxFieldsForPlayer)
            {
                return Computer;
            }
            if (countComputerFields == maxFieldsForPlayer)
            {
                return User;
            }

            if (Random.Next(2) == 0)
            {
                return User;
            }

            return Computer;
        }
    }
}
