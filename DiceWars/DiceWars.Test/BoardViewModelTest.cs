using System;
using System.Collections.Generic;
using DiceWars.ViewModels;
using Xamarin.Forms;
using Xunit;

namespace DiceWars.Test
{
    public class BoardViewModelTest
    {
        [Fact]
        public void GetNumberOfConnectedFields_NoConnectedFields_Returns0()
        {
            // Arrange
            var boardViewModel = new BoardView
            {
                CurrentPlayer = new Player {Name = "me", FavoriteColor = Color.Red}
            };
            var fields = new List<Button>
            {
                new Button{BackgroundColor = Color.Yellow},
                new Button{BackgroundColor = Color.Yellow},
                new Button{BackgroundColor = Color.Yellow},
                new Button{BackgroundColor = Color.Yellow},
                new Button{BackgroundColor = Color.Yellow},
            };
            boardViewModel.Fields = fields;

            // Act
            var maxConnectedFields = boardViewModel.GetNumberOfConnectedFields();

            // Assert
            Assert.True(maxConnectedFields == 0);
        }
    }
}
