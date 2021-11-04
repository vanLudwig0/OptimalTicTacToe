using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OptimalTicTacToe
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DefensePage : ContentPage
	{
		public GameEngine.Board GameBoard { get; }
		private int _winCount = 0;
		private int _bestWinCount = 0;

		public DefensePage()
		{
			InitializeComponent();

			GameBoard = new GameEngine.ButtonImpl.BoardButtonImpl(S00, S01, S02, S10, S11, S12, S20, S21, S22);
			ResetBoard();
		}

		private readonly Random _random = new Random();
		private void ResetBoard()
		{
			GameBoard.Clear();
			var picked = GameBoard.EmptyOutsides.RandomOrDefault();
			picked.Value = "X";
		}

		private void BlockCenter(bool value)
		{
			S11.IsEnabled = !value;
			S11.BackgroundColor = value ? Color.LightPink : Color.Transparent;
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
					button.Text = "O";

					//O wins
					GameEngine.Triple win = GameBoard.Triples.FirstOrDefault(triple => triple.AllO());
					if (win != null)
					{
						await GameOver(true);
						return;
					}

					await Task.Delay(350);
					BlockCenter(false);
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
			//X wins
			var winnable = GameBoard.WinnableX().RandomOrDefault();
			if (winnable != null)
			{
				winnable.First(s => s.Empty).Value = "X";
				await GameOver(false);
				return;
			}

			//X ties (considered success)
			var last = GameBoard.EmptySquares;
			if (last.Count() == 1)
			{
				last.First().Value = "X";
				await GameOver(true);
				return;
			}

			//Block an O win
			var loseable = GameBoard.WinnableO().RandomOrDefault();
			if (loseable != null)
			{
				loseable.First(s => s.Empty).Value = "X";
				return;
			}

			RulesEngine().Value = "X";			
			return;
		}

		private GameEngine.Square RulesEngine()
		{
			//Special cases for first x in a corner
			if (GameBoard.Matches("X__/_O_/___")) return GameBoard.Diagonals.First(t => t.Matches("XO_")).EmptySquares().First();
			if (GameBoard.Matches("X__/_O_/__X")) return GameBoard.Corners.Where(s => s.Empty).RandomOrDefault();


			List<GameEngine.Square> ret = new List<GameEngine.Square>();

			//Find all the triples with only Xs.  These must only have one X or else it would have already been detected as a winning move.
			var XTriples = GameBoard.Triples.Where(t => t.AnyX() && !t.AnyO()).ToList();

			//If two of the x-triples share a square, that square will create a winning fork (i.e. game over the next x move)
			for (int i = XTriples.Count - 1; i > 0; i--)
			{
				for (int j = i - 1; j >= 0; j--)
				{
					var intersection = XTriples[i].Intersection(XTriples[j]);
					if (intersection?.Empty == true) ret.Add(intersection);
				}
			}

			if (ret.Count > 0) return ret.RandomOrDefault();


			//Look for a forceable fork (i.e. game over in two x moves)
			foreach (var xTriple in XTriples)
			{
				var xSquare = xTriple.XSquares().First();

				//If we put an x in either of xTriple's empties, o will be forced to go in the other empty.  Use that to our advantage
				foreach (var potentialNextMove in xTriple.EmptySquares())
				{
					//Careful if we're forcing o to block into a something we'll have to block
					var forcedOMove = xTriple.Where(s => !ReferenceEquals(s, xSquare) && !ReferenceEquals(s, potentialNextMove)).First();
					var dangerousTriples = GameBoard.ContainingTriples(forcedOMove).Where(t => t.AnyO() && !t.AnyX());      
					var dangerousSquares = dangerousTriples.SelectMany(t => t.Where(s => s.Empty && !ReferenceEquals(s, forcedOMove)));
					if (dangerousSquares.Count() > 1) continue;
					var dangerousSquare = dangerousSquares.FirstOrDefault();


					foreach (var potentialWinningTriple in GameBoard.ContainingTriples(potentialNextMove).Where(t => t.AllEmpty()))
					{
						foreach (var potentialFork in potentialWinningTriple.Where(s => !ReferenceEquals(s, potentialNextMove)))
						{
							var otherPotentialWinningTriple = GameBoard.ContainingTriple(xSquare, potentialFork);
							if (otherPotentialWinningTriple != null)
							{
								//We won't be able to fork if we have to block elsewhere
								if (dangerousSquare?.Equals(potentialFork) == false) continue;

								ret.Add(potentialNextMove);
							}
						}
					}
				}
			}

			if (ret.Count > 0) return ret.RandomOrDefault();

			return GameBoard.EmptySquares.RandomOrDefault();
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

			CurrentStreakLabel.FormattedText.Spans[1].Text = _winCount.ToString() + (_winCount == 1 ? " Tie" : " Ties");
			RecordLabel.FormattedText.Spans[1].Text = _bestWinCount.ToString() + (_bestWinCount == 1 ? " Tie" : " Ties");
			ResetBoard();
		}
	}
}