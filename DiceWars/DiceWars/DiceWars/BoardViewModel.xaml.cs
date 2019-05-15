using System;
using System.Collections.Generic;
using System.Linq;
using DiceWars.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DiceWars
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BoardViewModel : ContentPage
	{
	    private const int MAX_COLUMN = 4;
	    private const int MAX_ROW = 4;

	    private static readonly Random Random = new Random();

	    private Button _challengerField;
	    private Button _defenderField;
	    private List<Button> _fields;
	    private Player _currentPlayer;

	    public Player CurrentPlayer
	    {
	        get => _currentPlayer;
	        set
	        {
	            _currentPlayer = value;
	            UpdateFieldForPlayer();
	        }
	    }

	    public BoardViewModel ()
		{
            StartGame();
		    CurrentPlayer = new Player { Name = "me", Color = Color.Red };
        }

	    private void StartGame()
	    {
	        InitializeComponent();
	        _fields = GetAllFieldsFromView();
	        SetRandomNumbers();
        }    

	    private void SetRandomNumbers()
	    {
	        foreach (var field in _fields)
	        {
	            field.Text = $"{Random.Next(1, 4).ToString()}";
                Console.WriteLine(field.Text);
	        }
	    }       

        private void OnFieldClicked(object sender, EventArgs e)
	    {
	        var clickedField = (Button) sender;

	        clickedField.BorderColor = Color.GreenYellow;

            // TODO: zweimal auf selben button: alles wird wieder resetted.

	        if (_challengerField == null)
	        {
	            _challengerField = clickedField;
	            SetUiToSecondMove();
            }
	        else
	        {
	            _defenderField = clickedField;
	            StartDiceChallenge();
                UpdateFieldForPlayer();
	        }
        }

	    private void StartDiceChallenge()
	    {
	        var rolledDiceNumberChallenger = RollDice(_challengerField);
	        var rolledDiceNumberDefender = RollDice(_defenderField);

            Console.WriteLine($"challenger: {rolledDiceNumberChallenger}; defender: {rolledDiceNumberDefender}");

	        if (rolledDiceNumberChallenger > rolledDiceNumberDefender)
	        {
                Console.WriteLine("Challenger Wins");
	            ChallengerWins();
	        }
	        else
	        {
                Console.Write("Challenger looses");
	            //ChallengerLooses();
	        }

	        ResetCurrentFields();
        }

	    private void ChallengerWins()
	    {
	        _defenderField.BackgroundColor = _challengerField.BackgroundColor;

	        int newDiceNumber = GetDiceNumberFromField(_challengerField);
            _defenderField.Text = (newDiceNumber - 1).ToString();
	        _challengerField.Text = "1";

	        if (HasTookAllFields(_challengerField))
	        {
	            EndGame();
	        }
	    }

	    private int GetDiceNumberFromField(Button field)
	    {
	        int.TryParse(field.Text, out var newDiceNumber);
	        return newDiceNumber;
	    }

	    private async void EndGame()
	    {
	        bool answer = await DisplayAlert($"{CurrentPlayer.Name} has won.", "new game?", "yes", "no");
            Console.WriteLine(answer);
	        if (answer)
	        {
                StartGame();
	        }
	        else
	        {
	            // go back to homescreen
            }
	    }

	    private bool HasTookAllFields(Button field)
	    {
	        foreach (var f in _fields)
	        {
	            if (f.BackgroundColor != field.BackgroundColor 
	                && GetDiceNumberFromField(f) > 1)
	            {
	                return false;
	            }
	        }
	        return true;
	    }

	    private void ResetCurrentFields()
	    {
	        foreach (var field in _fields)
	        {
	            field.IsEnabled = true;
	        }

	        if (_challengerField != null)
	        {
	            _challengerField.BorderColor = _challengerField.BackgroundColor;
            }

            _challengerField = null;
	        _defenderField = null;
        }

	    private int RollDice(Button field)
	    {
	        int dices;
            int.TryParse(field.Text, out dices);

	        int roledDiceNumbers = 0;

	        for (int i = 0; i < dices; i++)
	        {
	            roledDiceNumbers += Random.Next(1, 7);
	        }

	        return roledDiceNumbers;
	    }

	    private void SetUiToSecondMove()
	    {
	        foreach (var field in _fields)
	        {
	            if(IsSurroundingField(_challengerField, field)
	            && field.BackgroundColor != CurrentPlayer.Color)
	            {
	                field.IsEnabled = true;
                    // TODO: display disabled field to gray
                }
	            else
	            {
	                field.IsEnabled = false;
	            }
	        }
        }

	    private bool IsSurroundingField(Button centerField, Button surroungdingField)
	    {
	        var xDifference = Math.Abs(GetGridColumnLocation(centerField) - GetGridColumnLocation(surroungdingField));
	        var yDifference = Math.Abs(GetGridRowLocation(centerField) - GetGridRowLocation(surroungdingField));

	        if (xDifference == 1 && yDifference == 0
	            || xDifference == 0 && yDifference == 1)
	        {
                return true;
	        }
	        return false;
	    }

	    private List<Button> GetSurroundingOponentFields(Button centerField)
	    {
            //TODO: test
            return _fields.Where(x => IsSurroundingField(centerField, x)
                                        && centerField.BackgroundColor != x.BackgroundColor).ToList();
	    }

	    private int GetGridColumnLocation(Button button)
	    {
	        var width = Application.Current.MainPage.Width;
	        var x = button.X;
	        var part = width / MAX_ROW;

	        if (x == 0)
	        {
	            return 0;
	        }
	        var result = x / part;
            
            return (int) result;
	    }

	    private int GetGridRowLocation(Button button)
	    {
	        var height = Application.Current.MainPage.Height;
	        var y = button.Y;
	        var part = height / MAX_COLUMN;

	        if (y == 0)
	        {
	            return 0;
	        }

	        var result = y / part;
	        return (int) result +1;
	    }

	    private List<Button> GetAllFieldsFromView()
	    {
	        var fields = new List<Button>();
	        foreach (Button i in ((Grid) this.Content).Children.Where(c => c.GetType() == typeof(Button)))
            {
                if (i.BackgroundColor != Color.Aqua)
                {
                    fields.Add(i);
                }
            }

	        return fields;
	    }

	    private void OnEndRound(object sender, EventArgs e)
	    {
	        var endRoundButton = (Button)sender;
       
            ResetCurrentFields();

	        endRoundButton.IsEnabled = false;
	        _fields.ForEach(x => x.IsEnabled = false);

            ComputerPlays();

	        endRoundButton.IsEnabled = true;
	        _fields.ForEach(x => x.IsEnabled = true);

            //set player back to user
	        CurrentPlayer = new Player { Name = "me", Color = Color.Red };
        }
        
	    private void ComputerPlays()
	    {
	        var computer = new Player { Name = "computer", Color = Color.Yellow};
	        CurrentPlayer = computer;
            
	        _defenderField = null;

	        foreach (var challengerField in _fields)
	        {
	            if (challengerField.BackgroundColor == computer.Color 
	                && GetDiceNumberFromField(challengerField) > 1)
	            {
	                _challengerField = challengerField;

	                var oponentFields = GetSurroundingOponentFields(_challengerField);

	                if (oponentFields.Any())
	                {
	                    _defenderField = oponentFields.First();

	                    StartDiceChallenge();
                        break;
	                }
                }
	        }
        }

	    private void UpdateFieldForPlayer()
	    {
	        foreach (var field in _fields)
	        {
	            var enable = field.BackgroundColor == _currentPlayer.Color
	                         && GetDiceNumberFromField(field) > 1;
	            
                field.IsEnabled = enable;
	        }
	    }
    }
}