using aoc;

Day.Run(() =>
{
	Part1(10);
	Part1(int.MaxValue);
});

static void Part1(int rounds)
{
	var map = Input.ReadCharDictionaryGrid();

	var directions = new List<char> { 'N', 'S', 'W', 'E' };
	for (var i = 0; i < rounds; i++)
	{
		// First half.
		var proposals = new Dictionary<(int, int), List<(int, int)>>();
		var elves = map.FindAll('#').ToList();
		foreach (var e in elves)
		{
			var a = map.GetAdjacent8(e, true, '.').ToList();
			if (a.Any(c => c.v == '#'))
			{
				foreach (var d in directions)
				{
					var done = d switch
					{
						'N' => TryPropose(e, proposals, map, a[1].p, a[2].p, a[0].p),
						'S' => TryPropose(e, proposals, map, a[6].p, a[7].p, a[5].p),
						'W' => TryPropose(e, proposals, map, a[3].p, a[0].p, a[5].p),
						'E' => TryPropose(e, proposals, map, a[4].p, a[2].p, a[7].p),
						_ => throw new InvalidOperationException(),
					};
					if (done)
					{
						break;
					}
				}
			}
		}
		// Second half.
		var moved = false;
		foreach (var p in proposals)
		{
			if (p.Value.Count == 1)
			{
				map[p.Value.Single()] = '.';
				map[p.Key] = '#';
				moved = true;
			}
		}
		if (!moved)
		{
			Console.WriteLine($"In round {i + 1} no more elves are moving.");
			return;
		}

		// Cycle directions.
		var l = directions.First();
		directions.RemoveAt(0);
		directions.Add(l);
	}

	var cells = map.FindAll('#').ToList();
	var minX = cells.Min(e => e.x);
	var minY = cells.Min(e => e.y);
	var maxX = cells.Max(e => e.x);
	var maxY = cells.Max(e => e.y);
	var count = (maxX - minX + 1) * (maxY - minY + 1) - cells.Count;
	Console.WriteLine($"Number of empty ground tiles: {count}.");
}

static bool TryPropose((int x, int y) elf, Dictionary<(int, int), List<(int, int)>> proposals, DictionaryGrid<char> map, (int x, int y) a1, (int x, int y) a2, (int x, int y) a3)
{
	static bool Check(DictionaryGrid<char> map, (int x, int y) a) => map.Get(a).v == '.';

	if (Check(map, a1) &&	Check(map, a2) &&	Check(map, a3))
	{
		if (!proposals.ContainsKey(a1))
		{
			proposals.Add(a1, new());
		}
		proposals[a1].Add(elf);
		return true;
	}
	return false;
}