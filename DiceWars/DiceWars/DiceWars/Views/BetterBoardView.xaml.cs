using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiceWars.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DiceWars.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BetterBoardView : ContentPage
	{
	    private GameViewModel _gameViewModel;
	    private Button _lastFramedField;

		public BetterBoardView ()
		{
			InitializeComponent ();
            _gameViewModel = new GameViewModel();

		    Content = GenerateGrid();
            UpdateView();
        }

	    private Grid GenerateGrid()
	    {
	        var grid = new Grid();

            foreach (var field in _gameViewModel.Board)
	        {
                var button = new Button{Text = $"{field.NumberOfDices}", BackgroundColor = field.Owner.FavoriteColor};
	            Grid.SetColumn(button, field.X);
	            Grid.SetRow(button, field.Y);

                grid.Children.Add(button);
                grid.Padding = new Thickness(0,0,0,0);

	            button.Clicked += OnFieldClicked;
	        }

            // Ad EndRound Button
	        var endRoundButton = new Button(){Text = "End Round"};
	        Grid.SetColumn(endRoundButton, 1);
            Grid.SetColumnSpan(endRoundButton, 2);
	        Grid.SetRow(endRoundButton, 4);
	        grid.Children.Add(endRoundButton);
	        endRoundButton.Clicked += OnEndRoundClicked;

            return grid;
	    }

        private void OnEndRoundClicked(object sender, EventArgs e)
        {
            _gameViewModel.EndRound();
            UpdateView();
        }

        private void OnFieldClicked(object sender, EventArgs e)
        {
            var field = (Button)sender;

            if (_lastFramedField != null)
            {
                _lastFramedField.BorderWidth = 0;
                _lastFramedField = null;
            }
            else
            {
                _lastFramedField = field;
                _lastFramedField.BorderColor = Color.Red;
                _lastFramedField.BorderWidth = 2;
            }

	        var x = Grid.GetColumn(field);
	        var y = Grid.GetRow(field);

	        _gameViewModel.FieldGotSelected(x, y);
	        UpdateView();
        }

	    private void UpdateView()
	    {
	        foreach (var view in ((Grid)this.Content).Children.Where(c => c.GetType() == typeof(Button)))
	        {
	            var button = (Button) view;
	            foreach (var field in _gameViewModel.Board)
	            {
	                if (Grid.GetColumn(button) == field.X && Grid.GetRow(button) == field.Y)
	                {
	                    button.BackgroundColor = field.Owner.FavoriteColor;
	                    button.Text = field.NumberOfDices.ToString();
	                    button.IsEnabled = field.IsOption;
	                }
	            }
	        }

	        if (HasGameEnd())
	        {
	            ShoeEndGameMessage();
	        }
	    }

	    private async void ShoeEndGameMessage()
	    {
	        var player = _gameViewModel.Board[0, 0].Owner;
	        bool answer = await DisplayAlert($"{player.Name} has won.", "new game?", "yes", "no");

	        Console.WriteLine(answer);
	        if (answer)
	        {
	            _gameViewModel = new GameViewModel();
                UpdateView();
	        }
	        else
	        {
	            await Navigation.PopModalAsync();
            }
        }

	    private bool HasGameEnd()
	    {
	        var fieldToCompareWith = _gameViewModel.Board[0, 0];
	        foreach (var field in _gameViewModel.Board)
	        {
	            if (fieldToCompareWith.Owner.FavoriteColor != field.Owner.FavoriteColor)
	            {
	                return false;
	            }
	        }

	        return true;
	    }
	}
}