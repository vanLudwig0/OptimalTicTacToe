using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OptimalTicTacToe
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			MainPage = new MainPage();
		}

		protected override void OnStart()
		{
			OnResume();
		}

		protected override void OnSleep()
		{
			Current.Properties["OffenseWinCount"] = OffensePage.OffenseWinCount;
			Current.Properties["OffenseBestWinCount"] = OffensePage.OffenseBestWinCount;
			Current.Properties["DefenseWinCount"] = DefensePage.DefenseWinCount;
			Current.Properties["DefenseBestWinCount"] = DefensePage.DefenseBestWinCount;
			Current.SavePropertiesAsync();
		}

		protected override void OnResume()
		{
			OffensePage.OffenseWinCount = (Current.Properties["OffenseWinCount"] as int?) ?? 0;
			OffensePage.OffenseBestWinCount = (Current.Properties["OffenseBestWinCount"] as int?) ?? 0;
			DefensePage.DefenseWinCount = (Current.Properties["DefenseWinCount"] as int?) ?? 0;
			DefensePage.DefenseBestWinCount = (Current.Properties["DefenseBestWinCount"] as int?) ?? 0;
		}
	}
}
