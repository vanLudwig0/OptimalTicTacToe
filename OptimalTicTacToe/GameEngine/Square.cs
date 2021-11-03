using Xamarin.Forms;

namespace OptimalTicTacToe.GameEngine
{
	public abstract class Square
	{
		public abstract string Value { get; set; }

		public bool Empty => string.IsNullOrEmpty(Value);
		public bool X => Value == "X";
		public bool O => Value == "O";

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
		public SquareButtonImpl(Button binding)
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
