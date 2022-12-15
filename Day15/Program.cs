using aoc;

Day.Run(() =>
{
	Part1(2000000);
	Part2(4000000);
});

static void Part1(int row)
{
	var input = ReadInput().ToList();
	var beaconMinX = input.Min(i => i.Sensor.x - i.Distance);
	var beaconMaxX = input.Max(i => i.Sensor.x + i.Distance);

	var count = 0L;
	for (var x = beaconMinX; x <= beaconMaxX; x++)
	{
		var p = (x, row);
		if (!IsActualBeaconPosition(p, input) && !IsPossibleBeaconPosition(p, input))
		{
			count++;
		}
	}
	Console.WriteLine($"{count} positions can not contain a beacon.");
}

static void Part2(int max)
{
	var input = ReadInput().ToList();
	for (var y = 0; y < max; y++)
	{
		var x = 0;
		while (x <= max)
		{
			var p = (x, y);
			var s = input.FirstOrDefault(i => i.Contains(p));
			if (s != null)
			{
				var w = s.GetWidthAt(y);
				x = s.Sensor.x + w + 1;
			}
			else
			{
				Console.WriteLine($"The tuning frequency is {x * 4000000L + y} ({p}).");
				return;
			}
		}
	}
}
static bool IsActualBeaconPosition((int x, int y) p, List<SensorBeaconPair> input) => input.Any(i => i.Beacon == p);

static bool IsPossibleBeaconPosition((int x, int y) p, List<SensorBeaconPair> input)
{
	// False if there is an >actual< beacon at this position.
	if (IsActualBeaconPosition(p, input))
	{
		return false;
	}
	// If the point's (p) distance to any sensor is smaller than this sensors's distance to its
	// nearest beacon, no beacon can be at this point.
	return !input.Any(pair => pair.Distance >= SensorBeaconPair.GetDistance(pair.Sensor, p));
}

static IEnumerable<SensorBeaconPair> ReadInput()
{
	static int Parse(string s) => int.Parse(s.Split('=').Last());
	foreach (var line in Input.ReadStringList())
	{
		var parts = line.Split(' ');
		yield return new((Parse(parts[2][..^1]), Parse(parts[3][..^1])), (Parse(parts[8][..^1]), Parse(parts[9])));
	}
}

internal class SensorBeaconPair
{
	internal SensorBeaconPair((int x, int y) sensor, (int x, int y) beacon)
	{
		Sensor = sensor;
		Beacon = beacon;
		Distance = GetDistance(Sensor, Beacon);
	}

	public (int x, int y) Sensor { get; }
	public (int x, int y) Beacon { get; }
	public int Distance { get; }

	public override string ToString() => $"{Sensor} - {Beacon}";

	internal bool Contains((int x, int y) p) => GetDistance(p, Sensor) <= Distance;

	internal static int GetDistance((int x, int y) a, (int x, int y) b) => Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);

	internal int GetWidthAt(int y) => Distance - Math.Abs(Sensor.y - y);
}


