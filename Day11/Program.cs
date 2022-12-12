using System.Globalization;
using aoc;

Day.Run(() =>
{
	Part1(20, true);
	Part1(10000, false);
});

static void Part1(int rounds, bool simplify)
{
	var monkeys = ReadInput().ToList();
	for (var round = 0; round < rounds; round++)
	{
		foreach (var m in monkeys)
		{
			m.Simulate(monkeys, simplify);
		}
	}
	var inspections = monkeys.Select(m => m.Inspections).OrderDescending().ToList();
	Console.WriteLine($"Monkey business level: {inspections[0]} * {inspections[1]} = {inspections[0] * inspections[1]}");
}

static IEnumerable<Monkey> ReadInput()
{
	var lines = Input.ReadStringList().ToList();
	var i = 0;
	while (i < lines.Count)
	{
		var parts = lines[i++].Split(' ');
		var id = int.Parse(parts.Last()[..^1]);

		parts = lines[i++].Split(": ");
		var items = parts.Last().Split(", ").Select(p => new Item(long.Parse(p))).ToList();

		parts = lines[i++].Split("= ");
		parts = parts.Last().Split(' ');

		var useOld = parts.Last() == "old";
		var operation = (
			parts[1] switch
			{
				"*" => Operator.Multiply,
				"+" => Operator.Add,
				_ => throw new InvalidOperationException("operator"),
			},
			useOld,
			useOld ? -1 : int.Parse(parts.Last())
		);

		parts = lines[i++].Split(' ');
		var divideBy = int.Parse(parts.Last());
		
		parts = lines[i++].Split(' ');
		var ifTrue = int.Parse(parts.Last());

		parts = lines[i++].Split(' ');
		var ifFalse = int.Parse(parts.Last());

		i++;

		yield return new(
			id,
			items,
			operation,
			divideBy,
			ifTrue,
			ifFalse
		);
	}
}

internal class Monkey
{
	private (Operator Operator, bool UseOld, long Value) _operation;
	private readonly int _ifTrue;
	private readonly int _ifFalse;
	private readonly long _divideBy;

	public Monkey(int id, List<Item> items, (Operator, bool useOld, int) operation, int divideBy, int ifTrue, int ifFalse)
	{
		Id = id;
		Items = items;
		_operation = operation;
		_divideBy = divideBy;
		_ifTrue = ifTrue;
		_ifFalse = ifFalse;
	}

	internal int Id { get; }
	internal List<Item> Items { get; }
	internal long Inspections { get; private set; }

	public override string ToString()
	{
		return $"Monkey {Id}: {string.Join(", ", Items)}";
	}

	internal void Simulate(List<Monkey> monkeys, bool simplify)
	{
		foreach (var i in Items)
		{
			Inspections++;
			i.Add(_operation);
			
			if (simplify)
			{
				var v = i.Evaluate();
				i.Set(v / 3);
			}

			var to = i.Test(_divideBy) ? _ifTrue : _ifFalse;
			monkeys[to].Items.Add(i);
		}
		Items.Clear();
	}
}

internal class Item
{
	private readonly List<(Operator Operator, bool UseOld, long Value)> _operations = new();
	
	internal Item(long value)
	{
		Set(value);
	}

	internal void Add((Operator Operator, bool UseOld, long Value) operation)
	{
		_operations.Add(operation);
	}

	internal long Evaluate()
	{
		var result = 0L;
		foreach (var o in _operations)
		{
			result = Evaluate(o, result);
		}
		return result;
	}

	private static long Evaluate((Operator Operator, bool UseOld, long Value) o, long result)
	{
		var v = o.UseOld ? result : o.Value;
		return o.Operator switch
		{
			Operator.Add =>	result + v,
			Operator.Multiply => result * v,
			_ => throw new InvalidOperationException(),
		};
	}

	internal void Set(long value)
	{
		_operations.Clear();
		_operations.Add((Operator.Add, false, value));
	}

	internal bool Test(long divideBy)
	{
		var result = 0L;
		foreach (var o in _operations)
		{
			result = Evaluate(o, result) % divideBy;
		}
		return result == 0;
	}
}

internal enum Operator
{
	Multiply,
	Add,
}
