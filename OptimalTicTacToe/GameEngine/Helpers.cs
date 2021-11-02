using OptimalTicTacToe.GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public static class EnumerableExtension
{
	private static Random _random = new Random();
	public static T RandomOrDefault<T>(this IEnumerable<T> source)
	{
		return source.Skip(_random.Next(source.Count())).FirstOrDefault();
	}
}