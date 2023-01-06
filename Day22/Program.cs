using aoc;

Day.Run(() =>
{
	Part1(false);
	Part1(true);
});

static void Part1(bool useCubeGeometry)
{
	var (moves, map) = ReadInput();
	var edgeCaches = CreateEdgeCaches(map);
	var x = edgeCaches.minMaxX[0].min;
	var y = 0;
	var dir = Direction.Right;
	foreach (var move in moves)
	{
		if (move.TurnLeft)
		{
			dir = (Direction)((int)(4 + dir - 1) % 4);
		}
		else if (move.TurnRight)
		{
			dir = (Direction)((int)(dir + 1) % 4);
		}
		else
		{
			for (var i = 0; i < move.By; i++)
			{	
				(var dx, var dy) = dir switch
				{
					Direction.Left => (-1, 0),
					Direction.Right => (1, 0),
					Direction.Up => (0, -1),
					Direction.Down => (0, 1),
					_ => throw new InvalidOperationException()
				};
				var p = WrapAroundCheck(useCubeGeometry, x, y, dx, dy, dir, map, edgeCaches);
				if (map[(p.x, p.y)] == '#')
				{
					break;
				}
				x = p.x;
				y = p.y;
				dir = p.dir;
			}
		}
	}

	//Print(map, (x, y, dir));

	Console.WriteLine($"row: {y + 1}, col: {x + 1}, {dir} -> {(y + 1) * 1000 + (x + 1) * 4 + (int)dir}");
}

static (int x, int y, Direction dir) WrapAroundCheck(bool useCubeGeometry, int x, int y, int dx, int dy, Direction dir, Dictionary<(int x, int y), char> map, (Dictionary<int, (int min, int max)> minMaxX, Dictionary<int, (int min, int max)> minMaxY) edgeCaches)
	=> useCubeGeometry ? WrapAroundCheckCube(x, y, dx, dy, dir, map, edgeCaches) : WrapAroundCheckPlane(x, y, dx, dy, dir, map, edgeCaches);

static (int x, int y, Direction dir) WrapAroundCheckCube(int x, int y, int dx, int dy, Direction dir, Dictionary<(int x, int y), char> map, (Dictionary<int, (int min, int max)> minMaxX, Dictionary<int, (int min, int max)> minMaxY) edgeCaches)
{
	var minMinX = edgeCaches.minMaxX.Min(e => e.Value.min);
	var maxMaxX = edgeCaches.minMaxX.Max(e => e.Value.max);
	var minMinY = edgeCaches.minMaxY.Min(e => e.Value.min);
	var maxMaxY = edgeCaches.minMaxY.Max(e => e.Value.max);
	var w = maxMaxX - minMinX + 1;
	var h = maxMaxY - minMinY + 1;
	var dim = Math.Max(w / 4, h / 4);
	
	var minX = edgeCaches.minMaxX[y].min;
	var maxX = edgeCaches.minMaxX[y].max;
	var minY = edgeCaches.minMaxY[x].min;
	var maxY = edgeCaches.minMaxY[x].max;

	x += dx;
	y += dy;

	var col = x / dim;
	var row = y / dim;
	var offX = x % dim;
	var offY = y % dim;

	if (x < minX)
	{
		(x, y, dir) = row switch
		{
			0 => GetSide(offX, offY, 0, 2, Direction.Left, Direction.Left, dim), // blue -> green
			1 => GetSide(offX, offY, 0, 2, Direction.Left, Direction.Up, dim), // red -> green
			2 => GetSide(offX, offY, 1, 0, Direction.Left, Direction.Left, dim), // green -> blue
			3 => GetSide(offX, offY, 1, 0, Direction.Left, Direction.Up, dim), // white -> blue
			_ => throw new InvalidOperationException(),
		};
	}
	else if (x > maxX)
	{
		(x, y, dir) = row switch
		{
			0 => GetSide(offX, offY, 1, 2, Direction.Right, Direction.Right, dim), // yellow -> purple
			1 => GetSide(offX, offY, 2, 0, Direction.Right, Direction.Down, dim), // red -> yellow
			2 => GetSide(offX, offY, 2, 0, Direction.Right, Direction.Right, dim), // purple -> yellow
			3 => GetSide(offX, offY, 1, 2, Direction.Right, Direction.Down, dim), // white -> purple
			_ => throw new InvalidOperationException(),
		};
	}
	else if (y < minY)
	{
		(x, y, dir) = col switch
		{
			0 => GetSide(offX, offY, 1, 1, Direction.Up, Direction.Left, dim), // green -> red
			1 => GetSide(offX, offY, 0, 3, Direction.Up, Direction.Left, dim), // blue -> white
			2 => GetSide(offX, offY, 0, 3, Direction.Up, Direction.Down, dim), // yellow -> white
			_ => throw new InvalidOperationException(),
		};
	}
	else if (y > maxY)
	{
		(x, y, dir) = col switch
		{
			0 => GetSide(offX, offY, 2, 0, Direction.Down, Direction.Up, dim), // white -> yellow
			1 => GetSide(offX, offY, 0, 3, Direction.Down, Direction.Right, dim), // purple -> white
			2 => GetSide(offX, offY, 1, 1, Direction.Down, Direction.Right, dim), // yellow -> red
			_ => throw new InvalidOperationException(),
		};
	}

	return (x, y, dir);
}

