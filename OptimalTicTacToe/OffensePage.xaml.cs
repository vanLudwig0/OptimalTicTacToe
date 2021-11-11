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
		public static int OffenseWinCount { get; internal set; } = 0;
		public static int OffenseBestWinCount { get; internal set; } = 0;

		public OffensePage()
		{
			InitializeComponent();

			CurrentStreakLabel.FormattedText.Spans[1].Text = OffenseWinCount.ToString() + (OffenseWinCount == 1 ? " Win" : " Wins");
			RecordLabel.FormattedText.Spans[1].Text = OffenseBestWinCount.ToString() + (OffenseBestWinCount == 1 ? " Win" : " Wins");

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
				OffenseWinCount++;
				OffenseBestWinCount = Math.Max(OffenseBestWinCount, OffenseWinCount);
				await Task.Delay(350);
			}
			else
			{
				OffenseWinCount = 0;
				await Task.Delay(750);
			}

			CurrentStreakLabel.FormattedText.Spans[1].Text = OffenseWinCount.ToString() + (OffenseWinCount == 1 ? " Win" : " Wins");
			RecordLabel.FormattedText.Spans[1].Text = OffenseBestWinCount.ToString() + (OffenseBestWinCount == 1 ? " Win" : " Wins");
			ResetBoard();
		}
	}
}