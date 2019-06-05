using System;
using System.Collections.Generic;
using System.Linq;
using DiceWars.Models;
using Xamarin.Forms;

namespace DiceWars.ViewModels
{
    public class GameViewModel
    {
        private static readonly Random Random = new Random();

        private readonly Player _user = new Player() { Name = "User", FavoriteColor = Color.Yellow };
        private readonly Player _computer = new Player() { Name = "Computer", FavoriteColor = Color.Blue };

        private GameBoard _gameBoard;
        private Player _currentPlayer;

        public GameViewModel()
        {
            _gameBoard = new GameBoard();
            Board = _gameBoard.Board;
            CurrentPlayer = _user;
        }

        public Field[,] Board { get; set; }
        
        public Player CurrentPlayer
        {
            get => _currentPlayer;
            set
            {
                _currentPlayer = value;
                UpdateFieldsForPlayer();
            }
        }

        public Field ChallengerField { get; set; }
        public Field DeffenderField { get; set; }

        public void FieldGotSelected(int x, int y)
        {
            var clickedField = Board[x, y];

            if (ChallengerField == null)
            {
                ChallengerField = clickedField;
                SetPossibleOptionsForChallengerField();
            }
            else
            {
                DeffenderField = clickedField;

                if (DeffenderField == ChallengerField)
                {
                    ResetCurrentFields();
                    UpdateFieldsForPlayer();
                    return;
                }

                StartDiceChallenge();
                UpdateFieldsForPlayer();
            }
        }

        public void EndRound()
        {
            GetRewardForCurrentPlayer();
            ResetCurrentFields();

            ComputerPlays();

            CurrentPlayer = _user;
        }

        private void UpdateFieldsForPlayer()
        {
            foreach (var field in Board)
            {
                if (field.Owner.FavoriteColor == CurrentPlayer.FavoriteColor &&
                    field.NumberOfDices > 1 &&
                    IsNextToOpponentField(field))
                {
                    field.IsOption = true;
                }
                else
                {
                    field.IsOption = false;
                }
            }
        }

