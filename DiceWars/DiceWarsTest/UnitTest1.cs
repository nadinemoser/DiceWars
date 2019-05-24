using DiceWars;
using NUnit.Framework;
using Xamarin.Forms;

namespace Tests
{
    public class BoardViewModelTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            // Arrange
            var viewModel = new BoardViewModel();
            var field = new Button {Text = "1"};

            // Act
            viewModel.GetNumberOfConnectedFields();

            // Assert
        }


        public void GetTestdata()
        {

        }
    }
}