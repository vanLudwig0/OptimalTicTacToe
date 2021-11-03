using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OptimalTicTacToe
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class OffensePage : ContentPage
	{
		public GameEngine.Board GameBoard { get; }
		private int _winCount = 0;
		private int _bestWinCount = 0;

		public OffensePage()
		{
			InitializeComponent();

			GameBoard = new GameEngine.ButtonImpl.BoardButtonImpl(S00, S01, S02, S10, S11, S12, S20, S21, S22);
			ResetBoard();
		}

		private void ResetBoard()
		{
			do
			{
				GameBoard.Clear();
				GameBoard.EmptyOutsides.RandomOrDefault().Value = "X";
				GameBoard.EmptySquares.RandomOrDefault().Value = "O";

				if (GameBoard.Matches("_X_/_O_/___")) continue;     //Unwinnable
				if (GameBoard.Matches("_X_/___/_O_")) continue;     //Unwinnable
				if (GameBoard.Matches("OX_/___/___")) continue;     //Unwinnable

				break;
			} while (true);
		}

		//Clicked has some short sleeps in it to give the appearance of the computer thinking.  It makes it easier for the human
		//player to see what the computer's latest move was.  Using a spin lock to prevent the human from making additional clicks
		//during these sleeps.
		System.Threading.SpinLock _lock = new System.Threading.SpinLock();
		private async void Clicked(object sender, EventArgs e)
		{
			bool gotLock = false;
			try
			{
				_lock.Enter(ref gotLock);

				if (sender is Button button && string.IsNullOrEmpty(button.Text))
				{
					button.Text = "X";

					//X wins
					GameEngine.Triple win = GameBoard.Triples.FirstOrDefault(triple => triple.AllX());
					if (win != null)
					{
						await GameOver(true);
						return;
					}

					//Tie.  (Remember, a tie can only occur after an X move)
					if (GameBoard.EmptySquares.Count() == 0)
					{
						await GameOver(false);
						return;
					}

					await Task.Delay(350);
					await ComputerMove();
				}
			}
			catch { }
			finally
			{
				if (gotLock) _lock.Exit();
			}
		}

		private async Task ComputerMove()
		{
			//O wins
			var winnable = GameBoard.WinnableO().RandomOrDefault();
			if (winnable != null)
			{
				winnable.First(s => s.Empty).Value = "O";
				await GameOver(false);
				return;
			}

			//Make an intentional mistake when the first O is in the center and X makes the best response
			if (GameBoard.Matches("X__/_O_/__X"))
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

		private async Task GameOver(bool positiveResult)
		{
			if (positiveResult)
			{
				_winCount++;
				_bestWinCount = Math.Max(_bestWinCount, _winCount);
				await Task.Delay(350);
			}
			else
			{
				_winCount = 0;
				await Task.Delay(750);
			}

			CurrentStreakLabel.FormattedText.Spans[1].Text = _winCount.ToString() + (_winCount == 1 ? " Win" : " Wins");
			RecordLabel.FormattedText.Spans[1].Text = _bestWinCount.ToString() + (_bestWinCount == 1 ? " Win" : " Wins");
			ResetBoard();
		}
	}
}