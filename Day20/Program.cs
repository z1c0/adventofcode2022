using aoc;

Day.Run(() =>
{
	Part1(1, 1);
	Part1(811589153, 10);
});

static void Part1(long decryptionKey, int rounds)
{
	var n = 0;
	var original = Input.ReadIntList().Select(v => (n++, v * decryptionKey)).ToList();
	var list = original.ToList();
	for (var r = 0; r < rounds; r++)
	{
		foreach (var t in original)
		{
			var v = t.Item2;
			var from = list.IndexOf(t);
			if (v != 0)
			{
				var to = from;
				list.RemoveAt(from);
				if (to >= list.Count)
				{
					to = 0;
				}
				if (to < 0)
				{
					to = list.Count - 1;
				}

				v %= list.Count;
				if (v > 0)
				{
					for (var i = 0; i < v; i++)
					{
						to++;
						if (to >= list.Count)
						{
							to = 0;
						}
					}
				}
				else if (v < 0)
				{
					for (var i = 0; i < Math.Abs(v); i++)
					{
						to--;
						if (to < 0)
						{
							to = list.Count - 1;
						}
					}
				}
				list.Insert(to, t);
			}
		}
	}

	var zero = list.IndexOf(list.Single(i => i.Item2 == 0));
	var c1 = list[(zero + 1000) % list.Count].Item2;
	var c2 = list[(zero + 2000) % list.Count].Item2;
	var c3 = list[(zero + 3000) % list.Count].Item2;
	Console.WriteLine($"{c1} + {c2} + {c3} = {c1 + c2 + c3}");
}
