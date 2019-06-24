using System;
using System.Linq;
using DiceWars.Controls;
using DiceWars.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DiceWars.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BetterBoardView : ContentPage
    {
        private DiceResultsControl _userControl;
        private DiceResultsControl _defenderControl;
        private GameResultsViewModel _gameResultViewModel;
        private GameViewModel _gameViewModel;
        private Button _lastFramedField;
        private Button _endRoundButton;
        private Grid _grid;

        public BetterBoardView()
        {
            InitializeComponent();
            _gameViewModel = new GameViewModel();
            Content = GenerateGrid();
            UpdateView();
            SizeChanged += MainPageSizeChanged;
        }

        private void MainPageSizeChanged(object sender, EventArgs e)
        {
            bool isPortrait = Height > Width;
            if (isPortrait)
            {
                Grid.SetColumn(_userControl, 0);
                Grid.SetRow(_userControl, 4);

                Grid.SetColumn(_endRoundButton, 1);
                Grid.SetRow(_endRoundButton, 4);
                Grid.SetColumnSpan(_endRoundButton, 2);
                Grid.SetRowSpan(_endRoundButton, 1);

                Grid.SetColumn(_defenderControl, 3);
                Grid.SetRow(_defenderControl, 4);

                if (_grid.ColumnDefinitions.Count > 4 && _grid.RowDefinitions.Count > 4)
                {
                    _grid.ColumnDefinitions[4].Width = 0;
                    _grid.RowDefinitions[4].Height = new GridLength(4, GridUnitType.Star);
                }
                _userControl.Image = "yellowdiceoriginal.png";
                _defenderControl.Image = "lightbluediceoriginal.png";
            }
            else
            {
                Grid.SetColumn(_userControl, 4);
                Grid.SetRow(_userControl, 0);

                Grid.SetColumn(_endRoundButton, 4);
                Grid.SetRow(_endRoundButton, 1);
                Grid.SetColumnSpan(_endRoundButton, 1);
                Grid.SetRowSpan(_endRoundButton, 2);

                Grid.SetColumn(_defenderControl, 4);
                Grid.SetRow(_defenderControl, 3);

                if (_grid.ColumnDefinitions.Count > 4 && _grid.RowDefinitions.Count > 4)
                {
                    _grid.RowDefinitions[4].Height = 0;
                    _grid.ColumnDefinitions[4].Width = new GridLength(4, GridUnitType.Star);
                }

                _userControl.Image = "yellowdice.png";
                _defenderControl.Image = "lightbluedice.png";
            }
        }

        private Grid GenerateGrid()
        {
            _grid = new Grid();

            foreach (var field in _gameViewModel.Board)
            {
                var button = new Button
                {
                    Text = $"{field.NumberOfDices}",
                    FontSize = 20,

                    BackgroundColor = GetColor(field.Owner.FavoriteColor)
                };

                SetFrameForField(button, GetBorderColor(field.Owner.FavoriteColor));

                Grid.SetColumn(button, field.XCoordinate);
                Grid.SetRow(button, field.YCoordinate);

                _grid.Children.Add(button);
                _grid.Padding = new Thickness(0, 0, 0, 0);

                button.Clicked += OnFieldClicked;
            }

            // Add EndRound Button
            _endRoundButton = new Button {Text = "End Round"};
            Grid.SetColumn(_endRoundButton, 1);
            Grid.SetColumnSpan(_endRoundButton, 2);
            Grid.SetRow(_endRoundButton, 4);
            _grid.Children.Add(_endRoundButton);
            _endRoundButton.Clicked += OnEndRoundClicked;
            
            _userControl = new DiceResultsControl();
            _userControl.UserText = "Nadine :";
            _userControl.Image = "yellowdice.png";

            Grid.SetColumn(_userControl, 0);
            Grid.SetRow(_userControl, 4);
            _grid.Children.Add(_userControl);

            _defenderControl = new DiceResultsControl();
            _defenderControl.UserText = "Computer :";
            _defenderControl.Image = "lightbluedice.png";

            Grid.SetColumn(_defenderControl, 3);
            Grid.SetRow(_defenderControl, 4);
            _grid.Children.Add(_defenderControl);

            _grid.Margin = 10;

            return _grid;
        }

        private void OnEndRoundClicked(object sender, EventArgs e)
        {
            _gameViewModel.EndRound();
            UpdateView();
            ResetLastFramedField();
            ResetScore();
        }

        private void ResetScore()
        {
            _userControl.NumberOfDiceText = string.Empty;
            _defenderControl.NumberOfDiceText = string.Empty;
        }

        private void ResetLastFramedField()
        {
            if (_lastFramedField != null)
            {
                SetFrameForField(_lastFramedField, GetBorderColorFromBackground(_lastFramedField.BackgroundColor));
            }
        }

        private void OnFieldClicked(object sender, EventArgs e)
        {
            var field = (Button) sender;

            var x = Grid.GetColumn(field);
            var y = Grid.GetRow(field);

            _gameResultViewModel = _gameViewModel.FieldGotSelected(x, y);
            UpdateView();
        }

        private void SetFrameForField(Button field, Xamarin.Forms.Color color)
        {
            _lastFramedField = field;
            _lastFramedField.BorderColor = color;
            _lastFramedField.BorderWidth = 2;
        }

        private void UpdateView()
        {
            foreach (var view in ((Grid) Content).Children.Where(c => c.GetType() == typeof(Button)))
            {
                var button = (Button) view;
                foreach (var field in _gameViewModel.Board)
                {
                    if (Grid.GetColumn(button) == field.XCoordinate && Grid.GetRow(button) == field.YCoordinate)
                    {
                        button.BackgroundColor = GetColor(field.Owner.FavoriteColor);
                        button.Text = field.NumberOfDices.ToString();
                        button.IsEnabled = field.IsOption;
                        SetFrameForField(button, GetBorderColorFromBackground(button.BackgroundColor));
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
                foreach (var view in ((Grid) Content).Children.Where(c => c.GetType() == typeof(Button)))
                {
                    var button = (Button) view;

                    if (Grid.GetColumn(button) == field.XCoordinate && Grid.GetRow(button) == field.YCoordinate)
                    {
                        SetFrameForField(button, Xamarin.Forms.Color.Red);
                    }
                }
            }

            if (_gameResultViewModel.ChallengerField != null
                && _gameResultViewModel.DefenderField != null)
            {
                ResetLastFramedField();
                _userControl.NumberOfDiceText = $"{_gameResultViewModel.RolledDiceNumberChallenger.ToString()}";
                _defenderControl.NumberOfDiceText = $"{_gameResultViewModel.RolledDiceNumberDefender.ToString()}";
            }
        }

        private async void ShowEndGameMessage()
        {
            var player = _gameViewModel.Board[0, 0].Owner;
            var answer = await DisplayAlert($"{player.Name} has won.", "new game?", "yes", "no");

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

        private Xamarin.Forms.Color GetBorderColorFromBackground(Xamarin.Forms.Color color)
        {
            if (color == Xamarin.Forms.Color.Yellow)
            {
                return Xamarin.Forms.Color.Red;
            }

            if (color == Xamarin.Forms.Color.LightBlue)
            {
                return Xamarin.Forms.Color.Blue;
            }

            return Xamarin.Forms.Color.Black;
        }

        private Xamarin.Forms.Color GetBorderColor(Color color)
        {
            switch (color)
            {
                case Color.Yellow:
                {
                    return Xamarin.Forms.Color.Red;
                }
                case Color.Blue:
                {
                    return Xamarin.Forms.Color.Blue;
                }
                case Color.LightBlue:
                {
                    return Xamarin.Forms.Color.Blue;
                }
                default:
                {
                    return Xamarin.Forms.Color.Black;
                }
            }
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