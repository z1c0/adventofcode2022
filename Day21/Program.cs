using System.Diagnostics;
using aoc;

Day.Run(() =>
{
	Part1();
	Part2();
});

static void Part1()
{
	var monkeys = ReadInput();
	var root = Resolve("root", monkeys);
	Console.WriteLine($"Monkey '{root.Name}' will yell: {root.Value}.");
}

static void Part2()
{
	var monkeys = ReadInput();
	var root = monkeys["root"];
	root.Operator = Operator.Match;
	var target = Find(root, monkeys, int.MinValue);
	Console.WriteLine($"I yell: {target}.");
}

static long Find(Monkey monkey, Dictionary<string, Monkey> monkeys, long target)
{
	if (monkey.IsHuman)
	{
		return target;
	}
	var left = monkeys[monkey.Operand1!];
	var right = monkeys[monkey.Operand2!];
	var (resolve, isLeft) = left.HasHumanOperand(monkeys) ? (right.Name, true) : (left.Name, false);
	target = GetTarget(monkey.Operator, target, Resolve(resolve, monkeys).Value!.Value, !isLeft);
	return Find(isLeft ? left : right, monkeys, target);
}

static long GetTarget(Operator op, long parentValue, long childValue, bool isLeft)
{
	if (op == Operator.Match)
	{
		return childValue;
	}
	if (op == Operator.Add)
	{
		return parentValue - childValue;
	}
	if (op == Operator.Multiply)
	{
		return parentValue / childValue;
	}
	if (op == Operator.Subtract)
	{
		return isLeft ? childValue - parentValue : childValue + parentValue;
	}
	if (op == Operator.Divide)
	{
		return isLeft ? childValue / parentValue : childValue * parentValue;
	}
	throw new InvalidOperationException();
}

static Monkey Resolve(string name, Dictionary<string, Monkey> monkeys)
{
	var monkey = monkeys[name];
	if (!monkey.Value.HasValue)
	{
		var v1 = Resolve(monkey.Operand1!, monkeys).Value;
		var v2 = Resolve(monkey.Operand2!, monkeys).Value;
		monkey.Value = monkey.Operator switch
		{
			Operator.Add => v1 + v2,
			Operator.Subtract => v1 - v2,
			Operator.Multiply => v1 * v2,
			Operator.Divide => v1 / v2,
			Operator.Match => v1 == v2 ? 1 : 0,
			_ => throw new InvalidOperationException(),
		};
	}
	return monkey;
}

static Dictionary<string, Monkey> ReadInput()
{
	var dict = new Dictionary<string, Monkey>();
	foreach (var line in Input.ReadStringList())
	{
		var tokens = line.Split(": ");
		var name = tokens.First();
		long? value = null;
		var operand1 = string.Empty;
		var operand2 = string.Empty;
		var @operator = Operator.None;
		tokens = tokens.Last().Split(' ');
		if (tokens.Length == 1)
		{
			value = int.Parse(tokens.First());
		}
		else
		{
			operand1 = tokens[0];
			operand2 = tokens[2];
			@operator = tokens[1].Single() switch
			{
				'+' => Operator.Add,
				'-' => Operator.Subtract,
				'*' => Operator.Multiply,
				'/' => Operator.Divide,
				_ => throw new InvalidOperationException()
			};
		}

		dict.Add(name, new(name)
		{	
			Value = value,
			Operand1 = operand1,
			Operand2 = operand2,
			Operator = @operator,
		});
	}
	return dict;
}

internal class Monkey
{
	internal Monkey(string name)
	{
		Name = name;		
	}
	internal string Name { get; }
	internal string? Operand1 { get; init; }
	internal string? Operand2 { get; init; }
	internal long? Value { get; set; }
	internal Operator Operator { get; set; }

	public override string ToString()
	{
		var op = Operator switch
		{
			Operator.Add => '+',
			Operator.Subtract => '-',
			Operator.Multiply => '*',
			Operator.Divide => '/',
			Operator.Match => '=',
			Operator.None => '?',
			_ => throw new InvalidOperationException(),
		};
		var s = Value.HasValue ? Value.Value.ToString() : $"{Operand1} {op} {Operand2}";
		return $"{Name} - {s}";
	}

	internal bool IsHuman => Name == "humn";

	internal bool HasHumanOperand(Dictionary<string, Monkey> monkeys)
	{
		if (IsHuman)
		{
			return true;
		}
		if (Value.HasValue)
		{
			return false;
		}
		return
			monkeys[Operand1!].HasHumanOperand(monkeys) ||
			monkeys[Operand2!].HasHumanOperand(monkeys);
	}
}

internal enum Operator
{
	None,
	Add,
	Subtract,
	Multiply,
	Divide,
	Match,
}