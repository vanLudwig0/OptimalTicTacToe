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

	public static T Random<T>(this IEnumerable<T> source)
	{
		return source.Skip(_random.Next(source.Count())).First();
	}

	public static IEnumerable<T> Random<T>(this IEnumerable<T> source, int length)
	{
		List<T> src = source.ToList();

		for (int i = 0; i < length; i++)
		{
			T ret = src.Random();
			src.Remove(ret);
			yield return ret;
		}
	}
}