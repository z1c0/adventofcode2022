using aoc;

Day.Run(() =>
{
	Part1();
	Part2();
});

static void Part1()
{
	var allValves = ReadInput().ToList();

	var cache = new HashSet<string>();
	var queue = new PriorityQueue<State, int>();
	queue.Enqueue(new State(), 0);

	while (queue.Count > 0)
	{
		var state = queue.Dequeue();

		if (Check(state, allValves, cache))
		{
			if (CanOpenValve(state, state.Me, allValves))
			{
				var clone = state.Clone();
				clone.OpenValves.Add(state.Me);
				queue.Enqueue(clone, clone.TotalPressure * -1);
			}
			foreach (var c in GetConnectedValves(state.Me, allValves))
			{
				var clone = state.Clone();
				clone.Me = c;
				queue.Enqueue(clone, clone.TotalPressure * -1);
			}
		}
	}
	Console.WriteLine($"Maximum pressure: {State.MaxPressure}");
}

static void Part2()
{
	var allValves = ReadInput().ToList();

	var cache = new HashSet<string>();
	var queue = new PriorityQueue<State, int>();
	queue.Enqueue(new State() { Me = "AA", Elephant = "AA", Time = 4 }, 0);

	while (queue.Count > 0)
	{
		var state = queue.Dequeue();

		if (Check(state, allValves, cache))
		{
			// Both open a valve.
			if (CanOpenValve(state, state.Me, allValves) && CanOpenValve(state, state.Elephant, allValves))
			{
					var clone = state.Clone();
					clone.OpenValves.Add(state.Elephant);
					clone.OpenValves.Add(state.Me);
					queue.Enqueue(clone, clone.TotalPressure * -1);
			}

			// I open a valve, elephant moves.
			if (CanOpenValve(state, state.Me, allValves))
			{
				foreach (var c in GetConnectedValves(state.Elephant, allValves))
				{
					var clone1 = state.Clone();
					clone1.OpenValves.Add(state.Me);
					clone1.Elephant = c;
					queue.Enqueue(clone1, clone1.TotalPressure * -1);
				}
			}

			// Elephant opens a valve, I move.
			if (CanOpenValve(state, state.Elephant, allValves))
			{
				foreach (var c in GetConnectedValves(state.Me, allValves))
				{
					var clone2 = state.Clone();
					clone2.OpenValves.Add(state.Elephant);
					clone2.Me = c;
					queue.Enqueue(clone2, clone2.TotalPressure * -1);
				}
			}

			// Both move.
			foreach (var c1 in GetConnectedValves(state.Me, allValves))
			{
				foreach (var c2 in GetConnectedValves(state.Elephant, allValves))
				{
					if (c1 != c2)
					{
						var clone = state.Clone();
						clone.Me = c1;
						clone.Elephant = c2;
						queue.Enqueue(clone, clone.TotalPressure * -1);
					}
				}
			}
		}
	}

	Console.WriteLine($"Maximum pressure: {State.MaxPressure}");
}

static bool CanOpenValve(State state, string pos, List<Valve> allValves)
{
	var v = allValves.Single(v => v.Name == pos);
	return v.FlowRate > 0 && !state.OpenValves.Any(o => o == pos);
}

static List<string> GetConnectedValves(string pos, List<Valve> allValves)
{
	var v = allValves.Single(v => v.Name == pos);
	return v.ConnectedValves;
}

static bool Check(State state, List<Valve> allValves, HashSet<string> cache)
{
	var encoded = state.Encode();
	if (cache.Contains(encoded))
	{
		return false;
	}
	cache.Add(encoded);

	if (!state.Update(allValves))
	{
		return false;
	}

	return true;
}

static IEnumerable<Valve> ReadInput()
{
	foreach (var line in Input.ReadStringList())
	{
		var parts	= line.Split(' ');
		var connectedValves = string.Join(string.Empty, parts.Skip(9)).Split(',');
		yield return new(
			parts[1],
			int.Parse(parts[4].Split('=').Last()[..^1]),
			connectedValves.ToList()
		);
	}
}

internal class State
{
	internal State()
	{	
		Me = "AA";
		Elephant = "AA";
	}

	internal List<string> OpenValves { get; private set; } = new ();
	internal string Me { get; set; }
	internal string Elephant { get; set; }
	internal int Time { get; set; }
	internal int TotalPressure { get; private set; }
	internal static int MaxPressure { get; private set; }
	internal bool Update(List<Valve> allValves)
	{
		if (Time == 30)
		{
			return false;
		}

		Time++;
		foreach (var o in OpenValves)
		{
			TotalPressure += allValves.Single(v => v.Name == o).FlowRate;
		}
		if (TotalPressure > MaxPressure)
		{
			MaxPressure = TotalPressure;
		}
		return true;
	}

	internal State Clone() =>
		new()
		{
			OpenValves = OpenValves.ToList(),
			Me = Me,
			Elephant = Elephant,
			Time = Time,
			TotalPressure = TotalPressure,
		};

	internal string Encode()
	{
		var pos = string.Join('/', new[] { Me, Elephant }.Order());
		return $"{pos}-{Time}-{TotalPressure}";
	}
}

internal class Valve
{
	internal Valve(string name, int flowRate, List<string> connectedValves)
	{
		Name = name;
		FlowRate = flowRate;
		ConnectedValves = connectedValves;
	}
	internal string Name { get; }
	internal int FlowRate { get; }
	internal List<string> ConnectedValves { get; }

	public override string ToString() => $"{Name} {FlowRate} -> {string.Join(',', ConnectedValves)}";
}
