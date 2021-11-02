using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace OptimalTicTacToe.GameEngine
{
	public class Square
	{
		#region HACK
		private readonly Button _binding = null;
		public Square (Button binding)
		{
			_binding = binding;
		}

		public string Value 
		{
			get => _binding.Text;
			set => _binding.Text = value;
		}
		#endregion

		
		public bool Empty => string.IsNullOrEmpty(Value);
		public bool X => Value == "X";
		public bool O => Value == "O";

		public override string ToString()
		{
			return X ? " X " : O ? " O " : Empty ? " ▪ " : " # ";
		}
	}
}
