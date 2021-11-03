using System;
using Xamarin.Forms;

namespace OptimalTicTacToe
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

		private void Offense_Clicked(object sender, EventArgs e)
		{
			Navigation.PushModalAsync(new OffensePage());
		}

		private void Defense_Clicked(object sender, EventArgs e)
		{
			Navigation.PushModalAsync(new DefensePage());
		}
	}
}
