using aoc;

Day.Run(() =>
{
	Part1(false);
	Part1(true);
});

static void Part1(bool reverse)
{
	var (crates, moves) = ReadInput();

	foreach (var m in moves)
	{
		var moved = new List<char>();
		for (var i = 0; i < m.Count; i++)
		{
			moved.Add(crates[m.From - 1].Pop());
		}
		if (reverse)
		{
			moved.Reverse();
		}
		foreach (var c in moved)
		{
			crates[m.To - 1].Push(c);
		}
	}
	foreach (var c in crates)
	{
		Console.Write(c.FirstOrDefault(' '));
	}
	Console.WriteLine();
}

static (List<Stack<char>> Crates, List<Move> Moves) ReadInput()
{
	var crates = new List<Stack<char>>();
	var moves = new List<Move>();
	
	var lines = Input.ReadStringList().ToList();

	var nrOfCreates = lines.First().Length / 4 + 1;
	for (var c = 0; c < nrOfCreates; c++)
	{
		crates.Add(new());
	}

	var i = 0;
	while (true)
	{
		var line = lines[i++];
		if (!line.Contains('['))
		{
			break;
		}
		for (var c = 0; c < nrOfCreates; c++)
		{
			var crate = line[c * 4 + 1];
			if (crate != ' ')
			{
				crates[c].Push(crate);
			}
		}
	}
	for (var c = 0; c < nrOfCreates; c++)
	{
		var tmp = new Stack<char>();
		var stack = crates[c];
		while (stack.Any())
		{
			tmp.Push(stack.Pop());
		}
		crates[c] = tmp;
	}
	
	i++;
	while (i < lines.Count)
	{
		var line = lines[i++];
		var parts = line.Split(' ');
		moves.Add(new(int.Parse(parts[1]), int.Parse(parts[3]), int.Parse(parts[5])));
	}

	return (crates, moves);
}

internal record Move(int Count, int From, int To);