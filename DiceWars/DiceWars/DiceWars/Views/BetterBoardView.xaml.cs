using System;
using System.Linq;
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
                var button = new Button{Text = $"{field.NumberOfDices}", BackgroundColor = GetColor(field.Owner.FavoriteColor)};
	            Grid.SetColumn(button, field.XCoordinate);
	            Grid.SetRow(button, field.YCoordinate);

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
            ResetLastFramedField();
        }

	    private void ResetLastFramedField()
	    {
            if(_lastFramedField != null)
	        {
	            _lastFramedField.BorderWidth = 0;
	            _lastFramedField = null;
            }
        }

	    private void OnFieldClicked(object sender, EventArgs e)
        {
            var field = (Button)sender;

            if (_lastFramedField == null)
            {
                _lastFramedField = field;
                _lastFramedField.BorderColor = Xamarin.Forms.Color.Red;
                _lastFramedField.BorderWidth = 2;
            }
            else
            {
                ResetLastFramedField();
            }

            var x = Grid.GetColumn(field);
	        var y = Grid.GetRow(field);

	        _gameViewModel.FieldGotSelected(x, y);
	        UpdateView();
        }

	    private void UpdateView()
	    {
	        foreach (var view in ((Grid)Content).Children.Where(c => c.GetType() == typeof(Button)))
	        {
	            var button = (Button) view;
	            foreach (var field in _gameViewModel.Board)
	            {
	                if (Grid.GetColumn(button) == field.XCoordinate && Grid.GetRow(button) == field.YCoordinate)
	                {
	                    button.BackgroundColor = GetColor(field.Owner.FavoriteColor);
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

	    private Xamarin.Forms.Color GetColor(Color color)
	    {
	        switch (color)
	        {
	            case Color.Yellow:
	            {
	                return Xamarin.Forms.Color.Yellow;
	            }
	            case Color.Blue:
	            {
	                return Xamarin.Forms.Color.Blue;
	            }
	            default:
	            {
	                return Xamarin.Forms.Color.Black;
	            }
            }

        }
	}
}