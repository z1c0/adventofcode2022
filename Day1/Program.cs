using aoc;

Day.Run(() =>
{
	Part1(1);
	Part1(3);
});

static void Part1(int top)
{
	var caloriesSorted = ReadInput().OrderByDescending(e => e.Sum()).ToList();
	var totalCalories = 0L;
	for (var i = 0; i < top; i++)
	{
		totalCalories += caloriesSorted[i].Sum();
	}
	Console.WriteLine($"Total calories carried: {totalCalories}");
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