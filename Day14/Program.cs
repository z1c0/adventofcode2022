using System.Diagnostics;
using aoc;

Day.Run(() =>
{
	Part1(false);
	Part1(true);
});

static void Part1(bool simulateFloor)
{
	var map = ReadInput();
	var source = (500, 0);
	map[source] = '+';

	var restedUnitsCount = 0;
	var maxY = map.Keys.Max(k => k.y);
	if (simulateFloor)
	{
		maxY += 2;
	}
	while (true)
	{
		if (!Simulate(source, map, maxY, simulateFloor, ref restedUnitsCount))
		{
			break;
		}
	}
	// Only print test data.
	if (maxY < 50)
	{
		Print(map);
	}
	Console.WriteLine($"{restedUnitsCount} units of sand come to rest.");
}

static bool Simulate((int x, int y) source, Dictionary<(int x, int y), char> map, int maxY, bool simulateFloor, ref int restedUnitsCount)
{
	bool IsEmpty((int x, int y) p)
	{
		if (simulateFloor && p.y == maxY)
		{
			return false;
		}
		return !map.TryGetValue(p, out var c) || c == '.' || c == '+';
	} 

	var unit = source;
	while (true)
	{
		if (IsEmpty((unit.x, unit.y + 1)))
		{
			unit.y++;
		}
		else if (IsEmpty((unit.x - 1, unit.y + 1)))
		{
			unit.x--;
			unit.y++;
		}
		else if (IsEmpty((unit.x + 1, unit.y + 1)))
		{
			unit.x++;
			unit.y++;
		}
		else
		{
			// rest
			restedUnitsCount++;
			map[unit] = 'o';
			return unit != source;
		}
		if (unit.y == maxY)
		{
			return false;
		}
	}
}

static void Print(Dictionary<(int x, int y), char> map)
{
	var minX = map.Keys.Min(k => k.x) - 1;
	var minY = map.Keys.Min(k => k.y);
	var maxX = map.Keys.Max(k => k.x) + 1;
	var maxY = map.Keys.Max(k => k.y);
	for (var y = minY; y <= maxY; y++)
	{
		for (var x = minX; x <= maxX; x++)
		{
			if (!map.TryGetValue((x, y), out var c))
			{
				c = '.';
			}
			Console.Write(c);
		}
		Console.WriteLine();
	}
		Console.WriteLine();
}

static Dictionary<(int x, int y), char> ReadInput()
{
	static (int, int) Parse(string s)
	{
		var parts = s.Split(',');
		return (int.Parse(parts.First()), int.Parse(parts.Last()));
	}
	var map = new Dictionary<(int, int), char>();
	foreach (var line in Input.ReadStringList())
	{
		var parts = line.Split(" -> ");
		for (var i = 1; i < parts.Length; i++)
		{
			var (x1, y1) = Parse(parts[i - 1]);
			var (x2, y2) = Parse(parts[i]);
			var dx = Math.Sign(x2 - x1);
			var dy = Math.Sign(y2 - y1);
			do
			{
				map[(x1, y1)] = '#';
				x1 += dx;
				y1 += dy;
				map[(x1, y1)] = '#';
			}
			while (x1 != x2 || y1 != y2);
		}
	}
	return map;
}

