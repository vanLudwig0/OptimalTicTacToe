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
			object value;
			if (Current.Properties.TryGetValue("OffenseWinCount", out value)) OffensePage.OffenseWinCount = (value as int?) ?? 0; else OffensePage.OffenseWinCount = 0;
			if (Current.Properties.TryGetValue("OffenseBestWinCount", out value)) OffensePage.OffenseBestWinCount = (value as int?) ?? 0; else OffensePage.OffenseBestWinCount = 0;
			if (Current.Properties.TryGetValue("DefenseWinCount", out value)) DefensePage.DefenseWinCount = (value as int?) ?? 0; else DefensePage.DefenseWinCount = 0;
			if (Current.Properties.TryGetValue("DefenseBestWinCount", out value)) DefensePage.DefenseBestWinCount = (value as int?) ?? 0; else DefensePage.DefenseBestWinCount = 0;
		}
	}
}
