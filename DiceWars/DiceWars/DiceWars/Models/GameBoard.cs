using System;
using System.Collections.Generic;
using System.Text;
using DiceWars.ViewModels;
using Xamarin.Forms;

namespace DiceWars.Models
{
    public class GameBoard
    {
        private const int MAX_HEIGHT = 4;
        private const int MAX_WIDTH = 4;

        private static readonly Random Random = new Random();

        private Player _computer;
        private Player _user;

        public GameBoard()
        {
            _computer = new Player() { Name = "User", FavoriteColor = Color.Yellow };
            _user = new Player() { Name = "Computer", FavoriteColor = Color.Blue };
            GenerateBoard();
        }

        public Field[,] Board { get; set; }

        private void GenerateBoard()
        {
            Board = new Field[MAX_WIDTH, MAX_HEIGHT];
            Player player;

            var maxFieldsForPlayer = MAX_HEIGHT * MAX_WIDTH / 2;
            var countUser = 0;
            var countComputer = 0;

            // generate fields
            for (int x = 0; x < MAX_WIDTH; x++)
            {
                for (int y = 0; y < MAX_HEIGHT; y++)
                {
                    // find random player
                    if (countUser == maxFieldsForPlayer)
                    {
                        player = _computer;
                        countComputer++;
                    }
                    else if(countComputer == maxFieldsForPlayer)
                    {
                        player = _user;
                        countUser++;
                    }
                    else
                    {
                        if (Random.Next(2) == 0)
                        {
                            player = _user;
                            countUser++;
                        }
                        else
                        {
                            player = _computer;
                            countComputer++;
                        }
                    }

                    Board[x, y] = new Field()
                    {
                        X = x,
                        Y = y,
                        Owner =  player,
                        IsOption = true
                    };
                }
            }
            SetRandomNumbers(1, 3);
        }

        private void SetRandomNumbers(int min, int max)
        {
            foreach (var field in Board)
            {
                field.NumberOfDices = Random.Next(min, max + 1);
            }
        }

    }
}
