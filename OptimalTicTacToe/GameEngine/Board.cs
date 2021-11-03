using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace OptimalTicTacToe.GameEngine
{
	public class Board
	{
		#region HACK
		public Board(Button S00, Button S01, Button S02, Button S10, Button S11, Button S12, Button S20, Button S21, Button S22)
		{
			this.Square = new Square[3, 3]
			{
				{ new Square(S00), new Square(S01), new Square(S02) },
				{ new Square(S10), new Square(S11), new Square(S12) },
				{ new Square(S20), new Square(S21), new Square(S22) }
			};
		}
		#endregion

		//Remember Square is indexed by [row, column]
		public readonly Square[,] Square;

		//We can't use Lazy<> here, but these are easy to implement without it
		private Triple[] _diag = null, _rows = null, _cols = null, _mid = null;
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

		public Triple[] Middles => _mid ?? (_mid = new Triple[]
		{
			new Triple(Square[1, 0], Square[1, 1], Square[1, 2]),
			new Triple(Square[0, 1], Square[1, 1], Square[2, 1])
		});

		//Do the same for most useful categories of Squares
		private Square[] _squares = null, _corners = null, _edges = null, _centers = null;
		public Square[] Squares => _squares ?? (_squares = new Square[]
		{
			Square[0, 0],
			Square[0, 1],
			Square[0, 2],
			Square[1, 0],
			Square[1, 1],
			Square[1, 2],
			Square[2, 0],
			Square[2, 1],
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

		public IEnumerable<Square> Outsides => Corners.Concat(Edges);
		public IEnumerable<Square> EmptyOutsides => Outsides.Where(s => s.Empty);

		public IEnumerable<Square> EmptySquares => Squares.Where(s => s.Empty);
		public IEnumerable<Square> XSquares => Squares.Where(s => s.X);
		public IEnumerable<Square> OSquares => Squares.Where(s => s.O);

		public IEnumerable<Triple> Triples => Rows.Concat(Columns).Concat(Diagonals);
		public IEnumerable<Triple> WinnableX() => Triples.Where(t => t.WinnableX());
		public IEnumerable<Triple> WinnableO() => Triples.Where(t => t.WinnableO());
		public IEnumerable<Triple> Mixeds() => Triples.Where(t => t.Mixed());
		public IEnumerable<Triple> Matches(string regex) => Triples.Where(t => t.Matches(regex));

		public void Clear()
		{
			foreach (Square s in Squares) s.Value = "";
		}

		public override string ToString()
		{
			return string.Join("/", Rows.Select(r => r.ToString()));
		}
	}
}
