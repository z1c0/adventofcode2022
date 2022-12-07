using aoc;

Day.Run(() =>
{
	Part1(4);
	Part1(14);
});

static void Part1(int nrOfDifferentChars)
{
	var input = ReadInput().ToList();
	for (var i = 0; i < input.Count - nrOfDifferentChars + 1; i++)
	{
		var chars = new HashSet<char>();
		var collision = false;
		for (var j = i; j < i + nrOfDifferentChars; j++)
		{
			var c = input[j];
			if (chars.Contains(c))
			{
				collision = true;
				break;
			}
			chars.Add(c);
		}
		if (!collision)
		{
			Console.WriteLine($"First marker after character {i + nrOfDifferentChars}");
			break;
		}
	}
}

static List<char> ReadInput()
{
	return Input.ReadCharList().ToList();
}
