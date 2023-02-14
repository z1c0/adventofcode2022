using aoc;

Day.Run(
	ReadInput,
	(Part1, 2, 450),
	(Part2, 4, 837)
);

static int Part1(IEnumerable<(Section section1, Section section2)> input)
{
	static bool matchFully(Section s1, Section s2) => s1.From <= s2.From && s1.To >= s2.To;
	var count = Match(matchFully, input);
	Console.WriteLine($"{count} sections are fully contained in another.");
	return count;
}

static int Part2(IEnumerable<(Section section1, Section section2)> input)
{
	static bool MatchPartly(Section s1, Section s2) => s1.From >= s2.From && s1.From <= s2.To || s1.To >= s2.From && s1.To <= s2.To;
	var count = Match(MatchPartly, input);
	Console.WriteLine($"{count} sections are overlap partly.");
	return count;
}

static int Match(Func<Section, Section, bool> predicate, IEnumerable<(Section section1, Section section2)> input)
{
	var count = 0;
	foreach (var (s1, s2) in input)
	{
		if (predicate(s1, s2) || predicate(s2, s1))
		{
			count++;
		}
	}
	return count;
}

static IEnumerable<(Section section1, Section section2)> ReadInput()
{
	foreach (var line in Input.ReadStringList().ToList())
	{
		static Section ParseSection(string s)
		{
			var parts = s.Split('-');
			return new Section(int.Parse(parts.First()), int.Parse(parts.Last()));
		}
		var parts = line.Split(',');
		yield return (ParseSection(parts.First()), ParseSection(parts.Last()));
	}
}

internal record Section(int From, int To);