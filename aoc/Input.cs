namespace aoc
{
	public static partial class Input
	{
		private const string DEFAULT_INPUT_FILENAME = "input.txt";
		
		public static IEnumerable<int> ReadIntList(string fileName = DEFAULT_INPUT_FILENAME)
		{
			foreach (var line in File.ReadAllLines(fileName))
			{
				yield return int.Parse(line);
			}
		}

		public static IEnumerable<(string String, int Int)> ReadStringIntList(string fileName = DEFAULT_INPUT_FILENAME)
		{
			foreach (var line in File.ReadAllLines(fileName))
			{
				var tokens = line.Split(' ');
				yield return (tokens[0], int.Parse(tokens[1]));
			}
		}
	}
}
