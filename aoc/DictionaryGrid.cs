using System.Text;

namespace aoc
{
	public class DictionaryGrid<T> where T : struct
	{
		private readonly Dictionary<(int x, int y), T> _dict = new();

		public DictionaryGrid(Grid<T> grid)
		{
			grid.ForEach(p => this[p] = grid[p]);
		}

		public void Print()
		{
			var sb = new StringBuilder();
			PrintTo(sb);
			Console.Write(sb);
		}

		public int GetMinX() => _dict.Min(e => e.Key.x);
		public int GetMinY() => _dict.Min(e => e.Key.y);
		public int GetMaxX() => _dict.Max(e => e.Key.x);
		public int GetMaxY() => _dict.Max(e => e.Key.y);

		public IEnumerable<KeyValuePair<(int x, int y), T>> FindAll(T t)
		{
			return FindAll(e => e.Value.Equals(t));
		}

		public IEnumerable<KeyValuePair<(int x, int y), T>> FindAll(Func<KeyValuePair<(int x, int y), T>, bool> predicate)
		{
			foreach (var e in _dict)
			{
				if (predicate(e)) 
				{
					yield return e;
				}
			};
		}

		public void ForEach(Action<KeyValuePair<(int x, int y), T>> callback)
		{
			foreach (var e in _dict)
			{
				callback(e);
			}
		}

		public T this[(int X, int Y) p]
		{
			get => _dict[p];
			set => _dict[p] = value;
		}

		public T this[int x, int y]
		{
			get => this[(x, y)];
			set => this[(x, y)] = value;
		}

		public IEnumerable<KeyValuePair<(int x, int y), T>> GetAdjacent8((int x, int y) p, bool getOrCreate = false, T createWith = default)
			=> GetAdjacent8(p.x, p.y, getOrCreate, createWith);
		public IEnumerable<KeyValuePair<(int x, int y), T>> GetAdjacent8(int x, int y, bool getOrCreate = false, T createWith = default)
		{
			var cells = new[]
			{
				(x - 1, y - 1), // NW
				(x    , y - 1), // N
				(x + 1, y - 1), // NE
				(x - 1, y    ), // W
				(x + 1, y    ), // E
				(x - 1, y + 1), // SW
				(x    , y + 1), // S
				(x + 1, y + 1), // SE
			};
			foreach (var c in cells)
			{
				if (getOrCreate)
				{
					yield return GetOrCreate(c, createWith);
				}
				else
				{
					if (Exists(c))
					{
						yield return Get(c);
					}
				}
			}
		}

		public IEnumerable<KeyValuePair<(int x, int y), T>> GetAdjacent4((int x, int y) p, bool getOrCreate = false, T createWith = default)
			=> GetAdjacent4(p.x, p.y, getOrCreate, createWith);
		public IEnumerable<KeyValuePair<(int x, int y), T>> GetAdjacent4(int x, int y, bool getOrCreate = false, T createWith = default)
		{
			var cells = new[]
			{
				(x    , y - 1), // N
				(x - 1, y    ), // W
				(x + 1, y    ), // E
				(x    , y + 1), // S
			};
			foreach (var c in cells)
			{
				if (getOrCreate)
				{
					yield return GetOrCreate(c, createWith);
				}
				else
				{
					if (Exists(c))
					{
						yield return Get(c);
					}
				}
			}
		}
		public KeyValuePair<(int x, int y), T>  Get(int x, int y) => Get((x, y));
		public KeyValuePair<(int x, int y), T> Get((int x, int y) p) => new(p, _dict[p]);

		public KeyValuePair<(int x, int y), T>?  TryGet(int x, int y) => TryGet((x, y));
		public KeyValuePair<(int x, int y), T>? TryGet((int x, int y) p) => _dict.ContainsKey(p) ? new(p, _dict[p]) : null;
		public KeyValuePair<(int x, int y), T>  GetOrCreate(int x, int y, T createWith = default) => GetOrCreate((x, y), createWith);
		public KeyValuePair<(int x, int y), T> GetOrCreate((int x, int y) p, T createWith = default)
		{
			if (!_dict.ContainsKey(p))
			{
				_dict.Add(p, createWith);
			}
			return Get(p);
		}

		public bool Exists(int x, int y) => Exists((x, y));
		public bool Exists((int x, int y) p) => _dict.ContainsKey(p);
		
		public void PrintTo(StringBuilder sb)
		{
			var minX = GetMinX();
			var maxX = GetMaxX();
			var minY = GetMinY();
			var maxY = GetMaxY();
			for (var y = minY; y <= maxY; y++)
			{
				for (var x = minX; x <= maxX; x++)
				{
					var p = (x, y);
					var o = " ";
					if (_dict.TryGetValue(p, out T value))
					{
						o = value.ToString();
					}
					sb.Append(o);
				}
				sb.AppendLine();
			}
		}
	}

	public static partial class Input
	{
		public static DictionaryGrid<char> ReadCharDictionaryGrid(string fileName = Input.DEFAULT_INPUT_FILENAME)
			=> new(ReadCharGrid(fileName));
	}
}