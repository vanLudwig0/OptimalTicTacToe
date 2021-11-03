using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OptimalTicTacToe.GameEngine
{
	//Triples are any combination of three squares that would win the game.  So, it could be any of the rows, any of the columns, or either of the diagonals
	public class Triple : IEnumerable<Square>
	{
		private readonly Square[] _square = new Square[] { null, null, null };
		public Triple(Square end1, Square middle, Square end2)
		{
			_square[0] = end1;
			_square[1] = middle;
			_square[2] = end2;
		}

		public Square this[int index] => _square[index];

		public IEnumerator<Square> GetEnumerator()
		{
			yield return _square[0];
			yield return _square[1];
			yield return _square[2];
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public bool AllX() => _square.All(s => s.X);
		public bool AllO() => _square.All(s => s.O);
		public bool AllEmpty() => _square.All(s => s.Empty);
		public bool AnyX() => _square.Any(s => s.X);
		public bool AnyO() => _square.Any(s => s.O);
		public bool AnyEmpty() => _square.Any(s => s.Empty);
		public bool WinnableX() => CountX() == 2 && CountEmpty() == 1;
		public bool WinnableO() => CountO() == 2 && CountEmpty() == 1;

		public bool Mixed() => _square.Any(s => s.X) && _square.Any(s => s.O);

		public int CountX() => _square.Count(s => s.X);
		public int CountO() => _square.Count(s => s.O);
		public int CountEmpty() => _square.Count(s => s.Empty);

		public IEnumerable<Square> EmptySquares() => _square.Where(s => s.Empty);
		public IEnumerable<Square> XSquares() => _square.Where(s => s.X);
		public IEnumerable<Square> OSquares() => _square.Where(s => s.O);

		public bool Matches(string regex)
		{
			System.Text.RegularExpressions.Regex R = new System.Text.RegularExpressions.Regex(regex);
			return R.IsMatch(ToString());
		}

		public override string ToString()
		{
			return string.Join("", _square.Select(s => s.ToString()));
		}
	}
}
