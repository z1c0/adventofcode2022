namespace aoc
{
	public enum Context
	{
		Sample,
		Full,
	}

	public static partial class Input
	{
		public static Context Context { get; set; }= Context.Full;

		private static string GetFileName() => Context == Context.Full ? "input.txt" : "sample.txt";
		
		public static IEnumerable<char> ReadCharList()
		{
			return File.ReadAllText(GetFileName());
		}

		public static IList<string> ReadStringList() => File.ReadAllLines(GetFileName()).ToList();
		
		public static IEnumerable<int> ReadIntList()
		{
			foreach (var line in ReadStringList())
			{
				yield return int.Parse(line);
			}
		}

		public static IEnumerable<(char Char, int Int)> ReadCharIntList()
		{
			foreach (var line in ReadStringList())
			{
				var tokens = line.Split(' ');
				yield return (tokens[0][0], int.Parse(tokens[1]));
			}
		}

		public static IEnumerable<(string String, int Int)> ReadStringIntList()
		{
			foreach (var line in ReadStringList())
			{
				var tokens = line.Split(' ');
				yield return (tokens[0], int.Parse(tokens[1]));
			}
		}
	}
}
