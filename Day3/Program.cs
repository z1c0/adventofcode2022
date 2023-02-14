using aoc;

Day.Run(
	Input.ReadStringList,
	(Part1, 157L, 8243L),
	(Part2, 70L, 2631L)
);

static long Part1(IList<string> input) =>
	input.Sum(line => Score(Find(new() { line[..(line.Length / 2)], line[(line.Length / 2)..] })));

static long Part2(IList<string> input)
{
	var sum = 0L;
	for (var i = 0; i < input.Count; i += 3)
	{
		var c = Find(new() { input[i], input[i + 1], input[i + 2] });
		sum += Score(c);
	}
	return sum;
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