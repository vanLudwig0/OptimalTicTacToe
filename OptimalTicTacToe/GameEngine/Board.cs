using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using OptimalTicTacToe.GameEngine.ButtonImpl;

namespace OptimalTicTacToe.GameEngine
{
	public abstract class Board
	{
		//Remember Square is indexed by [row, column]
		public Square[,] Square;

		//We can't use Lazy<> here, but these are easy to implement without it
		private Triple[] _diag = null, _rows = null, _cols = null, _mid = null;
		public Triple[] Diagonals => _diag ?? (_diag = new Triple[]
		{
			new Triple(Square[0, 0], Square[1, 1], Square[2, 2], diagonal: true),
			new Triple(Square[0, 2], Square[1, 1], Square[2, 0], diagonal: true)
		});

		public Triple[] Rows => _rows ?? (_rows = new Triple[]
		{
			new Triple(Square[0, 0], Square[0, 1], Square[0, 2], row: true),
			new Triple(Square[1, 0], Square[1, 1], Square[1, 2], row: true),
			new Triple(Square[2, 0], Square[2, 1], Square[2, 2], row: true)
		});

		public Triple[] Columns => _cols ?? (_cols = new Triple[]
		{
			new Triple(Square[0, 0], Square[1, 0], Square[2, 0], column: true),
			new Triple(Square[0, 1], Square[1, 1], Square[2, 1], column: true),
			new Triple(Square[0, 2], Square[1, 2], Square[2, 2], column: true)
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

		//Useful Square queries
		public IEnumerable<Square> Outsides => Corners.Concat(Edges);
		public IEnumerable<Square> EmptyOutsides => Outsides.Where(s => s.Empty);

		public IEnumerable<Square> EmptySquares => Squares.Where(s => s.Empty);
		public IEnumerable<Square> XSquares => Squares.Where(s => s.X);
		public IEnumerable<Square> OSquares => Squares.Where(s => s.O);

		//Useful Triple queries
		public IEnumerable<Triple> Triples => Rows.Concat(Columns).Concat(Diagonals);
		public IEnumerable<Triple> WinnableX() => Triples.Where(t => t.WinnableX());
		public IEnumerable<Triple> WinnableO() => Triples.Where(t => t.WinnableO());
		public IEnumerable<Triple> Mixeds() => Triples.Where(t => t.Mixed());
		public IEnumerable<Triple> MatchingTriples(string pattern) => Triples.Where(t => t.Matches(pattern));
		public IEnumerable<Triple> ContainingTriples(Square square) => Triples.Where(t => t.Any(s => ReferenceEquals(square, s)));
		public Triple ContainingTriple(Square s1, Square s2) => Triples.FirstOrDefault(t => t.Any(s => ReferenceEquals(s1, s)) && t.Any(s => ReferenceEquals(s2, s)));

		//Check if this matches the specified pattern
		public bool Matches(string pattern, bool includingTransforms = true)
		{
			if (pattern == ToString()) return true;
			if (!includingTransforms) return false;

			if (pattern == string.Join("/", Rows.Select(r => r.ToReversedString()))) return true;       //Horizontal flip
			if (pattern == string.Join("/", Rows.Reverse().Select(r => r.ToString()))) return true;     //Vertical flip
			if (pattern == string.Join("/", Rows.Reverse().Select(r => r.ToReversedString()))) return true;     //Horizontal and Vertical flip

			if (pattern == string.Join("/", Columns.Select(c => c.ToString()))) return true;       //Transpose
			if (pattern == string.Join("/", Columns.Select(c => c.ToReversedString()))) return true;       //Transpose Horizontal flip
			if (pattern == string.Join("/", Columns.Reverse().Select(c => c.ToString()))) return true;     //Transpose Vertical flip
			if (pattern == string.Join("/", Columns.Reverse().Select(c => c.ToReversedString()))) return true;     //Transpose Horizontal and Vertical flip

			return false;
		}

		//Set all Squares back to ""
		public void Clear()
		{
			foreach (Square s in Squares) s.Value = "";
		}

		//Remember Matches(...) relies on ToString, so cannot change it arbitrarily
		public override string ToString()
		{
			return string.Join("/", Rows.Select(r => r.ToString()));
		}
	}
}

namespace OptimalTicTacToe.GameEngine.ButtonImpl
{ 
	public class BoardButtonImpl : Board
	{
		public BoardButtonImpl(Button S00, Button S01, Button S02, Button S10, Button S11, Button S12, Button S20, Button S21, Button S22)
		{
			this.Square = new Square[3, 3]
			{
				{ new SquareButtonImpl(S00, 0, 0), new SquareButtonImpl(S01, 0, 1), new SquareButtonImpl(S02, 0, 2) },
				{ new SquareButtonImpl(S10, 1, 0), new SquareButtonImpl(S11, 1, 1), new SquareButtonImpl(S12, 1, 2) },
				{ new SquareButtonImpl(S20, 2, 0), new SquareButtonImpl(S21, 2, 1), new SquareButtonImpl(S22, 2, 2) }
			};
		}
	}
}
