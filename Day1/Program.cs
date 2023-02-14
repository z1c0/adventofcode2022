using aoc;

Day.Run(
	ReadInput,
	(Solve, 1, 24000, 69310),
	(Solve, 3, 45000, 206104)
);

static int Solve(List<List<int>> input, int top)
{
	var totalCalories = ReadInput().Select(l => l.Sum()).OrderDescending().Take(top).Sum();
	Console.WriteLine($"Total calories carried: {totalCalories}");
	return totalCalories;
}

static List<List<int>> ReadInput()
{
	var input = new List<List<int>>() { new() };
	foreach (var s in Input.ReadStringList().ToList())
	{
		if (!string.IsNullOrEmpty(s))
		{
			input.Last().Add(int.Parse(s));
		}
		else
		{
			input.Add(new());
		}
	}
	return input;
}