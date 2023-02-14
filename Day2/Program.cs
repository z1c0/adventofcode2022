using aoc;

Day.Run(
	ReadInput,
	(Part1, 15L, 10624L),
	(Part2, 12L, 14060L)
);

static long Part1(IEnumerable<(Play Opponent, Play Me, Result Result)> input)
{
	var score = input.Sum(i => Score(i.Me, i.Opponent));
	Console.WriteLine($"The total score is {score}.");
	return score;
}

static long Part2(IEnumerable<(Play Opponent, Play Me, Result Result)> input)
{
	var score = 0L;
	foreach (var (Opponent, _, Result) in ReadInput().ToList())
	{
		Play me = Result switch
		{
			Result.Lose => Opponent switch
			{
				Play.Rock => Play.Scissors,
				Play.Paper => Play.Rock,
				Play.Scissors => Play.Paper,
				_ => throw new InvalidOperationException(),
			},
			Result.Draw => Opponent switch
			{
				_ => Opponent,
			},
			Result.Win => Opponent switch
			{
				Play.Rock => Play.Paper,
				Play.Paper => Play.Scissors,
				Play.Scissors => Play.Rock,
				_ => throw new InvalidOperationException(),
			},
			_ => throw new InvalidOperationException(),
		};
		score += Score(me, Opponent);
	}
	Console.WriteLine($"The total score is {score}.");
	return score;
}

static long Score(Play me, Play opponent)
{
	var score = (int)me + 1;
	score += me switch
	{
		Play.Rock => opponent switch
		{
			Play.Rock => 3,
			Play.Paper => 0,
			Play.Scissors => 6,
			_ => throw new InvalidOperationException(),
		},
		Play.Paper => opponent switch
		{
			Play.Rock => 6,
			Play.Paper => 3,
			Play.Scissors => 0,
			_ => throw new InvalidOperationException(),
		},
		Play.Scissors => opponent switch
		{
			Play.Rock => 0,
			Play.Paper => 6,
			Play.Scissors => 3,
			_ => throw new InvalidOperationException(),
		},
		_ => throw new InvalidOperationException(),
	};
	return score;
}

static IEnumerable<(Play Opponent, Play Me, Result Result)> ReadInput()
{
	foreach (var s in Input.ReadStringList())
	{
		yield return ((Play)(s[0] - 'A'), (Play)(s[2] - 'X'), (Result)(s[2] - 'X'));
	}
}

enum Play
{
	Rock,
	Paper,
	Scissors,
}

enum Result
{
	Lose,
	Draw,
	Win,
}