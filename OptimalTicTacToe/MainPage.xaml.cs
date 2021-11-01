using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
	}
}
