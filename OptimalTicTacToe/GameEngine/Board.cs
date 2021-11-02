using System.Collections.Generic;
using System.Linq;

namespace OptimalTicTacToe.GameEngine
{
	public class Board
	{
		//Remember Square is indexed by [row, column]
		public readonly Square[,] Square = new Square[3, 3] 
		{ 
			{ new Square(), new Square(), new Square() }, 
			{ new Square(), new Square(), new Square() }, 
			{ new Square(), new Square(), new Square() } 
		};

		//We can't use Lazy<> here, but these are easy to implement without it
		private Triple[] _diag = null, _rows = null, _cols = null;
		public Triple[] Diagonals => _diag ?? (_diag = new Triple[] 
		{
			new Triple(Square[0, 0], Square[1, 1], Square[2, 2]),
			new Triple(Square[0, 2], Square[1, 1], Square[2, 0])
		});

		public Triple[] Rows => _rows ?? (_rows = new Triple[]
		{
			new Triple(Square[0, 0], Square[0, 1], Square[0, 2]),
			new Triple(Square[1, 0], Square[1, 1], Square[1, 2]),
			new Triple(Square[2, 0], Square[2, 1], Square[2, 2])
		});

		public Triple[] Columns => _cols ?? (_cols = new Triple[]
		{
			new Triple(Square[0, 0], Square[1, 0], Square[2, 0]),
			new Triple(Square[0, 1], Square[1, 1], Square[2, 1]),
			new Triple(Square[0, 2], Square[1, 2], Square[2, 2])
		});

		//Do the same for most useful categories of Squares
		private Square[] _squares = null, _corners = null, _edges = null, _centers = null;
		public Square[] Squares => _squares ?? (_squares = new Square[]
		{
			Square[0, 0],
			Square[1, 0],
			Square[2, 0],
			Square[0, 1],
			Square[1, 1],
			Square[2, 1],
			Square[0, 2],
			Square[1, 2],
			Square[2, 2]
		});

		public Square[] Corners => _corners ?? (_corners = new Square[]
		{
			Square[0, 0],
			Square[2, 0],
			Square[0, 2],
			Square[2, 2]
		});

		public Square[] Edges => _edges ?? (_edges = new Square[]
		{
			Square[0, 1],
			Square[1, 0],
			Square[1, 2],
			Square[2, 1]
		});

		public Square[] Centers => _centers ?? (_centers = new Square[]
		{
			Square[1, 1]
		});


		public IEnumerable<Square> EmptySquares => Squares.Where(s => s.Empty);
		public IEnumerable<Square> XSquares => Squares.Where(s => s.X);
		public IEnumerable<Square> OSquares => Squares.Where(s => s.O);

		public IEnumerable<Triple> Triples => Rows.Concat(Columns).Concat(Diagonals);
	}
}
