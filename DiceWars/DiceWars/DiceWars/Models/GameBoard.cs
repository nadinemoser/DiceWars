using System;
using System.Collections.Generic;
using System.Linq;

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
            Computer = new Player { Name = "Computer", FavoriteColor = Color.LightBlue };
            GenerateBoard();
        }

        public Player Computer { get; set; }
        public Player User { get; set; }
        public Field[,] Fields { get; set; }

        public void UpdateFieldsForPlayer(Player player)
        {
            foreach (var field in Fields)
            {
                field.IsOption = field.Owner == player &&
                                 field.NumberOfDices > 1 &&
                                 IsNextToOpponentField(field, player);
            }
        }
     
        public bool IsOpponentField(Field centerField, Field opponentField)
        {
            if (IsSurroundingField(centerField, opponentField)
                && centerField.Owner != opponentField.Owner)
            {
                return true;
            }

            return false;
        }

        public void SetPossibleOptionsForField(Field challengerField)
        {
            foreach (var field in Fields)
            {
                field.IsOption = IsOpponentField(challengerField, field);           
            }

            challengerField.IsOption = true;
        }

        public List<Field> GetSurroundingOponentFields(Field centerField)
        {
            var surroundingOpponentFields = new List<Field>();
            foreach (var field in Fields)
            {
                if (IsSurroundingField(centerField, field)
                    && centerField.Owner != field.Owner)
                {
                    surroundingOpponentFields.Add(field);
                }
            }

            return surroundingOpponentFields;
        }

        public List<Field> GetFieldsFromPlayer(Player player)
        {
            var playerFields = new List<Field>();

            foreach (var field in Fields)
            {
                if (field.Owner == player
                    && field.NumberOfDices > 1)
                {
                    playerFields.Add(field);
                }
            }

            return playerFields;
        }

        public int GetNumberOfConnectedFields(Player currentPlayer)
        {
            var ownerFields = GetFieldsFromPlayer(currentPlayer);

            var connectedFields = new List<Field>();
            var counter = 0;
            var maxCounter = 0;
            var stop = false;

            //while (!stop)
            //{
            //    foreach (var field in ownerFields)
            //    {
            //        if (!connectedFields.Any())
            //        {
            //            connectedFields.Add(field);
            //            counter++;
            //            continue;
            //        }

            //        var hasSurroundingField = connectedFields.Any(x => IsSurroundingField(x, field));

            //        if (hasSurroundingField)
            //        {
            //            connectedFields.Add(field);
            //            counter++;
            //        }
            //    }

            //    connectedFields.ForEach(x => ownerFields.Remove(x));
            //    connectedFields = new List<Field>();

            //    if (maxCounter < counter)
            //    {
            //        maxCounter = counter;
            //    }

            //    counter = 0;
            //    var numberOwnerFields = ownerFields.Count();

            //    if (numberOwnerFields < maxCounter)
            //    {
            //        stop = true;
            //    }
            //}

            maxCounter = 5;
            return maxCounter;
        }

        private bool IsNextToOpponentField(Field centerField, Player player)
        {
            foreach (var field in Fields)
            {
                if (IsSurroundingField(centerField, field) &&
                    field.Owner != player)

                {
                    return true;
                }
            }

            return false;
        }

        private bool IsSurroundingField(Field centerField, Field opponentField)
        {
            var xDifference = Math.Abs(opponentField.XCoordinate - centerField.XCoordinate);
            var yDifference = Math.Abs(opponentField.YCoordinate - centerField.YCoordinate);

            var totalDifference = xDifference + yDifference;

            return totalDifference == 1;
        }

        private void GenerateBoard()
        {
            Fields = new Field[WIDTH, HEIGHT];

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

                    Fields[x, y] = new Field(x, y)
                    {
                        Owner = player,
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
