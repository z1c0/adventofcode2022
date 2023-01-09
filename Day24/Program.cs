using aoc;

Day.Run(() =>
{
	Part1();
	Part2();
});

static void Part1()
{
	var (blizzards, width, height) = ReadInput();
	var s = Route(true, width, height, blizzards);
	Console.WriteLine($"Reached the goal in {s.Time} minutes.");
}

static void Part2()
{
	var (blizzards, width, height) = ReadInput();
	var s1 = Route(true, width, height, blizzards);
	var s2 = Route(false, width, height, s1.Blizzards);
	var s3 = Route(true, width, height, s2.Blizzards);
	Console.WriteLine($"Reached the goal in {s1.Time + s2.Time + s3.Time} ({s1.Time} + {s2.Time} + {s3.Time}) minutes.");
}

static (List<(int X, int Y, char)> blizzards, int width, int height) ReadInput()
{
	var grid = Input.ReadCharGrid();
	var blizzards = grid.FindAll(c => c.v != '#' && c.v != '.').Select(c => (c.p.x, c.p.y, c.v)).ToList();
	return (blizzards, grid.Width, grid.Height);
}

static State Route(bool forward, int width, int height, List<(int X, int Y, char)> blizzards)
{
	var from = (1, 0);
	var to = (width - 2, height - 1);
	if (!forward)
	{
		(from, to) = (to, from);
	}
	var queue = new PriorityQueue<State, int>();
	var initState = new State(blizzards, width, height, from, from, to, 0);
	var cache = new Dictionary<string, int>();
	queue.Enqueue(initState, int.MaxValue);
	State? minState = null;
	while (queue.Count > 0)
	{
		var s = queue.Dequeue();
		var key = s.FingerPrint();
		if (cache.ContainsKey(key))
		{
			if (s.Time >= cache[key] && minState != null)
			{
				continue;
			}
		}
		cache[key] = s.Time;

		if (minState == null || s.Time < minState.Time)
		{
			if (s.Check())
			{
				minState = s;
				Console.WriteLine($"Time: {s.Time}");
				//s.Print();
			}
			else
			{
				s.MoveBlizzards();
				var adjacents = new[] 
				{
					(s.Pos.x + 1, s.Pos.y),
					(s.Pos.x, s.Pos.y + 1),
					(s.Pos.x - 1, s.Pos.y),
					(s.Pos.x, s.Pos.y - 1),
					s.Pos,
				};
				var next = adjacents.Where(s.IsFree).ToList();
				if (next.Any())
				{
					foreach (var a in next)
					{
						var copy = s.Clone(a);
						queue.Enqueue(copy, copy.Priority);
					}
				}
			}
		}
	}
	return minState!;
}

internal class State
{
	private readonly int _width;
	private readonly int _height;
	private readonly (int x, int y) _from;
	private readonly (int x, int y) _to;

	internal State(List<(int x, int y, char dir)> blizzards, int width, int height, (int x, int y) pos, (int x, int y) from, (int x, int y) to, int time)
	{
		Blizzards = blizzards;
		_width = width;
		_height = height;
		_from = from;
		_to = to;
		Pos = pos;
		Time = time;
	}

	internal bool IsFree((int x, int y) p)
	{
		if (p == _to || p == _from)
		{
			return true;
		}
		return p.x >= 1 && p.y >= 1 && p.x <= _width - 2 && p.y <= _height -2 && !Blizzards.Any(b => b.x == p.x && b.y == p.y);
	} 

	internal void MoveBlizzards()
	{
		for (var i = 0; i < Blizzards.Count; i++)
		{
			var b = Blizzards[i];
			(var x, var y) = b.dir switch
			{
				'>' => (b.x < _width - 2 ? b.x + 1 : 1, b.y),
				'<' => (b.x > 1 ? b.x - 1 : _width - 2, b.y),
				'v' => (b.x, b.y < _height - 2 ? b.y + 1 : 1),
				'^' => (b.x, b.y > 1 ? b.y - 1 : _height - 2),
				_ => throw new InvalidOperationException(),
			};
			Blizzards[i] = (x, y, b.dir);
		}
	}

	internal void Print()
	{
		var map = new Grid<char>(_width, _height);
		map.ForEach(p => map[p] = (p.X == 0 || p.Y == 0 || p.X == _width - 1 || p.Y == _height - 1) ? '#' : '.');
		foreach (var (x, y, dir) in Blizzards)
		{
			map[x, y] = dir;
		}
		map[(1, 0)] = '.';
		map[(_width - 2, _height - 1)] = '.';
		map[Pos] = '*';
		map.Print();
		Console.WriteLine();
	}

	internal int Priority
	{
		get
		{
			var dx = _to.x - Pos.x;
			var dy = _to.y - Pos.y;
			return dx * dx + dy * dy;
		}
	}

	internal List<(int x, int y, char dir)> Blizzards { get;}
	internal int Time { get; }

	internal (int x, int y) Pos { get; set; }
	internal bool Check() => Pos == _to;

	internal State Clone((int x, int y) pos) =>	new(Blizzards.ToList(), _width, _height, pos, _from, _to, Time + 1);

	internal string FingerPrint() => Blizzards.First(b => b.dir == '<').ToString() + Pos;
}