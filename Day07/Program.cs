var input = File.ReadAllLines("input.txt").ToList();

Node root = new('d', "/", null);

Stack<Node> stack = new();
stack.Push(root);

Dictionary<string, Node> dirs = new()
{
    { "/", root }
};

//reference to top of stack
Node current = root;

foreach (var line in input.Skip(1))
{
    var o = line.Split(' ');
    switch (o[0])
    {
        case "$":
            //ignore "ls"
            if (o[1] == "cd")
            {
                if (o[2] == "..")
                {
                    stack.Pop();
                    current = stack.Peek();
                }
                else
                {
                    //this assumes that if we are CDing to a directory we've already seen it from an LS and it's in our dictionary
                    current = dirs[$"{current.FullName}/{o[2]}"];
                    stack.Push(current);
                }
            }
            break;
        case "dir":
            //directory record from ls
            if (!current.Children.ContainsKey($"{current.FullName}/{o[1]}"))
            {
                var n = new Node('d', o[1], stack.Peek());
                current.Children.Add(n.FullName, n);
                dirs.Add(n.FullName, n);
            }
            break;
        default:
            //file record from ls
            if (!current.Children.ContainsKey($"{current.FullName}/{o[1]}"))
            {
                var n = new Node('f', o[1], int.Parse(o[0]), current);
                current.Children.Add(n.FullName, n);
            }
            break;
    }
}

Console.WriteLine($"Part 1: {dirs.Where(x => x.Value.Size <= 100000)
        .Sum(y => y.Value.Size)}");

var spaceAvailable = 70000000 - root.Size;
var spaceNeeded = 30000000;

var minDeleteSize = spaceNeeded - spaceAvailable;

Console.WriteLine($"Part 2: {dirs.Where(x => x.Value.Size > minDeleteSize)
        .Min(x => x.Value.Size)}");

public class Node
{
    public char NodeType { get; init; }
    public Node? Parent { get; init; }
    public Dictionary<string, Node> Children { get; } = new Dictionary<string, Node>();
    public string Name { get; init; }

    public string FullName
    {
        get
        {
            return $"{(Parent?.FullName ?? "")}/{Name}";
        }
    }

    private readonly int size;

    public int Size
    {
        get
        {
            if (NodeType == 'f')
                return size;
            else
                return Children.Sum(x => x.Value.Size);
        }
    }

    public Node(char nodeType, string name, int size, Node? parent)
        : this(nodeType, name, parent)
    {
        this.size = size;
    }
    public Node(char nodeType, string name, Node? parent)
    {
        NodeType = nodeType;
        Name = name;
        Parent = parent;
    }
}