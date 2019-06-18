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
	    private GameResultsViewModel _gameResultViewModel;
	    private Label _challengerLabel;
	    private Label _defenderLabel;

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

            // add labels for amount of dice numbers
	        _challengerLabel = new Label
	        {
	            HorizontalTextAlignment = TextAlignment.Center,
	            VerticalTextAlignment = TextAlignment.Center,
	            Text = "User"
	        };
	        Grid.SetColumn(_challengerLabel, 0);
	        Grid.SetRow(_challengerLabel, 4);


            var boxView = new BoxView {BackgroundColor = Xamarin.Forms.Color.Yellow, CornerRadius = 40};
	        Grid.SetColumn(boxView, 0);
	        Grid.SetRow(boxView, 4);

	        grid.Children.Add(boxView);
	        grid.Children.Add(_challengerLabel);

	        _defenderLabel = new Label
	        {
	            HorizontalTextAlignment = TextAlignment.Center,
	            VerticalTextAlignment = TextAlignment.Center,
	            Text = "Computer"
	        };
	        Grid.SetColumn(_defenderLabel, 3);
	        Grid.SetRow(_defenderLabel, 4);

	        var boxViewBlue = new BoxView
	        {
	            BackgroundColor = Xamarin.Forms.Color.LightBlue,
	            CornerRadius = 40
	        };

	        Grid.SetColumn(boxViewBlue, 3);
	        Grid.SetRow(boxViewBlue, 4);
	        grid.Children.Add(boxViewBlue);
            grid.Children.Add(_defenderLabel);

	        grid.Margin = 10;

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

            var x = Grid.GetColumn(field);
	        var y = Grid.GetRow(field);

	        _gameResultViewModel = _gameViewModel.FieldGotSelected(x, y);
	        UpdateView();
        }

        private void SetFrameForField(Button field)
        {
            _lastFramedField = field;
            _lastFramedField.BorderColor = Xamarin.Forms.Color.Red;
            _lastFramedField.BorderWidth = 2;
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

	        UpdateViewWithResults();

	        if (HasGameEnd())
	        {
	            ShowEndGameMessage();
	        }
	    }

	    private void UpdateViewWithResults()
	    {
	        if (_gameResultViewModel == null)
	        {
                ResetLastFramedField();
	            return;
	        }

	        if (_gameResultViewModel.ChallengerField != null 
	            && _gameResultViewModel.DefenderField == null)
	        {
                var field = _gameResultViewModel.ChallengerField;
                foreach (var view in ((Grid)Content).Children.Where(c => c.GetType() == typeof(Button)))
                {
                    var button = (Button)view;

                    if (Grid.GetColumn(button) == field.XCoordinate && Grid.GetRow(button) == field.YCoordinate)
                    {
                        SetFrameForField(button);
                    }
                }
            }

	        if (_gameResultViewModel.ChallengerField != null 
	            && _gameResultViewModel.DefenderField != null)
	        {
                ResetLastFramedField();
	            _challengerLabel.Text = $"{_gameResultViewModel.ChallengerFieldUser}:  {_gameResultViewModel.RolledDiceNumberChallenger.ToString()}";
	            _defenderLabel.Text = $"{_gameResultViewModel.DefenderFieldUser}:  {_gameResultViewModel.RolledDiceNumberDefender.ToString()}";
            }
	    }

	    private async void ShowEndGameMessage()
	    {
	        var player = _gameViewModel.Board[0, 0].Owner;
	        bool answer = await DisplayAlert($"{player.Name} has won.", "new game?", "yes", "no");

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
	            if (fieldToCompareWith.Owner != field.Owner)
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
	            case Color.LightBlue:
	            {
	                return Xamarin.Forms.Color.LightBlue;
	            }
	            default:
	            {
	                return Xamarin.Forms.Color.Black;
	            }
            }

        }
	}
}