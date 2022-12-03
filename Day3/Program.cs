using aoc;

Day.Run(() =>
{
	Part1();
	Part2();
});

static void Part1()
{
	var sum = 0L;
	foreach (var line in ReadInput())
	{
		var c = Find(new() { line[..(line.Length / 2)], line[(line.Length / 2)..] });
		sum += Score(c);
	}
	Console.WriteLine($"The sum of the priorities is: {sum}");
}

static void Part2()
{
	var sum = 0L;
	var input = ReadInput();
	for (var i = 0; i < input.Count; i += 3)
	{
		var c = Find(new() { input[i], input[i + 1], input[i + 2] });
		sum += Score(c);
	}
	Console.WriteLine($"The sum of the priorities is: {sum}");
}

static char Find(List<string> lines)
{
	foreach (var c in lines.First())
	{
		if (lines.Skip(1).All(l => l.Contains(c)))
		{
			return c;
		}
	}
	throw new InvalidOperationException();
}

static int Score(char c)
{
	return c switch
	{
		>= 'a' and <= 'z' => c - 'a' + 1,
		>= 'A' and <= 'Z' => c - 'A' + 27,
		_ => throw new InvalidOperationException(),
	};
}

static List<string> ReadInput()
{
	return Input.ReadStringList().ToList();
}
