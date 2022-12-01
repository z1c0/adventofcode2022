using System.Diagnostics;
using System.Reflection;

namespace aoc
{
	public static class Day
	{
		public static void Run(Action a)
		{
			var name = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location);
			Console.WriteLine($"START {name}");
			var sw = Stopwatch.StartNew();
			try
			{
				a();
			}
			catch (Exception e)
			{
				Console.WriteLine($"ERROR: {e}");
			}
			finally
			{
				Console.WriteLine($"END (after {sw.Elapsed.TotalSeconds} seconds)");
			}
		}
	}
}