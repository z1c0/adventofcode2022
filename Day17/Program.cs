using aoc;

Day.Run(() =>
{
	Part1();
	Part2();
});

static void Part1()
{
	var height = CalculateHeight(2022);
	Console.WriteLine($"The tower is {height} units tall.");
}

static void Part2()
{
	const int periodHeight = 2702;
	const int periodRounds = 1725;
	var height = periodHeight * (1000000000000L / periodRounds);
	height += CalculateHeight(1000000000000L % periodRounds);
	Console.WriteLine($"The tower is {height} units tall.");
}

static int CalculateHeight(long rounds)
{
	var pieces = new RingAccess<char[,]>(new List<char[,]>() {
		new char[,] { { '#', '#', '#', '#' } },
		new char[,] {
			{ '.', '#', '.' },
			{ '#', '#', '#' },
			{ '.', '#', '.' }
		},
		new char[,] {
			{ '.', '.', '#' },
			{ '.', '.', '#' },
			{ '#', '#', '#' }
		},
		new char[,] {
			{ '#' },
			{ '#' },
			{ '#' },
			{ '#' },
		},
		new char[,] {
			{ '#', '#' },
			{ '#', '#' },
		}
	});

	var moves = new RingAccess<char>(ReadInput());
	var chamber = new Dictionary<(int x, int y), char>();

	for (var i = 0; i < rounds; i++)
	{
		var piece = pieces.Next();
		Simulate(piece, chamber, moves, i);
	}
	Print(chamber);
	var h  = (chamber.Min(e => e.Key.y) * -1) + 1;
	return h;
}

static void Simulate(char[,] piece, Dictionary<(int x, int y), char> chamber, RingAccess<char> moves, int round)
{
	var x = 2;
	var h = piece.GetLength(0);
	var minY = chamber.Any() ? chamber.Where(e => e.Value == '#').Min(e => e.Key.y) : 1;
	var y = minY - 4 - (h - 1);
	
	while (true)
	{
		var m = moves.Next();
		var dx = m == '<' ? -1 : 1;
		if (Check((x, y), (dx, 0), piece, chamber))
		{
			x += dx;
		}
		if (Check((x, y), (0, 1), piece, chamber))
		{
			y++;
		}
		else
		{
			Put((x, y), piece, chamber);
			break;
		}
	}
}

static void Put((int x, int y) p, char[,] piece, Dictionary<(int x, int y), char> chamber)
{
	var h = piece.GetLength(0);
	var w = piece.GetLength(1);
	for (var y = 0; y < h; y++)
	{
		for (var x = 0; x < w; x++)
		{
			var c = piece[y, x];
			if (c != '.')
			{
				chamber[(x + p.x, y + p.y)] = c;
			}
		}
	}
}

static bool Check((int x, int y) p, (int x, int y) d, char[,] piece, Dictionary<(int x, int y), char> chamber)
{
	var h = piece.GetLength(0);
	var w = piece.GetLength(1);
	p.x += d.x;
	p.y += d.y;
	if (p.x < 0 || p.x + w > 7 || p.y > 0)
	{
		return false;
	}	
	for (var y = 0; y < h; y++)
	{
		for (var x = 0; x < w; x++)
		{
			if (piece[y, x] != '.' && GetAt((x + p.x, y + p.y), chamber) != '.')
			{
				return false;
			}
		}
	}
	return true;
}

static char GetAt((int x, int y) p, Dictionary<(int x, int y), char> chamber)
{
	if (chamber.TryGetValue(p, out var c))
	{
		return c;
	}
	return '.';
}

static void Print(Dictionary<(int x, int y), char> chamber)
{
	var minY = chamber.Min(e => e.Key.y);
	for (var y = minY; y <= 0; y++)
	{
		Console.Write('|');
		for (var x = 0; x < 7; x++)
		{
			Console.Write(GetAt((x, y), chamber));
		}
		Console.WriteLine($"| {y * -1 + 1}");
	}
	Console.WriteLine("+-------+");
	Console.WriteLine();
}

static List<char> ReadInput() => Input.ReadCharList().ToList();

internal class RingAccess<T>
{
	private readonly IList<T> _list;
	private int _index = 0;

	internal RingAccess(IList<T> list)
	{
		_list = list;
	}

	internal T Next()
	{
		var t = _list[_index];
		_index = (_index + 1) % _list.Count;
		return t;
	}
}