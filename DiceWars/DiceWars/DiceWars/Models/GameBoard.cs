using System;

namespace DiceWars.Models
{
    public class GameBoard
    {
        private const int HEIGHT = 4;
        private const int WIDTH = 4;

        private static readonly Random Random = new Random();

        public GameBoard()
        {
            User = new Player { Name = "User", FavoriteColor = Color.Yellow };
            Computer = new Player { Name = "Computer", FavoriteColor = Color.Blue };
            GenerateBoard();
        }

        public Player Computer { get; set; }
        public Player User { get; set; }
        public Field[,] Board { get; set; }

        private void GenerateBoard()
        {
            Board = new Field[WIDTH, HEIGHT];

            var countUserFields = 0;
            var countComputerFields = 0;

            // generate fields
            for (var x = 0; x < WIDTH; x++)
            {
                for (var y = 0; y < HEIGHT; y++)
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

                    Board[x, y] = new Field(x, y)
                    {
                        Owner =  player,
                        IsOption = true,
                        NumberOfDices = Random.Next(1, 4)
                    };
                }
            }
        }

        private Player GetRandomPlayer(int countUserFields, int countComputerFields)
        {
            var maxFieldsForPlayer = HEIGHT * WIDTH / 2;
         
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
