using DiceWars.Models;

namespace DiceWars.ViewModels
{
    public class GameResultsViewModel
    {
        public Field ChallengerField { get; set; }
        public Field DefenderField { get; set; }
        public string ChallengerFieldUser { get; set; }
        public string DefenderFieldUser { get; set; }
        public int RolledDiceNumberChallenger { get; set; }
        public int RolledDiceNumberDefender { get; set; }
    }
}
