using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DiceWars
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BoardZwei : ContentPage
    {
        public IEnumerable<string> liste;
        ObservableCollection<Employee> employees = new ObservableCollection<Employee>();
        public ObservableCollection<Employee> Employees { get { return employees; } }

        public BoardZwei()
        {
            liste = new List<string>{"hello", "world"};
            BindingContext = liste;

            var button1 = new Button() {Text = "Button1", BackgroundColor = Color.Red };
            var button2 = new Button() {Text = "Button2", BackgroundColor = Color.Yellow };
            var button3 = new Button() {Text = "Button3", BackgroundColor = Color.Red };
            var button4 = new Button() {Text = "Button4", BackgroundColor = Color.Yellow };

            InitializeComponent();
        }
    }
}