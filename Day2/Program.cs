using aoc;

Day.Run(() =>
{
	Part1();
	Part2();
});

static void Part1()
{
	var score = 0L;
	foreach (var (Opponent, Me, _) in ReadInput().ToList())
	{
		score += Score(Me, Opponent);
	}
	Console.WriteLine($"The total score is {score}.");
}

static void Part2()
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
}

static int Score(Play me, Play opponent)
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