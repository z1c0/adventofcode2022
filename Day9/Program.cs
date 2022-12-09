using aoc;

Day.Run(() =>
{
	Part1(2);
	Part1(10);
});

static void Part1(int ropeLength)
{
	var rope = new (int X, int Y)[ropeLength];
	var history = new HashSet<(int, int)>() { rope.Last() };
	foreach (var (dir, steps) in ReadInput())
	{
		for (var i = 0; i < steps; i++)
		{
			switch (dir)
			{
				case Direction.Left:
					rope[0].X--;
					break;
				case Direction.Up:
					rope[0].Y--;
					break;
				case Direction.Right:
					rope[0].X++;
					break;
				case Direction.Down:
					rope[0].Y++;
					break;
			}
			//System.Console.WriteLine(rope.First());

			// follow tail
			for (var j = 1; j < rope.Length; j++)
			{
				var (dx, dy) = (rope[j - 1].X - rope[j].X, rope[j - 1].Y - rope[j].Y);
				if (Math.Abs(dx) == 2)
				{
					rope[j].X += Math.Sign(dx);
					if (dy != 0)
					{
						rope[j].Y += Math.Sign(dy);
					}
				}
				else if (Math.Abs(dy) == 2)
				{
					rope[j].Y += Math.Sign(dy);
					if (dx != 0)
					{
						rope[j].X += Math.Sign(dx);
					}
				}

				if (!history.Contains(rope.Last()))
				{
					history.Add(rope.Last());
				}
			}
		}
	}
	Print(rope);
	Console.WriteLine($"The tail of the rope visited {history.Count} positions.");
}

static void Print((int X, int Y)[] rope)
{
	var minX = rope.Min(k => k.X) - 1;
	var minY = rope.Min(k => k.Y) - 1;
	var maxX = rope.Max(k => k.X) + 1;
	var maxY = rope.Max(k => k.Y) + 1;
	for (var y = minY; y <= maxY; y++)
	{
		for (var x = minX; x <= maxX; x++)
		{
			var c = '.';
			for (var k = rope.Length - 1; k >= 0; k--)
			{
				if (rope[k] == (x, y))
				{
					c = (char)('0' + k);
				}
			}
			Console.Write(c);
		}
		Console.WriteLine();
	}
}

static IEnumerable<(Direction Dir, int Steps)> ReadInput()
{
	foreach (var (c, i) in Input.ReadCharIntList())
	{
		yield return
		(
			c switch
			{
				'L' => Direction.Left,
				'U' => Direction.Up,
				'R' => Direction.Right,
				'D' => Direction.Down,
				_ => throw new InvalidOperationException(),
			},
			i
		);
	}
}

internal enum Direction 
{
	Left,
	Up,
	Right,
	Down,
}

