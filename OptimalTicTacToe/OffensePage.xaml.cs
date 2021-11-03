using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OptimalTicTacToe
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class OffensePage : ContentPage
	{
		public GameEngine.Board GameBoard { get; set; }
		private int _winCount = 0;
		private int _bestWinCount = 0;

		public OffensePage()
		{
			InitializeComponent();

			GameBoard = new GameEngine.Board(S00, S01, S02, S10, S11, S12, S20, S21, S22);
			ResetBoard();
		}

		private void ResetBoard()
		{
			do
			{
				GameBoard.Clear();
				GameBoard.EmptyOutsides.RandomOrDefault().Value = "X";
				GameBoard.EmptySquares.RandomOrDefault().Value = "O";

				if (GameBoard.Middles.Any(t => t.Mixed())) continue;	//Unwinnable
				if (GameBoard.Mixeds().Any(t => t[1].X)) continue;	//Unwinnable
				break;
			} while (true);
		}

		private async void Clicked(object sender, EventArgs e)
		{
			if (sender is Button button && string.IsNullOrEmpty(button.Text))
			{
				button.Text = "X";

				await Task.Delay(500);

				//X wins
				var win = GameBoard.Triples.FirstOrDefault(triple => triple.AllX());
				if (win != null)
				{
					_winCount++;
					_bestWinCount = Math.Max(_bestWinCount, _winCount);
					CurrentStreakLabel.FormattedText.Spans[1].Text = _winCount.ToString() + (_winCount == 1 ? " Win" : " Wins");
					RecordLabel.FormattedText.Spans[1].Text = _bestWinCount.ToString() + (_bestWinCount == 1 ? " Win" : " Wins");
					ResetBoard();
					return;
				}

				//Tie.  (Remember, a tie can only occur after an X move)
				if (GameBoard.EmptySquares.Count() == 0)
				{
					_winCount = 0;
					CurrentStreakLabel.FormattedText.Spans[1].Text = _winCount.ToString() + (_winCount == 1 ? " Win" : " Wins");
					ResetBoard();
					return;
				}

				await ComputerMove();
			}
		}

		private async Task ComputerMove()
		{
			//O wins
			var winnable = GameBoard.WinnableO().RandomOrDefault();
			if (winnable != null)
			{
				winnable.First(s => s.Empty).Value = "O";
				_winCount = 0;
				CurrentStreakLabel.FormattedText.Spans[1].Text = _winCount.ToString() + (_winCount == 1 ? " Win" : " Wins");
				await Task.Delay(1000);
				ResetBoard();
				return;
			}

			//Make an intentional mistake when the first O is in the center
			if (GameBoard.EmptySquares.Count() == 6 && GameBoard.Diagonals.Any(t => t.Matches("XOX")))
			{
				GameBoard.Corners.Where(s => s.Empty).RandomOrDefault().Value = "O";
				return;
			}


			//Block an X win
			var loseable = GameBoard.WinnableX().RandomOrDefault();
			if (loseable != null)
			{
				loseable.First(s => s.Empty).Value = "O";
				return;
			}


			GameBoard.EmptySquares.RandomOrDefault().Value = "O";
		}
	}
}