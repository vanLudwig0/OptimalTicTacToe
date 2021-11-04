using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace OptimalTicTacToe.GameEngine
{
	public abstract class Square
	{
		public Square(int row, int column)
		{
			Row = row;
			Column = column;
		}

		public abstract string Value { get; set; }

		public bool Empty => string.IsNullOrEmpty(Value);
		public bool X => Value == "X";
		public bool O => Value == "O";

		public int Row { get; }
		public int Column { get; }
		//public Square Opposite { get; internal set; }

		public bool Corner => Row != 1 && Column != 1;
		public bool Center => Row == 1 && Column == 1;
		public bool Edge => !Corner && !Center;

		//Remember Matches(...) relies on ToString, so cannot change it arbitrarily
		public override string ToString()
		{
			return X ? "X" : O ? "O" : Empty ? "_" : "#";
		}
	}
}

namespace OptimalTicTacToe.GameEngine.ButtonImpl
{
	public class SquareButtonImpl : Square
	{
		private readonly Button _binding = null;
		public SquareButtonImpl(Button binding, int row, int column) : base(row, column)
		{
			_binding = binding;
		}

		public override string Value
		{
			get => _binding.Text;
			set => _binding.Text = value;
		}
	}
}