        private bool IsNextToOpponentField(Field centerField)
        {
            foreach (var field in Board)
            {
                if (IsSurroundingField(centerField, field) &&
                    field.Owner.FavoriteColor != CurrentPlayer.FavoriteColor)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsSurroundingField(Field centerField, Field opponentField)
        {
            var xDifference = Math.Abs(opponentField.X - centerField.X);
            var yDifference = Math.Abs(opponentField.Y - centerField.Y);

            if (xDifference == 1 && yDifference == 0 ||
                xDifference == 0 && yDifference == 1)
            {
                return true;
            }

            return false;
        }

        private void SetPossibleOptionsForChallengerField()
        {
            foreach (var field in Board)
            {
                field.IsOption = IsOpponentField(ChallengerField, field);
                if (field == ChallengerField)
                {
                    field.IsOption = true;
                }
            }
        }

        private bool IsOpponentField(Field centerField, Field opponentField)
        {
            if (IsSurroundingField(centerField, opponentField) 
                && centerField.Owner.FavoriteColor != opponentField.Owner.FavoriteColor)
            {
                return true;
            }

            return false;
        }

        private void StartDiceChallenge()
        {
            var rolledDiceNumberChallenger = RollDice(ChallengerField);
            var rolledDiceNumberDefender = RollDice(DeffenderField);
            
            if (rolledDiceNumberChallenger > rolledDiceNumberDefender)
            {
                ChallengerWins();
            }
            else
            {
                ChallengerLooses();
            }
            ResetCurrentFields();
        }

        private void ChallengerLooses()
        {
            ChallengerField.NumberOfDices = 1;
        }

        private void ChallengerWins()
        {
            DeffenderField.Owner = CurrentPlayer;
            DeffenderField.NumberOfDices = ChallengerField.NumberOfDices - 1;
            ChallengerField.NumberOfDices = 1;

        }
        
        private void ResetCurrentFields()
        {
            ChallengerField = null;
            DeffenderField = null;
        }

        private int RollDice(Field field)
        {
            int roledDiceNumbers = 0;

            for (int i = 0; i < field.NumberOfDices; i++)
            {
                roledDiceNumbers += Random.Next(1, 7);
            }

            return roledDiceNumbers;
        }

        private void ComputerPlays()
        {
            CurrentPlayer = _computer;

            var ownFields = new List<Field>();

            foreach (var field in Board)
            {
                if (field.Owner.FavoriteColor == CurrentPlayer.FavoriteColor
                    && field.NumberOfDices > 1)
                {
                    ownFields.Add(field);
                }
            }

            foreach (var challengerField in ownFields)
            {
                var opponentFields = GetSurroundingOponentFields(challengerField);

                if (!opponentFields.Any())
                {
                    continue;
                }

                foreach (var opponentField in opponentFields)
                {
                    ChallengerField = challengerField;
                    DeffenderField = opponentField;

                    if (challengerField.NumberOfDices > 1 
                        && IsOpponentField(ChallengerField, DeffenderField))
                    {
                        StartDiceChallenge();
                    }
                }
            }

            GetRewardForCurrentPlayer();
        }

        private List<Field> GetSurroundingOponentFields(Field centerField)
        {
            var surroundingOpponentFields = new List<Field>();
            foreach (var field in Board)
            {
                if (IsSurroundingField(centerField, field)
                    && centerField.Owner.FavoriteColor != field.Owner.FavoriteColor)
                {
                    surroundingOpponentFields.Add(field);
                }
            }

            return surroundingOpponentFields;
        }

        private void GetRewardForCurrentPlayer()
        {
            var newDices = GetNumberOfConnectedFields();
            SpreadDice(newDices);
        }

        private void SpreadDice(int newDice)
        {
            var possibleOwnFields = new List<Field>();
            
            foreach (var field in Board)
            {
                if (CurrentPlayer.FavoriteColor == field.Owner.FavoriteColor 
                    && field.NumberOfDices < 8)
                {
                    possibleOwnFields.Add(field);
                }
            }

            var ownFieldsCount = possibleOwnFields.Count;

            var numberOfDice = possibleOwnFields.Select(x => x.NumberOfDices).Sum();
            var maxSpaceForDice = ownFieldsCount * 8 - numberOfDice;

            if (maxSpaceForDice < newDice)
            {
                possibleOwnFields.ForEach(x => x.NumberOfDices = 8);
                return;
            }

            for (int i = 0; i < newDice; i++)
            {
                var randomIndex = Random.Next(ownFieldsCount);

                if (possibleOwnFields[randomIndex].NumberOfDices < 8)
                {
                    possibleOwnFields[randomIndex].NumberOfDices++;
                }
                else
                {
                    i--;
                }
            }
        }

        public int GetNumberOfConnectedFields()
        {
            var ownerFields = new List<Field>();
            foreach (var field in Board)
            {
                if (field.Owner.FavoriteColor == CurrentPlayer.FavoriteColor)
                {
                    ownerFields.Add(field);
                }
            }

            var connectedFields = new List<Field>();
            var counter = 0;
            var maxCounter = 0;
            var stop = false;

            while (!stop)
            {
                foreach (var field in ownerFields)
                {
                    var hasConnectedFields = !connectedFields.Any();

                    if (hasConnectedFields)
                    {
                        connectedFields.Add(field);
                        counter++;
                        continue;
                    }

                    var hasSurroundingField = ownerFields.Any(x => IsSurroundingField(x, field));

                    if (hasSurroundingField)
                    {
                        connectedFields.Add(field);
                        counter++;
                    }
                }

                connectedFields.ForEach(x => ownerFields.Remove(x));

                if (maxCounter < counter)
                {
                    maxCounter = counter;
                }

                counter = 0;
                var numberOwnerFields = ownerFields.Count();

                if (numberOwnerFields < maxCounter)
                {
                    stop = true;
                }
            }
            return maxCounter;
        }
    }
}
