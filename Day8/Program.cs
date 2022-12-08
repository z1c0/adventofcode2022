using System.Diagnostics;
using aoc;

Day.Run(() =>
{
	var grid = Input.ReadIntGrid();
	Part1(grid);
	Part2(grid);
});

static void Part1(Grid<int> grid)
{
	var visible = 0;
	grid.ForEach(c =>
	{
		foreach (var n in GetNeighbors(grid, c))
		{
			if (!n.Any() || n.All(h => h < grid[c]))
			{
				visible++;
				break;
			}
		}
	});
	Console.WriteLine($"{visible} trees are visible from the outside.");
}

static void Part2(Grid<int> grid)
{
	var maxScore = 0L;
	grid.ForEach(c =>
	{
		if (!grid.IsOnEdge(c))
		{
			var score = 1;
			foreach (var n in GetNeighbors(grid, c))
			{
				var count = 0;
				foreach (var h in n)
				{
					count++;
					if (h >= grid[c])
					{
						break;
					}
				}
				score *= count;
			}
			maxScore = Math.Max(maxScore, score);
		}
	});
	Console.WriteLine($"The highest scenic score possible is {maxScore}.");
}

static List<int>[] GetNeighbors(Grid<int> grid, (int X, int Y) c)
{
	var left = new List<int>();
	for (var x = c.X - 1; x >= 0; x--)
	{
		left.Add(grid[x, c.Y]);
	}
	var top = new List<int>();
	for (var y = c.Y - 1; y >= 0; y--)
	{
		top.Add(grid[c.X, y]);
	}
	var right = new List<int>();
	for (var x = c.X + 1; x < grid.Width; x++)
	{
		right.Add(grid[x, c.Y]);
	}
	var bottom = new List<int>();
	for (var y = c.Y + 1; y < grid.Height; y++)
	{
		bottom.Add(grid[c.X, y]);
	}
	return new[] { left, top, right, bottom };
}