using System;
using System.Collections.Generic;
using System.Text;

namespace OptimalTicTacToe.GameEngine
{
	public class Square
	{
		public string Value { get; set; } = "O";

		public bool Empty => Value == "";
		public bool X => Value == "X";
		public bool O => Value == "O";
	}
}
