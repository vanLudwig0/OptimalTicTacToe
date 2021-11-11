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
		public static int DefenseWinCount { get; internal set; } = 0;
		public static int DefenseBestWinCount { get; internal set; } = 0;

		public DefensePage()
		{
			InitializeComponent();

			CurrentStreakLabel.FormattedText.Spans[1].Text = DefenseWinCount.ToString() + (DefenseWinCount == 1 ? " Tie" : " Ties");
			RecordLabel.FormattedText.Spans[1].Text = DefenseBestWinCount.ToString() + (DefenseBestWinCount == 1 ? " Tie" : " Ties");

			GameBoard = new GameEngine.ButtonImpl.BoardButtonImpl(S00, S01, S02, S10, S11, S12, S20, S21, S22);
			ResetBoard();
		}

		private void ResetBoard()
		{
			GameBoard.Clear();
			GameBoard.EmptyOutsides.RandomOrDefault().Value = "X";
			RandomBlock();
		}

		//Unblock all squares after the first move
		private void UnBlock()
		{
			S00.IsEnabled = S01.IsEnabled = S02.IsEnabled = true;
			S10.IsEnabled = S11.IsEnabled = S12.IsEnabled = true;
			S20.IsEnabled = S21.IsEnabled = S22.IsEnabled = true;

			S00.BackgroundColor = S01.BackgroundColor = S02.BackgroundColor = Color.Transparent;
			S10.BackgroundColor = S11.BackgroundColor = S12.BackgroundColor = Color.Transparent;
			S20.BackgroundColor = S21.BackgroundColor = S22.BackgroundColor = Color.Transparent;
		}

		//Set a few squares disable at startup to make things more interesting
		private readonly Random _random = new Random();
		private void RandomBlock()
		{
			if (GameBoard.EmptySquares.Count() != 8) return;
			var xSquare = GameBoard.XSquares.FirstOrDefault(s => s.Edge);

			var blockables = (xSquare == null ? GameBoard.Outsides.Where(s => s.Empty) : GameBoard.EmptySquares).Select(s => s as OptimalTicTacToe.GameEngine.ButtonImpl.SquareButtonImpl);
			blockables = blockables.Random(_random.Next(4));

			foreach (var block in blockables)
			{
				block.OwningButton.IsEnabled = false;
				block.OwningButton.BackgroundColor = Color.LightPink;
			}
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
					UnBlock();

					//O wins
					GameEngine.Triple win = GameBoard.Triples.FirstOrDefault(triple => triple.AllO());
					if (win != null)
					{
						await GameOver(true);
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
			//Special case for first x in a corner
			if (GameBoard.Matches("X__/_O_/___")) return GameBoard.Diagonals.First(t => t.Matches("XO_")).EmptySquares().First();
			
			List<GameEngine.Square> ret = new List<GameEngine.Square>();

			//Find all the triples with only Xs.  These must only have one X or else it would have already been detected as a winning move.
			var XTriples = GameBoard.Triples.Where(t => t.AnyX() && !t.AnyO()).ToList();

			//If two of the x-triples share a square, that square will create a winning fork (i.e. game over the next x move)
			Parallel.For (1, XTriples.Count, i =>
			{
				for (int j = i - 1; j >= 0; j--)
				{
					var intersection = XTriples[i].Intersection(XTriples[j]);
					if (intersection?.Empty == true) lock (ret) { ret.Add(intersection); }
				}
			});

			if (ret.Count > 0) return ret.RandomOrDefault();


			//Find all the triples with only Os.  These must only have one O or else it would have already been detected as a needed block.
			var OTriples = GameBoard.Triples.Where(t => t.AnyO() && !t.AnyX()).ToList();

			//If two of the o-triples share a square, that square will create a losing fork (i.e. game over the next o move)
			Parallel.For(1, OTriples.Count, i =>
			{
				for (int j = i - 1; j >= 0; j--)
				{
					var intersection = OTriples[i].Intersection(OTriples[j]);
					if (intersection?.Empty == true) lock (ret) { ret.Add(intersection); }
				}
			});

			if (ret.Count > 0) return ret.RandomOrDefault();


			//Look for a forceable fork (i.e. game over in two x moves)
			Parallel.ForEach(XTriples, xTriple =>
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

								lock (ret) { ret.Add(potentialNextMove); }
							}
						}
					}
				}
			});

			if (ret.Count > 0) return ret.RandomOrDefault();

			//Pick the square with the most remaining ways to win and hope for the best
			return GameBoard.EmptySquares.OrderByDescending(s => GameBoard.ContainingTriples(s).Where(t => !t.AnyO()).Count()).First();
		}

		private async Task GameOver(bool positiveResult)
		{
			if (positiveResult)
			{
				DefenseWinCount++;
				DefenseBestWinCount = Math.Max(DefenseBestWinCount, DefenseWinCount);
				await Task.Delay(350);
			}
			else
			{
				DefenseWinCount = 0;
				await Task.Delay(750);
			}

			CurrentStreakLabel.FormattedText.Spans[1].Text = DefenseWinCount.ToString() + (DefenseWinCount == 1 ? " Tie" : " Ties");
			RecordLabel.FormattedText.Spans[1].Text = DefenseBestWinCount.ToString() + (DefenseBestWinCount == 1 ? " Tie" : " Ties");
			ResetBoard();
		}
	}
}