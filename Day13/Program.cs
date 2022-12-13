using System.Diagnostics;
using aoc;

Day.Run(() =>
{
	Part1();
	Part2();
});

static void Part1()
{
	var sum = 0L;
	var index = 0;
	foreach (var (packet1, packet2) in ReadInput())
	{
		index++;
		if (packet1.CompareTo(packet2) < 0)
		{
			sum += index;
		}
	}
	Console.WriteLine($"Sum: {sum}");
}

static void Part2()
{
	var key = 1;
	var packets = ReadInput()
		.SelectMany(p => new[] { p.packet1, p.packet2 })
		.Concat(Packet.CreateDividers())
		.Order()
		.ToList();
	for (var i = 0; i < packets.Count; i++)
	{
		if (packets[i].IsDivider)
		{
			key *= i + 1;
		}
	}
	Console.WriteLine($"Decoder key: {key}");
}


static IEnumerable<(Packet packet1, Packet packet2)> ReadInput()
{
	var lines = Input.ReadStringList().ToList();
	var i = 0;
	while (i < lines.Count)
	{
		var packet1 = Packet.Parse(lines[i++]);
		var packet2 = Packet.Parse(lines[i++]);
		yield return (packet1, packet2);
		i++;
	}
}

internal class Packet : IComparable
{
	private readonly int _number;
	private readonly List<Packet>? _list;

	private Packet(int number)
	{
		_number = number;
	}

	private Packet(List<Packet> list)
	{
		_list = list;
	}

	internal static IEnumerable<Packet> CreateDividers()
	{
		yield return new(new List<Packet> { new(2) }) { IsDivider = true };
		yield return new(new List<Packet> { new(6) }) { IsDivider = true };
	}

	internal bool IsDivider { get; private set; }

	internal static Packet Parse(string s)
	{
		var pos = 0;
		return Parse(s, ref pos);
	}

	private static Packet Parse(string s, ref int pos)
	{
		Packet packet;
		if (s[pos] == '[')
		{
			pos++;
			var list = new List<Packet>();
			while (s[pos] != ']')
			{
				var p = Parse(s, ref pos);
				list.Add(p);
				if (s[pos] == ',')
				{
					pos++;
				}
			}
			pos++;
			packet = new Packet(list);
		}
		else
		{
			Debug.Assert(char.IsDigit(s[pos]));
			var v = s[pos++] - '0';
			if (char.IsDigit(s[pos]))
			{
				v *= 10;
				v += s[pos++] - '0';
			}
			packet = new Packet(v);
		}
		return packet;
	}

	public int CompareTo(object? obj)
	{
		if (obj is not Packet other)
		{
			throw new InvalidOperationException();
		}
		if (_list == null && other._list == null)
		{
			return _number.CompareTo(other._number);
		}
		else if (_list != null && other._list != null)
		{
			return CompareLists(_list, other._list);
		}
		else if (_list != null && other._list == null)
		{
			return CompareLists(_list, new() { other });
		}
		else if (_list == null && other._list != null)
		{
			return CompareLists(new() { this }, other._list);
		}
		return 0;
	}

	private static int CompareLists(List<Packet> list1, List<Packet> list2)
	{
		var l1 = list1.Count;
		var l2 = list2.Count;
		for (var i = 0; i < Math.Min(l1, l2); i++)
		{
			var c = list1[i].CompareTo(list2[i]);
			if (c < 0)
			{
				return -1;
			}
			else if  (c > 0)
			{
				return 1;
			}
		}
		if (l2 > l1)
		{
			return -1;
		}
		else if (l1 > l2)
		{
			return 1;
		}
		return 0;
	}

	public override string ToString()
	{
		if (_list != null)
		{
			return $"[{string.Join(',', _list)}]";
		}
		return _number.ToString();
	}
}