static	(int x, int y, Direction dir) GetSide(int x, int y, int col, int row, Direction dirFrom, Direction dirTo, int dim)
{
	var xx = col * dim;
	var yy = row * dim;

	var dd = dirTo switch
	{
		Direction.Left => Direction.Right,
		Direction.Right => Direction.Left,
		Direction.Down => Direction.Up,
		Direction.Up => Direction.Down,
		_ => throw new InvalidOperationException(),
	};

	switch (dirTo)
	{
		case Direction.Left:
			xx += 0;
			break;
		case Direction.Right:
			xx += dim - 1;
			break;
		case Direction.Up:
			yy += 0;
			break;
		case Direction.Down:
			yy += dim - 1;
			break;
	}

	if (dirFrom == Direction.Left)
	{
		if (dirTo == Direction.Up)
		{
			xx += y;
		}
		else if (dirTo == Direction.Left)
		{
			yy += dim - 1 - y;
		}
		else throw new NotImplementedException();
	}
	else if (dirFrom == Direction.Right)
	{
		if (dirTo == Direction.Down)
		{
			xx += y;
		}
		else if (dirTo == Direction.Right)
		{
			yy += dim - 1 - y;
		}
		else throw new NotImplementedException();
	}
	else if (dirFrom == Direction.Up)
	{
		if (dirTo == Direction.Left)
		{
			yy += x;
		}
		else if (dirTo == Direction.Down)
		{
			xx += x;
		}
		else throw new NotImplementedException();
	}
	else if (dirFrom == Direction.Down)
	{
		if (dirTo == Direction.Right)
		{
			yy += x;
		}
		else if (dirTo == Direction.Up)
		{
			xx += x;
		}
		else throw new NotImplementedException();
	}
	return (xx, yy, dd);
}


static (int x, int y, Direction dir) WrapAroundCheckPlane(int x, int y, int dx, int dy, Direction dir, Dictionary<(int x, int y), char> map, (Dictionary<int, (int min, int max)> minMaxX, Dictionary<int, (int min, int max)> minMaxY) edgeCaches)
{
	var minX = edgeCaches.minMaxX[y].min;
	var maxX = edgeCaches.minMaxX[y].max;
	var minY = edgeCaches.minMaxY[x].min;
	var maxY = edgeCaches.minMaxY[x].max;

	x += dx;
	y += dy;

	if (x < minX) x = maxX;
	if (x > maxX) x = minX;
	if (y < minY) y = maxY;
	if (y > maxY) y = minY;

	return (x, y, dir);
}

static (Dictionary<int, (int min, int max)> minMaxX, Dictionary<int, (int min, int max)> minMaxY) CreateEdgeCaches(Dictionary<(int x, int y), char> map)
{
	var minX = map.Min(e => e.Key.x);
	var maxX = map.Max(e => e.Key.x);
	var minY = map.Min(e => e.Key.y);
	var maxY = map.Max(e => e.Key.y);
	var minMaxX = new Dictionary<int, (int min, int max)>();
	var minMaxY = new Dictionary<int, (int min, int max)>();
	for (var y = minY; y <= maxY; y++)
	{
		var row = map.Where(e => e.Key.y == y).Select(e => e.Key.x).ToList();
		minMaxX[y] = (row.Min(), row.Max());
	}
	for (var x = minX; x <= maxX; x++)
	{
		var col = map.Where(e => e.Key.x == x).Select(e => e.Key.y).ToList();
		minMaxY[x] = (col.Min(), col.Max());
	}
	return (minMaxX, minMaxY);
}

static void Print(Dictionary<(int x, int y), char> map, (int x, int y, Direction dir) pos)
{
	var minX = map.Min(e => e.Key.x);
	var maxX = map.Max(e => e.Key.x);
	var minY = map.Min(e => e.Key.y);
	var maxY = map.Max(e => e.Key.y);
	for (var y = minY; y <= maxY; y++)
	{
		for (var x = minX; x <= maxX; x++)
		{
			var ch = ' ';
			if (x == pos.x && y == pos.y)
			{
				ch = pos.dir switch
				{
					Direction.Up => '^',
					Direction.Down => 'v',
					Direction.Left => '<',
					Direction.Right => '>',
					_ => throw new InvalidOperationException(),
				};
			}
			else if (map.ContainsKey((x, y)))
			{
				ch = map[(x, y)];
			}
			Console.Write(ch);
		}
		Console.WriteLine();
	}
	Console.WriteLine();
}

static (List<Move> moves, Dictionary<(int x, int y), char> map) ReadInput()
{
	var lines = Input.ReadStringList().ToList();
	var dict = new Dictionary<(int x, int y), char>();
	for (var y = 0; y < lines.Count - 2; y++)
	{
		var line = lines[y];
		for (var x = 0; x < line.Length; x++)
		{
			var c = line[x];
			if (c != ' ')
			{
				dict[(x, y)] = c;
			}
		}
	}
	
	var moves = new List<Move>();
	var i = 0;
	var s = lines.Last();
	while (i < s.Length)
	{
		if (s[i] == 'L')
		{
			moves.Add(new () { TurnLeft = true });
			i++;
		}
		else if (s[i] == 'R')
		{
			moves.Add(new () { TurnRight = true });
			i++;
		}
		else
		{
			var by = s[i] - '0';
			i++;
			while (i < s.Length && char.IsDigit(s[i]))
			{
				by *= 10;
				by += s[i++] - '0';
			}
			moves.Add(new (by));
		}
	}

	return (moves, dict);
}

internal enum Direction
{
	Right = 0,
	Down = 1,
	Left = 2,
	Up = 3,
}

internal class Move
{
	internal Move(int by = -1)
	{
		By = by;
	}

	public int By { get; }
	internal bool TurnLeft { get; init; }
	internal bool TurnRight { get; init; }

	public override string ToString() => TurnLeft ? "L" : TurnRight ? "R" : By.ToString();
}


