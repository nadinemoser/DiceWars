using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using DiceWars.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DiceWars
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BoardView : ContentPage
	{
	    private const int MAX_COLUMN = 4;
	    private const int MAX_ROW = 4;

	    private static readonly Random Random = new Random();

	    private Button _challengerField;
	    private Button _defenderField;
	    public List<Button> Fields { get; set; }
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

	    public BoardView ()
		{
            StartGame();
		    CurrentPlayer = new Player { Name = "me", FavoriteColor = Color.Red };
        }

	    private void StartGame()
	    {
	        InitializeComponent();
	        Fields = GetAllFieldsFromView();
	        SetRandomNumbers();
        }    

	    private void SetRandomNumbers()
	    {
	        foreach (var field in Fields)
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
	            if (_defenderField == _challengerField)
	            {
	                _challengerField = null;
	                return;
	            }

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
                ChallengerLooses();
            }

	        ResetCurrentFields();
        }

	    private void ChallengerLooses()
	    {

	        _challengerField.Text = "1";
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
	        foreach (var f in Fields)
	        {
	            if (f.BackgroundColor != field.BackgroundColor)
	            {
	                return false;
	            }
	        }
	        return true;
	    }

	    private void ResetCurrentFields()
	    {
	        foreach (var field in Fields)
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
	        foreach (var field in Fields)
	        {
	            if(IsSurroundingField(_challengerField, field)
	            && field.BackgroundColor != CurrentPlayer.FavoriteColor)
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
	        var temp = new List<Button>();
            foreach (var field in Fields)
            {
                if (IsSurroundingField(centerField, field)
                    && centerField.BackgroundColor != field.BackgroundColor)
                {
                    temp.Add(field);
                }
            }

	        return temp;
	    }

	    private List<Button> GetSurroundingFields(Button centerField)
	    {
	        //TODO: test
	        return Fields.Where(x => IsSurroundingField(centerField, x)).ToList();
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

	        GetReward();

	        endRoundButton.IsEnabled = false;
	        Fields.ForEach(x => x.IsEnabled = false);

            ResetCurrentFields();
            
            ComputerPlays();

	        endRoundButton.IsEnabled = true;
	        Fields.ForEach(x => x.IsEnabled = true);

            //set player back to user
	        CurrentPlayer = new Player { Name = "me", FavoriteColor = Color.Red };
        }

	    private void GetReward()
	    {
	        var newDices = GetNumberOfConnectedFields();
	        SpreadDices(newDices);
        }

	    private void SpreadDices(int newDices)
	    {
	        var ownFields = new List<Button>();
            //Get own fields...
	        foreach (var field in Fields)
	        {
	            if (CurrentPlayer.FavoriteColor == field.BackgroundColor)
	            {
                    ownFields.Add(field);
	            }
	        }

	        var ownFieldsCount = ownFields.Count;

            for (int i = 0; i < newDices; i++)
	        {
	            var randomIndex = Random.Next(ownFieldsCount);
	            var currentDiceNumber = GetDiceNumberFromField(ownFields[randomIndex]);

	            if (currentDiceNumber < 8)
	            {
	                ownFields[randomIndex].Text = (currentDiceNumber + 1).ToString();
	            }
	            else
	            {
                    i--;
                }	            
	        }          
	    }

	    public int GetNumberOfConnectedFields()
	    {
	        var ownerFields = Fields.Where(x => x.BackgroundColor == _currentPlayer.FavoriteColor).ToList();
	        
	        var connectedFields = new List<Button>();
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

	    private void ComputerPlays()
	    {
	        var computer = new Player { Name = "computer", FavoriteColor = Color.Yellow};
	        CurrentPlayer = computer;
            
	        _defenderField = null;

	        var ownFields = new List<Button>();

	        ownFields = Fields.FindAll(x => x.BackgroundColor == computer.FavoriteColor 
	                                        && GetDiceNumberFromField(x) > 1);

	        foreach (var challengerField in ownFields)
	        {
	            _challengerField = challengerField;
                // funktioniert das?? o.O
	            var oponentFields = GetSurroundingOponentFields(_challengerField);

	            if (oponentFields.Any())
	            {
	                _defenderField = oponentFields.First();

	                StartDiceChallenge();
                    break;
	            }
	        }

            GetReward();
        }

	    private void UpdateFieldForPlayer()
	    {
	        foreach (var field in Fields)
	        {
	            if (field.BackgroundColor == CurrentPlayer.FavoriteColor
	                && GetDiceNumberFromField(field) > 1)
	            {
	                field.IsEnabled = true;
	            }
	            else
	            {
	                field.IsEnabled = false;
	            }
	        }
        }
    }
}