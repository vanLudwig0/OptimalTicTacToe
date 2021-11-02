using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OptimalTicTacToe
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class OffensePage : ContentPage
	{
		public GameEngine.Board GameBoard { get; set; } = new GameEngine.Board();

		public OffensePage()
		{
			InitializeComponent();

			BindingContext = this;
		}
	}
}