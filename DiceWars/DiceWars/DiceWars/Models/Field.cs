namespace DiceWars.Models
{
    public class Field
    {
        public Field(int xCoordinate, int yCoordinate)
        {
            XCoordinate = xCoordinate;
            YCoordinate = yCoordinate;
        }

        public Player Owner { get; set; }
        public int NumberOfDices { get; set; }
        public int XCoordinate { get; }
        public int YCoordinate { get; }
        public bool IsOption { get; set; }
    }
}
