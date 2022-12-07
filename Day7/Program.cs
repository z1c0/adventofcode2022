using System.Diagnostics;
using aoc;

Day.Run(() =>
{
	var sizes = GetSortedDirectorySizes();
	Part1(sizes);
	Part2(sizes);
});

static void Part1(List<long> sizes)
{
	var sum = sizes.Where(s => s <= 100000).Sum();
	Console.WriteLine($"Total size: {sum}");
}

static void Part2(List<long> sizes)
{
	var size = sizes.First(s => 70000000 - sizes.Last() + s >= 30000000);
	Console.WriteLine($"The smallest directory size to delete: {size}");
}

static ElfFile ReadInput()
{
	var isListing = false;

	var root = new ElfFile("/", true);
	var current = root;

	foreach (var line in Input.ReadStringList())
	{
		Debug.Assert(current != null);
		Debug.Assert(current.IsDirectory);

		var parts = line.Split(' ');
		if (parts.First() == "$")
		{
			var cmd = parts[1];
			if (cmd == "cd")
			{
				isListing = false;
				var name = parts[2];
				if (name == "/")
				{
					current = root;
				}
				else if (name == "..")
				{
					current = current?.Parent;
				}
				else
				{
					current = current?.GetChild(name);
				}
			}
			else if (cmd == "ls")
			{
				isListing = true;
			}
			else
			{
				throw new InvalidOperationException(cmd);
			}
		}
		else
		{
			Debug.Assert(isListing);
			var isDirectory = !long.TryParse(parts.First(), out var size);
			current?.AddChild(new (parts[1], isDirectory, size));
		}
	}

	return root;
}

static List<long> GetSortedDirectorySizes()
{
	var sizes = new List<long>();
	void Visit(ElfFile file)
	{
		if (file.IsDirectory)
		{
			sizes.Add(file.GetSize());
			foreach (var c in file.GetChildren())
			{
				Visit(c);
			}
		}
	}
	Visit(ReadInput());
	sizes.Sort();
	return sizes;
}

internal class ElfFile
{
	private readonly List<ElfFile> _children = new();
	private long? _size;

	internal ElfFile(string name, bool isDirectory, long size = 0)
	{
		Name = name;
		IsDirectory = isDirectory;
		if (!isDirectory)
		{
			_size = size;
		}
	}

	public string Name { get; }

	public bool IsDirectory { get; }

	public ElfFile? Parent { get; private set; }

	internal void AddChild(ElfFile child)	
	{
		child.Parent = this;
		_children.Add(child);
	}

	internal IReadOnlyList<ElfFile> GetChildren() => _children;
	internal ElfFile GetChild(string name) => _children.Single(c => c.Name == name);

	internal long GetSize()
	{
		if (!_size.HasValue)
		{
			Debug.Assert(IsDirectory);
			_size = 0L;
			foreach (var c in _children)
			{
				_size += c.GetSize();
			}
		}
		return _size.Value;
	}
}
