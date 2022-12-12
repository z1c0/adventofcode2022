using aoc;

Day.Run(() =>
{
	Part1();
});

static void Part1()
{
	var map = Input.ReadCharGrid();
	var start = map.Find('S');
	var end = map.Find('E');
	map[start] = 'a';
	map[end] = 'z';

	Console.WriteLine($"Fewest steps from current position: {BFS(new[] { start }, end, map)}");
	Console.WriteLine($"Fewest steps from any a-square: {BFS(map.FindAll('a'), end, map)}");
}

static int BFS(IEnumerable<(int X, int Y)> startPositions, (int X, int Y) end, Grid<char> map)
{
	var minSteps = int.MaxValue;
	foreach (var start in startPositions)
	{
		var visited = new HashSet<(int, int)>() { start };
		var queue = new Queue<((int, int) Pos, int Distance)>();
		queue.Enqueue((start, 0));
		while (queue.Any())
		{
			var (current, distance) = queue.Dequeue();
			foreach (var a in map.GetAdjacent4(current))
			{
				var h = map[a];
				if (!visited.Contains(a) && h - map[current] <= 1)
				{
					if (a == end)
					{
						minSteps = Math.Min(minSteps, distance + 1);
						break;
					}

					visited.Add(a);
					queue.Enqueue((a, distance + 1));
				}
			}
		}
	}
	return minSteps;
}
