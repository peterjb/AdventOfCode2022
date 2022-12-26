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
    if (node.rate > 0)
    {
        openableNodes.Add(node);
    }
}

foreach (var line in lines)
{
    nodes[line.Substring(0, 2)].neighbors = new HashSet<Node>(line.Split(';')[1].Split(',').Select(nodeName => nodes[nodeName]));
}

Dictionary<(Node a, Node b), int> distanceCache = new();

var p1 = maxP(30);
Console.WriteLine($"Part 1: {p1.pressure}");

var p21 = maxP(26);
openableNodes.ExceptWith(p21.visited);
var p22 = maxP(26);
Console.WriteLine($"Part 2: {p21.pressure} + {p22.pressure} = {p21.pressure + p22.pressure}");

(long pressure, List<Node> visited) maxP(int minutes)
{

    return findMaxPressure(nodes["AA"], openableNodes, 0, minutes);
}

(long pressure, List<Node> visited) findMaxPressure(Node cur, HashSet<Node> openable, int minutes, int maxTime)
{
    (long pressure, List<Node> visited) maxPressure = (0L, new());
    var os = openable.Where(n => n != cur)
        .Select(n => (node: n, distance: distance(cur, n)))
        .Where(n => n.distance + 2 + minutes <= maxTime)
        .ToList();

    var release = (maxTime - minutes) * cur.rate;

    if (os.Count == 0)
    {
        return (release, new() { cur });
    }

    foreach (var (node, d) in os)
    {
        openable.Remove(node);
        var r = findMaxPressure(node, openable, minutes + d + 1, maxTime);
        if(r.pressure > maxPressure.pressure)
        {
            maxPressure = r; 
        }
        openable.Add(node);
    }

    return (release + maxPressure.pressure, maxPressure.visited.Append(cur).ToList());
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

int distanceHelper(Node start, Node end, int d, Dictionary<Node, int> dv)
{
    int minD = int.MaxValue;

    if (start.neighbors.Contains(end))
    {
        return d + 1;
    }

    foreach (var n in start.neighbors)
    {
        if (dv.ContainsKey(n))
        {
            if (dv[n] < d + 1)
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

class Node
{
    public string name;
    public bool open;
    public int rate;
    public HashSet<Node> neighbors = new();
}