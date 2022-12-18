using aoc;

Day.Run(() =>
{
	Part1();
	Part2();
});

static void Part1()
{
	var count = GetSurfaceArea(ReadInput());
	Console.WriteLine($"The surface area is: {count}");
}

static void Part2()
{
	var cubes = ReadInput();
	var cache = new Dictionary<(int, int, int), bool>();
	var minX = cubes.Min(c => c.x);
	var maxX = cubes.Max(c => c.x);
	var minY = cubes.Min(c => c.y);
	var maxY = cubes.Max(c => c.y);
	var minZ = cubes.Min(c => c.z);
	var maxZ = cubes.Max(c => c.z);

	bool IsConnectedToWater((int x, int y, int z) p, HashSet<(int x, int y, int z)> cubes, Dictionary<(int, int, int), bool> result, HashSet<(int, int, int)> history)
	{
		history.Add(p);
		if (result.TryGetValue(p, out bool value))
		{
			return value;
		}
		if (p.x < minX || p.x > maxX || p.y < minY || p.y > maxY || p.z < minZ || p.z > maxZ)
		{
			result.Add(p, true);
			return true;
		}
		foreach (var a in GetEmptyAdjacent(p, cubes))
		{
			if (!history.Contains(a))
			{
				if (IsConnectedToWater(a, cubes, result, history))
				{
					result.Add(p, true);
					return true;
				}
			}
		}
		result.Add(p, false);
		return false;
	}
	
	for (var x = minX; x <= maxX; x++)
	{
		for (var y = minY; y <= maxY; y++)
		{
			for (var z = minZ; z <= maxZ; z++)
			{
				var p = (x, y, z);
				if (!cubes.Contains(p))
				{
					IsConnectedToWater(p, cubes, cache, new());
				}
			}
		}
	}

	var count = GetSurfaceArea(cubes);
	var c = cache.Where(e => !e.Value).Sum(e => 6 - GetEmptyAdjacent(e.Key, cubes).Count());
	Console.WriteLine($"The exterior surface area is: {count - c}");
}

static int GetSurfaceArea(HashSet<(int x, int y, int z)> cubes)
{
	var minX = cubes.Min(c => c.x);
	var maxX = cubes.Max(c => c.x);
	var minY = cubes.Min(c => c.y);
	var maxY = cubes.Max(c => c.y);
	var minZ = cubes.Min(c => c.z);
	var maxZ = cubes.Max(c => c.z);

	var count = 0;
	for (var x = minX; x <= maxX; x++)
	{
		for (var y = minY; y <= maxY; y++)
		{
			for (var z = minZ; z <= maxZ; z++)
			{
				if (cubes.Contains((x, y, z)))
				{
					count += GetEmptyAdjacent((x, y, z), cubes).Count();
				}
			}
		}
	}
	return count;
}

static IEnumerable<(int x, int y, int z)> GetEmptyAdjacent((int x, int y, int z) c, HashSet<(int x, int y, int z)> cubes)
{
	if (!cubes.Contains((c.x - 1, c.y, c.z))) yield return (c.x - 1, c.y, c.z);
	if (!cubes.Contains((c.x + 1, c.y, c.z))) yield return (c.x + 1, c.y, c.z);
	if (!cubes.Contains((c.x, c.y - 1, c.z))) yield return (c.x, c.y - 1, c.z);
	if (!cubes.Contains((c.x, c.y + 1, c.z))) yield return (c.x, c.y + 1, c.z);
	if (!cubes.Contains((c.x, c.y, c.z - 1))) yield return (c.x, c.y, c.z - 1);
	if (!cubes.Contains((c.x, c.y, c.z + 1))) yield return (c.x, c.y, c.z + 1);
}

static HashSet<(int x, int y, int z)> ReadInput()
{
	var cube = new HashSet<(int, int, int)>();
	foreach (var line in Input.ReadStringList())
	{
		var tokens = line.Split(',');
		cube.Add((int.Parse(tokens[0]),  int.Parse(tokens[1]), int.Parse(tokens[2])));
	}
	return cube;
}
