using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;

var lines = File.ReadAllLines("input.txt");

Dictionary<string, Node> nodes = new();
HashSet<Node> openableNodes = new();

foreach (var line in lines)
{
    var n = line.Substring(0, 2);
    var node = new Node()
    {
        name = n,
        rate = int.Parse(line.Substring(3, line.IndexOf(';') - 3)),
        open = false
    };
    nodes.Add(n, node);
    if(node.rate > 0)
    {
        openableNodes.Add(node);
    }
}

foreach (var line in lines)
{
    nodes[line.Substring(0, 2)].neighbors = new HashSet<Node>(line.Split(';')[1].Split(',').Select(nodeName => nodes[nodeName]));
}

Dictionary<(Node a, Node b), int> distanceCache = new();


var yasdfa = findMaxPressure(nodes["AA"], openableNodes, 0);

Console.WriteLine(yasdfa);

long findMaxPressure(Node cur, HashSet<Node> openable, int minutes)
{
    long maxPressure = 0;
    var os = openable.Where(n => cur != n).Select(n => (n, distance(cur, n))).ToList();
    
    var release = (30 - minutes) * cur.rate;

    if (os.Count == 0)
    {
        return release;
    }

    foreach (var (node, d) in os)
    {
        if (d + 2 + minutes <= 30)
        {
            openable.Remove(node);
            maxPressure = Math.Max(maxPressure, findMaxPressure(node, openable, minutes + d + 1));
            openable.Add(node);
        }
    }

    return release + maxPressure;
}

int distance(Node start, Node end)
{
    var minDist = int.MaxValue;
    if (distanceCache.TryGetValue((start, end), out minDist))
    {
        return minDist;
    }    
    else
    {
        Dictionary<Node, int> dv = new() { { start, 0 } };
        minDist = distanceHelper(start, end, 0, dv);
        distanceCache.Add((start, end), minDist);
        distanceCache.Add((end, start), minDist);
        return minDist;
    }
}

int distanceHelper(Node start, Node end, int d, Dictionary<Node,int> dv)
{
    int minD = int.MaxValue;

    if(start.neighbors.Contains(end))
    {
        return d + 1;
    }

    foreach (var n in start.neighbors)
    {
        if (dv.ContainsKey(n))
        {
            if(dv[n] < d + 1)   
                continue;
            else
                dv[n] = d + 1;
        }
        else
        {
            dv.Add(n, d + 1);
        }
        var nd = distanceHelper(n, end, d + 1, dv);
        minD = Math.Min(minD, nd);
    }

    return minD;
}

class Node {
    public string name;
    public bool open;
    public int rate;
    public HashSet<Node> neighbors = new();
}