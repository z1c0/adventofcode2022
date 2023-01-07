using aoc;

Day.Run(() =>
{
	Part1();
	Part2();
});

static void Part1()
{
	var blueprints = ReadInput().ToList();
	var qualityLevel = 0;
	var id = 1;
	foreach (var b in blueprints)
	{
		Simulate(new(b) { OreRobotCount = 1, Time = 24 });
		Console.WriteLine($"{id}: {State.Maximum}");
		qualityLevel += id * State.Maximum;
		id++;
	}
	Console.WriteLine($"Total quality level: {qualityLevel}");
}

static void Part2()
{
	var blueprints = ReadInput().ToList();
	var result = 1;
	foreach (var b in blueprints.Take(3))
	{
		Simulate(new(b) { OreRobotCount = 1, Time = 32 });
		Console.WriteLine($"max: {State.Maximum}");
		result *= State.Maximum;
	}
	Console.WriteLine($"Result: {result}");
}

static void Simulate(State start)
{
	State.Maximum = 0;
	var cache = new Dictionary<string, int>();

	static void AddIfNotNull(List<State> states, State? state)
	{
		if (state != null)
		{
			states.Add(state);
		}
	}

	var queue = new PriorityQueue<State, int>();
	queue.Enqueue(start, 0);
	while (queue.Count > 0)
	{
		var state = queue.Dequeue();
		if (Check(state, cache))
		{
			var g = state.TryBuyGeodeRobot();
			if (g != null)
			{
				g.Update();
				queue.Enqueue(g, g.Score() * -1);
			}
			else
			{
				var newStates = new List<State>() { state };
				AddIfNotNull(newStates, state.TryBuyOreRobot());
				AddIfNotNull(newStates, state.TryBuyClayRobot());
				AddIfNotNull(newStates, state.TryBuyObsidianRobot());
				AddIfNotNull(newStates, state.TryBuyGeodeRobot());
				foreach (var s in newStates)
				{
					s.Update();
					queue.Enqueue(s, s.Score() * -1);
				}
			}
		}
	}
}

static bool Check(State state, Dictionary<string, int> cache)
{
	var key = state.Encode();
	if (cache.ContainsKey(key))
	{
		if (state.Score() <= cache[key])
		{
			return false;
		}
	}
	cache[key] = state.Score();

	if (state.Minute == state.Time)
	{
		if (state.Geode > State.Maximum)
		{
			//Console.WriteLine(state.Geode);
			State.Maximum = state.Geode;
		}
		return false;
	}
	return true;
}

static IEnumerable<Blueprint> ReadInput()
{
	foreach (var line in Input.ReadStringList())
	{
		var parts = line[(line.IndexOf(':') + 2)..].Split(". ");
		yield return new Blueprint
		{
			OreRobotCost = (int.Parse(parts[0].Split(' ')[4]), 0, 0),
			ClayRobotCost = (int.Parse(parts[1].Split(' ')[4]), 0, 0),
			ObsidianRobotCost = (int.Parse(parts[2].Split(' ')[4]), int.Parse(parts[2].Split(' ')[7]), 0),
			GeodeRobotCost = (int.Parse(parts[3].Split(' ')[4]), 0, int.Parse(parts[3].Split(' ')[7])),
		};
	}
}

internal class State
{
	private readonly Blueprint _blueprint;
	private bool _addOreRobot;
	private bool _addClayRobot;
	private bool _addObsidianRobot;
	private bool _addGeodeRobot;

	internal State(Blueprint blueprint) => _blueprint = blueprint;

	internal int OreRobotCount { get; set; }
	internal int ClayRobotCount { get; set; }
	internal int ObsidianRobotCount { get; set; }
	internal int GeodeRobotCount { get; set; }
	internal int Minute { get; set; }
	internal int Time { get; init; }

	internal int Ore { get; set; }
	internal int Clay { get; set; }
	internal int Obsidian { get; set; }
	internal int Geode { get; set; }

