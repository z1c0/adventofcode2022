using System.Text;
using aoc;

Day.Run(() =>
{
	Part1();
});

static void Part1()
{
	Console.WriteLine($"SNAFU sum: {ConvertToSnafu(Input.ReadStringList().Sum(ConvertFromSnafu))}");
}

static long ConvertFromSnafu(string s)
{
	var result = 0L;
	foreach (var c in s)
	{
		result *= 5;
		result += c switch
		{
			'0' => 0,
			'1' => 1,
			'2' => 2,
			'-' => -1,
			'=' => -2,
			_ => throw new InvalidOperationException(),
		};
	}
	return result;
}

static string ConvertToSnafu(long n)
{
	var sb = new StringBuilder();
	do
	{
		sb.Insert(0, (n % 5) switch
		{
			0 => '0',
			1 => '1',
			2 => '2',
			3 => '=',
			4 => '-',
			_ => throw new InvalidOperationException()
		});
		if (n % 5 > 2)
		{
			n = (n / 5) + 1;
		}
		else
		{
			n /= 5;
		}
	}
	while (n != 0);
	return sb.ToString();
}