using DiceWars.Models;
using DiceWars.ViewModels;
using Xamarin.Forms;
using Xunit;

namespace DiceWars.Test
{
    public class GameViewModelTest
    {
        [Fact]
        public void GetNumberOfConnectedFields_OneConnectedFields_Returns0()
        {
            // Arrange
            var user = new Player { FavoriteColor = Color.Yellow };

            var boardViewModel = new GameViewModel
            {
                CurrentPlayer = user,
                Board = GetFieldsWithComputerPlayer()
            };

            boardViewModel.Board[0, 0] = new Field { Owner = user };
            
            // Act
            var maxConnectedFields = boardViewModel.GetNumberOfConnectedFields();

            // Assert
            Assert.True(maxConnectedFields == 1);
        }

        [Fact]
        public void GetNumberOfConnectedFields_TwoConnectedFields_Returns2()
        {
            // Arrange
            var user = new Player { FavoriteColor = Color.Yellow };

            var boardViewModel = new GameViewModel
            {
                CurrentPlayer = user,
                Board = GetFieldsWithComputerPlayer()
            };

            boardViewModel.Board[0, 0] = new Field { Owner = user };
            boardViewModel.Board[0, 1] = new Field { Owner = user };

            // Act
            var maxConnectedFields = boardViewModel.GetNumberOfConnectedFields();

            // Assert
            Assert.True(maxConnectedFields == 2);
        }

        private Field[,] GetFieldsWithComputerPlayer()
        {
            var computer = new Player { FavoriteColor = Color.Blue };

            var board = new Field[4, 4];

            board[0, 0] = new Field { Owner = computer };
            board[0, 1] = new Field { Owner = computer };
            board[0, 2] = new Field { Owner = computer };
            board[0, 3] = new Field { Owner = computer };

            board[1, 0] = new Field { Owner = computer };
            board[1, 1] = new Field { Owner = computer };
            board[1, 2] = new Field { Owner = computer };
            board[1, 3] = new Field { Owner = computer };

            board[2, 0] = new Field { Owner = computer };
            board[2, 1] = new Field { Owner = computer };
            board[2, 2] = new Field { Owner = computer };
            board[2, 3] = new Field { Owner = computer };

            board[3, 0] = new Field { Owner = computer };
            board[3, 1] = new Field { Owner = computer };
            board[3, 2] = new Field { Owner = computer };
            board[3, 3] = new Field { Owner = computer };

            return board;
        }
    }
}
