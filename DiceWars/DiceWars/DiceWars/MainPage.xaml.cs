using System;
using DiceWars.Views;
using Xamarin.Forms;

namespace DiceWars
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnStartBtnClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new BetterBoardView());
        }
    }
}
