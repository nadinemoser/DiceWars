using System;
using System.Collections.Generic;
using System.Linq;
using DiceWars.Models;

namespace DiceWars.ViewModels
{
    public class GameViewModel
    {
        private const int MAX_AMOUNT_OF_DICE = 8;

        private static readonly Random Random = new Random();

        private readonly Player _user;
        private readonly Player _computer;
        private readonly GameBoard _gameBoard;

        private GameResultsViewModel _gameResults;
        private Player _currentPlayer;

        public GameViewModel()
        {
            _gameBoard = new GameBoard();
            Board = _gameBoard.Fields;
            _user = _gameBoard.User;
            _computer = _gameBoard.Computer;
            CurrentPlayer = _user;
        }
        
        public Player CurrentPlayer
        {
            get => _currentPlayer;
            set
            {
                _currentPlayer = value;
                _gameBoard.UpdateFieldsForPlayer(CurrentPlayer);
            }
        }

        public Field[,] Board { get; set; }
        
        public Field ChallengerField { get; set; }
        public Field DefenderField { get; set; }

        public GameResultsViewModel FieldGotSelected(int x, int y)
        {
            _gameResults = new GameResultsViewModel();
            var clickedField = Board[x, y];

            if (ChallengerField == null)
            {
                ChallengerField = clickedField;
                _gameBoard.SetPossibleOptionsForField(ChallengerField);
                _gameResults.ChallengerField = ChallengerField;
            }
            else
            {
                DefenderField = clickedField;

                if (DefenderField == ChallengerField)
                {
                    ResetCurrentFields();
                    _gameBoard.UpdateFieldsForPlayer(CurrentPlayer);
                    return null;
                }

                _gameResults.ChallengerField = ChallengerField;
                _gameResults.DefenderField = DefenderField;

                StartDiceChallenge();
                _gameBoard.UpdateFieldsForPlayer(CurrentPlayer);
            }

            return _gameResults;
        }

        public void EndRound()
        {
            GetRewardForCurrentPlayer();
            ResetCurrentFields();

            ComputerPlays();

            ResetCurrentFields();
        }
        
        private void StartDiceChallenge()
        {
            _gameResults.RolledDiceNumberChallenger = RollDice(ChallengerField);
            _gameResults.RolledDiceNumberDefender = RollDice(DefenderField);
            
            if (_gameResults.RolledDiceNumberChallenger > _gameResults.RolledDiceNumberDefender)
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
            DefenderField.Owner = CurrentPlayer;
            DefenderField.NumberOfDices = ChallengerField.NumberOfDices - 1;
            ChallengerField.NumberOfDices = 1;
        }
        
        private void ResetCurrentFields()
        {
            ChallengerField = null;
            DefenderField = null;
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

            var ownFields = _gameBoard.GetFieldsFromPlayer(_computer);

            foreach (var challengerField in ownFields)
            {
                var opponentFields = _gameBoard.GetSurroundingOponentFields(challengerField);

                if (!opponentFields.Any())
                {
                    continue;
                }

                foreach (var opponentField in opponentFields)
                {
                    ChallengerField = challengerField;
                    DefenderField = opponentField;

                    if (challengerField.NumberOfDices > 1 
                        && _gameBoard.IsOpponentField(ChallengerField, DefenderField))
                    {
                        StartDiceChallenge();
                    }
                }
            }

            GetRewardForCurrentPlayer();

            CurrentPlayer = _user;
        }

        private void GetRewardForCurrentPlayer()
        {
            var newDices = _gameBoard.GetNumberOfConnectedFields(CurrentPlayer);
            SpreadDice(newDices);
        }

        private void SpreadDice(int newDice)
        {
            var possibleOwnFields = new List<Field>();
            
            foreach (var field in Board)
            {
                if (CurrentPlayer == field.Owner 
                    && field.NumberOfDices < MAX_AMOUNT_OF_DICE)
                {
                    possibleOwnFields.Add(field);
                }
            }

            var ownFieldsCount = possibleOwnFields.Count;

            var numberOfDice = possibleOwnFields.Select(x => x.NumberOfDices).Sum();
            var maxSpaceForDice = ownFieldsCount * MAX_AMOUNT_OF_DICE - numberOfDice;

            if (maxSpaceForDice <= newDice)
            {
                possibleOwnFields.ForEach(x => x.NumberOfDices = MAX_AMOUNT_OF_DICE);
                return;
            }

            for (int i = 0; i < newDice; i++)
            {
                var randomIndex = Random.Next(ownFieldsCount);

                if (possibleOwnFields[randomIndex].NumberOfDices < MAX_AMOUNT_OF_DICE)
                {
                    possibleOwnFields[randomIndex].NumberOfDices++;
                }
                else
                {
                    i--;
                }
            }
        }
    }
}
