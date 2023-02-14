using System.Diagnostics;
using System.Reflection;

namespace aoc
{
	public static class Day
	{
		public static void Run(Action a)
		{
			var name = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location);
			Print("---------------------------------------------------------", ConsoleColor.Cyan);
			Print($"{name}", ConsoleColor.Cyan);
			Print("---------------------------------------------------------", ConsoleColor.Cyan);
			var sw = Stopwatch.StartNew();
			try
			{
				a();
			}
			catch (Exception e)
			{
				Print($"ERROR: {e}", ConsoleColor.Red);
			}
			finally
			{
				Print("---------------------------------------------------------", ConsoleColor.Cyan);
				Print($"Finished after {sw.Elapsed.TotalSeconds} seconds.", ConsoleColor.Cyan);
				Print("---------------------------------------------------------", ConsoleColor.Cyan);
			}
		}

		public static void Run<TInput, TParam, TResult>(
			Func<TInput> funcInput,
			(Func<TInput, TParam, TResult> func, TParam param, TResult sampleResult, TResult fullResult) part1,
			(Func<TInput, TParam, TResult> func, TParam param, TResult sampleResult, TResult fullResult) part2)
		{
			Run(() =>
			{
				void Run(Context context, TResult result1, TResult result2)
				{
					Input.Context = context;
					Check(part1.func(funcInput(), part1.param), result1);
					Check(part2.func(funcInput(), part2.param), result2);
				}
				Run(Context.Sample, part1.sampleResult, part2.sampleResult);
				Run(Context.Full, part1.fullResult, part2.fullResult);
			});
		}

		public static void Run<TInput, TResult>(
			Func<TInput> funcInput,
			(Func<TInput, TResult> func, TResult sampleResult, TResult fullResult) part1,
			(Func<TInput, TResult> func, TResult sampleResult, TResult fullResult) part2)
		{
			Run(() =>
			{
				void Run(Context context, TResult result1, TResult result2)
				{
					Input.Context = context;
					Check(part1.func(funcInput()), result1);
					Check(part2.func(funcInput()), result2);
				}
				Run(Context.Sample, part1.sampleResult, part2.sampleResult);
				Run(Context.Full, part1.fullResult, part2.fullResult);
			});
		}

		private static void Check<TResult>(TResult actual, TResult expected)
		{
			if (!actual!.Equals(expected))
			{
				Print($"Expected '{expected}', actual: '{actual}'", ConsoleColor.Red);
			}
			else
			{
				Print("Part 1 ✅", ConsoleColor.Green);
			}
		}

		private static void Print(string msg, ConsoleColor color)
		{
				var c = Console.ForegroundColor;
				Console.ForegroundColor = color;
				Console.WriteLine(msg);
				Console.ForegroundColor = c;
		}
	}
}