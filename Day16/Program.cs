var stopwatch = new System.Diagnostics.Stopwatch();
stopwatch.Start();
var lines = File.ReadAllLines("input.txt");

Dictionary<string, Node> nodes = new();
HashSet<Node> openableNodes = new();
Dictionary<(string, string, int, int), (long pressure, List<Node> visited)> pressureMemo = new();
long memocount = 0;

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

var p1 = maxP(30, openableNodes);
Console.WriteLine($"Part 1: {p1.pressure}");

var subsets = openableNodes.GetSubsets().OrderBy(x => x.Count).ToList();
SortedSet<long> ls = new();
for(var s = 0; s < subsets.Count; s++)
{
    var p21 = maxP(26, subsets[s]);
    var st = new HashSet<Node>(openableNodes);
    st.ExceptWith(p21.visited);
    var p22 = maxP(26, st);

    ls.Add(p21.pressure + p22.pressure);
}

Console.WriteLine($"Part 1: {ls.Max()}");

stopwatch.Stop();

//Console.WriteLine(stopwatch.Elapsed.TotalSeconds);


(long pressure, List<Node> visited) maxP(int minutes, HashSet<Node> openableNodes)
{

    return findMaxPressure(nodes["AA"], openableNodes, 0, minutes);
}


(long pressure, List<Node> visited) findMaxPressure(Node cur, HashSet<Node> openable, int minutes, int maxTime)
{
    var key = (cur.name, string.Join(' ',openable.OrderBy(x => x.name).Select(x => x.name)), minutes, maxTime);
    if (pressureMemo.ContainsKey(key))
    {
        memocount++;
        return pressureMemo[key];
    }

    (long pressure, List<Node> visited) maxPressure = (0L, new());
    var os = openable.Where(n => n != cur)
        .Select(n => (node: n, distance: distance(cur, n)))
        .Where(n => n.distance + 2 + minutes <= maxTime)
        .ToList();

    var release = (maxTime - minutes) * cur.rate;

    if (os.Count == 0)
    {
        (long, List<Node>) r2 = (release, new() { cur });
        pressureMemo.Add(key, r2);
        return r2;
    }

    foreach (var (node, d) in os)
    {
        openable.Remove(node);
        var r = findMaxPressure(node, openable, minutes + d + 1, maxTime);
        if (r.pressure > maxPressure.pressure)
        {
            maxPressure = r;
        }
        openable.Add(node);
    }
    (long, List<Node>) r3 = (release + maxPressure.pressure, maxPressure.visited.Append(cur).ToList());
    pressureMemo.Add(key, r3);
    return r3;
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

void printSet<T>(HashSet<T> set)
{
    Console.Write("{");
    Console.Write(string.Join(",", set));
    Console.Write("}");
    Console.WriteLine();
}

class Node
{
    public string name;
    public bool open;
    public int rate;
    public HashSet<Node> neighbors = new();
}

static class Extensions
{
    public static IEnumerable<HashSet<T>> GetSubsets<T>(this HashSet<T> set)
    {
        for(var i = 0; i < Math.Pow(2, set.Count()); i++)
        {
            HashSet<T> tmp = new();
            for (var j = 0; j < set.Count();j++)
            {
                if ((i >> j & 1) == 1)
                    tmp.Add(set.ElementAt(j));
            }
            yield return tmp;
        }
    }

}