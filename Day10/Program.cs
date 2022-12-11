using aoc;

Day.Run(() =>
{
	Part1();
});

static void Part1()
{
	var cycle = 0L;
  var x = 1L;
	var sum = 0L;

	void Draw()
	{
		var xPos = cycle % 40;
		if (xPos -1 <= x && x <= xPos + 1)
		{
			Console.Write('#');
		}
		else
		{
			Console.Write('.');
		}
		if ((cycle + 1) % 40 == 0)
		{
			Console.WriteLine();
		}
	}

	void NextCycle()
	{
		Draw();
		cycle++;
		if (cycle == 20 || cycle == 60 || cycle == 100 || cycle == 140 || cycle == 180 || cycle == 220)
		{
			sum += x * cycle;
		}
	}

	foreach (var v in ReadInput())
	{
		switch (v.Instruction)
		{
			case Instruction.Addx:
				NextCycle();
				NextCycle();
				x += v.Value;
				break;

			case Instruction.Noop:
				NextCycle();
				break;

			default:
				break;
		}
	}
	Console.WriteLine($"Sum of the signal strengths: {sum}.");
}

static IEnumerable<(Instruction Instruction, long Value)> ReadInput()
{
	foreach (var line in Input.ReadStringList())
	{
		var parts = line.Split(' ');
		yield return(
			parts.First() switch
			{
				"addx" => Instruction.Addx,
				"noop" => Instruction.Noop,
				_ => throw new InvalidOperationException()
			},
			parts.Length > 1 ? long.Parse(parts.Last()) : 0
		);
	}
}

internal enum Instruction
{
	Addx,
	Noop,
}

