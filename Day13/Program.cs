using System.Text;

var pairs = File.ReadAllText("input.txt").Split($"{Environment.NewLine}{Environment.NewLine}").ToArray();

var rightOrderCount = 0;

List<node> packetList = new();

var comparer = new nodeComparer();

for (var n = 0; n < pairs.Length; n++)
{
    var pair = pairs[n];

    var packetNodes = new node[2];

    var packets = pair.Split(Environment.NewLine);

    for (var i = 0; i < packets.Length; i++)
    {
        Stack<node> nstack = new Stack<node>();

        var packet = new node();
        var currentNode = packet;

        for(var j = 1; j < packets[i].Length - 1; j++) 
        {
            var c = packets[i][j];
            switch (c)
            {
                case ',':
                    break;
                case '[':
                    var newNode = new node();
                    currentNode.children.Add(newNode);
                    nstack.Push(currentNode);
                    currentNode = newNode;
                    break;
                case ']':
                    currentNode = nstack.Pop();
                    break;
                default:
                    var b = new StringBuilder();
                    b.Append(c);
                    while (char.IsDigit(packets[i][j+1]))
                    {   
                        j++;
                        c = packets[i][j];
                        b.Append(c);
                    }
                    currentNode.children.Add(new node() { value = int.Parse(b.ToString()) });
                    break;
            }
        }
        packetNodes[i] = packet;
        packetList.Add(packet);
    }

    var lhs = packetNodes[0];
    var rhs = packetNodes[1];

    if (comparer.compare(lhs, rhs) < 0)
        rightOrderCount+=(n+1);

}

Console.WriteLine(rightOrderCount);


var divider1 = new node();
var divider1v = new node();
divider1v.children.Add(new node() { value = 2 });
divider1.children.Add(divider1v);

var divider2 = new node();
var divider2v = new node();
divider2v.children.Add(new node() { value = 6 });
divider2.children.Add(divider2v);

packetList.Add(divider1);
packetList.Add(divider2);

packetList.Sort(comparer);

var d1i = packetList.FindIndex(x => comparer.compare(x, divider1) == 0) + 1;
var d2i = packetList.FindIndex(x => comparer.compare(x, divider2) == 0) + 1;

Console.WriteLine(d1i * d2i); 

class node
{
    public int? value;
    public List<node> children = new List<node>();
}

class nodeComparer : IComparer<node>
{
    public int Compare(node? x, node? y)
    {
        return compare(x, y);
    }

    public int compare(node lhs, node rhs)
    {
        if (lhs.value != null && rhs.value != null)
        {
            return lhs.value.Value.CompareTo(rhs.value.Value);
        }
        else if (lhs.value is null && rhs.value is null)
        {
            for (var i = 0; i < lhs.children.Count && i < rhs.children.Count; i++)
            {
                var r = compare(lhs.children[i], rhs.children[i]);
                if (r == 0)
                {
                    continue;
                }
                else
                {
                    return r;
                }
            }
            return lhs.children.Count.CompareTo(rhs.children.Count);
        }
        else if (lhs.value is null)
        {
            var cnode = new node();
            cnode.children.Add(rhs);
            return compare(lhs, cnode);
        }
        else
        {
            var cnode = new node();
            cnode.children.Add(lhs);
            return compare(cnode, rhs);
        }
    }
}
