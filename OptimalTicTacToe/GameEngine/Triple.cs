using System;
using System.Collections;
using System.Collections.Generic;

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
	}
}