	internal static int Maximum { get; set; }

	internal void Update()
	{
		Ore += OreRobotCount;
		Clay += ClayRobotCount;
		Obsidian += ObsidianRobotCount;
		Geode += GeodeRobotCount;
		Minute++;
		if (_addOreRobot)
		{
			OreRobotCount++;
			_addOreRobot = false;
		}
		if (_addClayRobot)
		{
			ClayRobotCount++;
			_addClayRobot = false;
		}
		if (_addObsidianRobot)
		{
			ObsidianRobotCount++;
			_addObsidianRobot = false;
		}
		if (_addGeodeRobot)
		{
			GeodeRobotCount++;
			_addGeodeRobot = false;
		}
	}

	public override string ToString()
	{
		return $"Ore: {Ore}, Clay: {Clay}, Obsidian: {Obsidian}, Geode: {Geode}";
	}

	internal State? TryBuyOreRobot()
	{
		if (Time - Minute < 14) return null;

		State? clone = null;
		if (Ore >= _blueprint.OreRobotCost.ore)
		{
			clone = Clone();
			clone._addOreRobot = true;
			clone.Ore -= _blueprint.OreRobotCost.ore;
		}
		return clone;

	}
	internal State? TryBuyClayRobot()
	{
		if (Time - Minute < 6) return null;

		State? clone = null;
		if (Ore >= _blueprint.ClayRobotCost.ore)
		{
			clone = Clone();
			clone._addClayRobot = true;
			clone.Ore -= _blueprint.ClayRobotCost.ore;
		}
		return clone;
	}
	internal State? TryBuyObsidianRobot()
	{
		if (Time - Minute < 4) return null;

		State? clone = null;
		if (Ore >= _blueprint.ObsidianRobotCost.ore && Clay >= _blueprint.ObsidianRobotCost.clay)
		{
			clone = Clone();
			clone._addObsidianRobot = true;
			clone.Ore -= _blueprint.ObsidianRobotCost.ore;
			clone.Clay -= _blueprint.ObsidianRobotCost.clay;
		}
		return clone;
	}
	internal State? TryBuyGeodeRobot()
	{
		if (Time - Minute < 1) return null;

		State? clone = null;
		if (Ore >= _blueprint.GeodeRobotCost.ore && Obsidian >= _blueprint.GeodeRobotCost.obsidian)
		{
			clone = Clone();
			clone._addGeodeRobot = true;
			clone.Ore -= _blueprint.GeodeRobotCost.ore;
			clone.Obsidian -= _blueprint.GeodeRobotCost.obsidian;
		}
		return clone;
	}
	internal State Clone() =>
		new(_blueprint)
		{
			Ore = Ore,
			Clay = Clay,
			Obsidian = Obsidian,
			Geode = Geode,
			OreRobotCount = OreRobotCount,
			ClayRobotCount = ClayRobotCount,
			ObsidianRobotCount = ObsidianRobotCount,
			GeodeRobotCount = GeodeRobotCount,
			Minute = Minute,
			Time = Time,
		};

	internal int Score() => Geode;
	internal string Encode() => $"{Minute}-{Ore}-{Clay}-{Obsidian}-{Geode}-{OreRobotCount}-{ClayRobotCount}-{ObsidianRobotCount}-{GeodeRobotCount}";
}

internal class Blueprint
{
	internal (int ore, int clay, int obsidian) OreRobotCost { get; init; }
	internal (int ore, int clay, int obsidian) ClayRobotCost { get; init; }
	internal (int ore, int clay, int obsidian) ObsidianRobotCost { get; init; }
	internal (int ore, int clay, int obsidian) GeodeRobotCost { get; init; }

	public override string ToString() => $"{OreRobotCost} - {ClayRobotCost} - {ObsidianRobotCost} - {GeodeRobotCost}";
}


