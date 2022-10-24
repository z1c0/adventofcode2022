using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace aoc
{
	public class Grid<T> where T : struct
	{
		public Grid(int width, int height)
		{
			Width = width;
			Height = height;
			_data = new T[Height, Width];
		}

		public void Print()
		{
			var sb = new StringBuilder();
			PrintTo(sb);
			Console.Write(sb);
		}

		public void ForEach(Action<(int X, int Y)> callback)
		{
			for (var y = 0; y < Height; y++)
			{
				for (var x = 0; x < Width; x++)
				{
					callback((x, y));
				}
			}
		}

		public Grid<T> Resize(int newWidth, int newHeight)
		{
			var newGrid = new Grid<T>(newWidth, newHeight);
			ForEach(p => {
				if (newGrid.IsInBounds(p)) 
				{
					newGrid[p] = this[p];
				}
			});
			return newGrid;
		}

		public int Size => Width * Height;

		public override string ToString()
		{
			var sb = new StringBuilder();
			PrintTo(sb);
			return sb.ToString();
		}

		public void PrintTo(StringBuilder sb)
		{
			for (var y = 0; y < Height; y++)
			{
				for (var x = 0; x < Width; x++)
				{
					sb.Append(_data[y, x]);
				}
				sb.AppendLine();
			}
		}

		public override bool Equals(object? obj)
		{
			if (obj is not Grid<T> other || Width != other.Width || Height != other.Height)
			{
				return false;
			}
			for (var y = 0; y < Height; y++)
			{
				for (var x = 0; x < Width; x++)
				{
					if (!this[x, y].Equals(other[x, y]))
					{
						return false;
					}
				}
			}
			return true;
		}

		public override int GetHashCode() => (Width, Height).GetHashCode();

		public Grid<T> Clone()
		{
			var clone = new Grid<T>(Width, Height);
			for (var y = 0; y < Height; y++)
			{
				for (var x = 0; x < Width; x++)
				{
					clone._data[y, x] = _data[y, x];
				}
			}
			return clone;
		}

		public Dictionary<T, int> CountDistinct()
		{
			var counts = new Dictionary<T, int>();
			for (var y = 0; y < Height; y++)
			{
				for (var x = 0; x < Width; x++)
				{
					var c = _data[y, x];
					if (!counts.TryAdd(c, 1))
					{
						counts[c]++;
					}
				}
			}
			return counts;
		}

		public (int X, int Y) Find(T t)
		{
			var all = FindAll(t);
			return all.Any() ? all.First() : (-1, -1);
		}

		public IEnumerable<(int X, int Y)> FindAll(T t)
		{
			return FindAll((x, y) => this[(x, y)].Equals(t));
		}

		public IEnumerable<(int X, int Y)> FindAll(Func<int, int, bool> predicate)
		{
			for (var y = 0; y < Height; y++)
			{
				for (var x = 0; x < Width; x++)
				{
					if (predicate(x, y))
					{
						yield return (x, y);
					}
				}
			}
		}

		public void Fill(T t, Func<int, int, bool>? predicate = null)
		{
			for (var y = 0; y < Height; y++)
			{
				for (var x = 0; x < Width; x++)
				{
					if (predicate == null || predicate(x, y))
					{
						_data[y, x] = t;
					}
				}
			}
		}

		public int Count(Func<T, bool> func)
		{
			var count = 0;
			for (var y = 0; y < Height; y++)
			{
				for (var x = 0; x < Width; x++)
				{
					if (func(_data[y, x]))
					{
						count++;
					}
				}
			}
			return count;
		}

		public IEnumerable<(int X, int Y)> GetAdjacent4((int X, int Y) p)
		{
			return GetAdjacent4(p.X, p.Y);
		}

		public IEnumerable<(int X, int Y)> GetAdjacent4(int x, int y)
		{
			if (IsInBounds(x - 1, y))
			{
				yield return (x - 1, y);
			}
			if (IsInBounds(x + 1, y))
			{
				yield return (x + 1, y);
			}
			if (IsInBounds(x, y - 1))
			{
				yield return (x, y - 1);
			}
			if (IsInBounds(x, y + 1))
			{
				yield return (x, y + 1);
			}
		}

		public IEnumerable<(int X, int Y)> GetAdjacent8((int X, int Y) p)
		{
			return GetAdjacent8(p.X, p.Y);
		}

		public IEnumerable<(int X, int Y)> GetAdjacent8(int x, int y)
		{
			foreach (var a in GetAdjacent4(x, y))
			{
				yield return a;
			}
			if (IsInBounds(x - 1, y - 1))
			{
				yield return (x - 1, y - 1);
			}
			if (IsInBounds(x + 1, y - 1))
			{
				yield return (x + 1, y - 1);
			}
			if (IsInBounds(x - 1, y + 1))
			{
				yield return (x - 1, y + 1);
			}
			if (IsInBounds(x + 1, y + 1))
			{
				yield return (x + 1, y + 1);
			}
		}

		public Dictionary<T, int> CountAdjacent4Distinct(int x, int y)
		{
			return GetAdjacentDistinct(x, y, false);
		}

		public Dictionary<T, int> CountAdjacent8Distinct(int x, int y)
		{
			return GetAdjacentDistinct(x, y, true);
		}

		private Dictionary<T, int> GetAdjacentDistinct(int x, int y, bool all)
		{
			var adjacents = new Dictionary<T, int>();
			void CheckThenAdd(int x, int y)
			{
				if (IsInBounds(x, y))
				{
					var c = _data[y, x];
					if (!adjacents.TryAdd(c, 1))
					{
						adjacents[c]++;
					}
				}
			}
			CheckThenAdd(x - 1, y);
			CheckThenAdd(x + 1, y);
			CheckThenAdd(x, y - 1);
			CheckThenAdd(x, y + 1);
			if (all)
			{
				CheckThenAdd(x - 1, y - 1);
				CheckThenAdd(x + 1, y - 1);
				CheckThenAdd(x - 1, y + 1);
				CheckThenAdd(x + 1, y + 1);
			}
			return adjacents;
		}

		public int CountAdjacent8(int x, int y, T t)
		{
			var counts = CountAdjacent8Distinct(x, y);
			counts.TryGetValue(t, out var count);
			return count;
		}

		public int CountAdjacent4(int x, int y, T t)
		{
			var counts = CountAdjacent4Distinct(x, y);
			counts.TryGetValue(t, out var count);
			return count;
		}

		public bool IsInBounds((int X, int Y) p) => IsInBounds(p.X, p.Y);

		public bool IsInBounds(int x, int y)
		{
			return x >= 0 && y >= 0 && x < Width && y < Height;
		}

		public int Count(T t)
		{
			var counts = CountDistinct();
			counts.TryGetValue(t, out var count);
			return count;
		}

		public T this[(int X, int Y) p]
		{
			get => this[p.X, p.Y];
			set => this[p.X, p.Y] = value;
		}

		public T this[int x, int y]
		{
			get => _data[y, x];
			set => _data[y, x] = value;
		}

		public int Width { get; }
		public int Height { get; }
		private readonly T[,] _data;
	}

	public static partial class Input
	{
		public static Grid<int> ReadIntGrid(string fileName = "input.txt") => ParseIntGrid(File.ReadAllLines(fileName));
		public static Grid<int> ParseIntGrid(IEnumerable<string> lines) => ParseGrid(lines, c => int.Parse(c.ToString()));

		public static Grid<char> ReadCharGrid(string fileName = "input.txt") => ParseCharGrid(File.ReadAllLines(fileName));
		public static Grid<char> ParseCharGrid(IEnumerable<string> lines) => ParseGrid(lines, c => c);

		private static Grid<T> ParseGrid<T>(IEnumerable<string> lines, Func<char, T> converter) where T : struct
		{
			var h = lines.Count();
			var w = lines.First().Length;
			var grid = new Grid<T>(w, h);
			for (var y = 0; y < h; y++)
			{
				for (var x = 0; x < w; x++)
				{
					grid[x, y] = converter(lines.ElementAt(y)[x]);
				}
			}
			return grid;
		}
	}
}
