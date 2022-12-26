var input = File.ReadAllLines("input.txt");

HashSet<(int r, int c)> initialBlizzardPositions = new();
Dictionary<int, (int r, int c, char d)> blizzards = new();

string dirs = "<>^v";

Dictionary<char, (int r, int c)> moves = new()
{
    { '<', (0,-1) },
    { '>', (0,1)},
    { 'v', (1,0) },
    { '^',(-1,0) }
};

var height = input.Length;
var width = input[0].Length;

int repeat = lcm(height - 2, width - 2);

var start = (r: 0, c: input.First().IndexOf('.'));
var end = (r: height - 1, c: input.Last().IndexOf('.'));

//read in initial map
int id = 0;
for (var r = 0; r < height; r++)
{
    for (var c = 0; c < width; c++)
    {
        if (dirs.Contains(input[r][c]))
        {
            blizzards.Add(id++, (r, c, input[r][c]));
            initialBlizzardPositions.Add((r, c));
        }
    }
}

//pre cache all the possible blizzard positions
Dictionary<int, HashSet<(int r, int c)>> blizzardPositionCache = new()
{
    {0, initialBlizzardPositions }
};

for (var i = 1; i < repeat; i++)
{
    blizzardPositionCache[i] = moveBlizzard();
}

bool doPrint = false;
Dictionary<(int r, int c, int t), int?> visited = new();
HashSet<(int r, int c, int t)> visited2 = new();
(int, int) temp;

int minimum = int.MaxValue;
var x = FindShortestPath(start, 0);

Console.WriteLine($"Part 1: {x}");

temp = start;
start = end;
end = temp;
visited.Clear();
visited2.Clear();
minimum = int.MaxValue;
var y = FindShortestPath(start, x.Value);

temp = start;
start = end;
end = temp;
visited.Clear();
visited2.Clear();
minimum = int.MaxValue;
var z = FindShortestPath(start, y.Value + x.Value);

Console.WriteLine($"Part 2: {x}, {y}, {z}: {x+y+z}");

int dist((int r, int c) lhs, (int r, int c) rhs)
{
    return Math.Abs(rhs.r - lhs.r) + Math.Abs(rhs.c - lhs.c);
}
HashSet<(int r, int c)> moveBlizzard()
{
    HashSet<(int r, int c)> newBlizzards = new();

    foreach (var b in blizzards)
    {
        var newPos = (r: b.Value.r + moves[b.Value.d].r, c: b.Value.c + moves[b.Value.d].c);
        if (newPos.r == height - 1)
            newPos.r = 1;
        else if (newPos.r == 0)
            newPos.r = height - 2;
        else if (newPos.c == width - 1)
            newPos.c = 1;
        else if (newPos.c == 0)
            newPos.c = width - 2;

        blizzards[b.Key] = (newPos.r, newPos.c, b.Value.d);
        newBlizzards.Add((newPos.r, newPos.c));
    }
    return newBlizzards;
}

void print((int r, int c) pos, int t, int minTime)
{
    Console.WriteLine($"Time {t}; Shortest {minTime}");
    for (var r = 0; r < Math.Min(10, height); r++)
    {
        for (var c = 0; c < Math.Min(10, width); c++)
        {
            if ((r, c) == pos)
                Console.Write('E');
            else
            {
                if (r == 0 || c == 0 || r == height - 1 || c == width - 1)
                    Console.Write('#');
                else
                {
                    var b = blizzardPositionCache[t%repeat];
                    if(b.Contains((r,c)))
                    {
                        Console.Write('B');
                    }
                    else
                    {
                        Console.Write('.');
                    }
                //
                //    var x = blizzards.Where(y => y.Value.r == r && y.Value.c == c);
                //    if (x.Count() == 1)
                //    {
                //        Console.Write(x.First().Value.d);
                //    }
                //    else if (x.Count() > 1)
                //    {
                //        Console.Write(x.Count());
                //    }
                //    else
                //    {
                //        Console.Write('.');
                //    }
                }
            }
        }
        Console.WriteLine();
    }
    Console.WriteLine();
}

int? FindShortestPath((int r, int c) pos, int absoluteTime)
{
    if (doPrint)
    {
        print(pos, absoluteTime, 0);
        Console.ReadKey();
    }

    if (pos == end)
    {
        minimum = Math.Min(minimum, absoluteTime);
        return 0;
    }

    if (dist(pos, end) + absoluteTime >= minimum)
    {
        return null;
    }

    int relativeTime = absoluteTime % repeat;

    int? time = null;
    if (visited.TryGetValue((pos.r, pos.c, relativeTime), out time))
    {
        return time;
    }

    if (!visited2.Add((pos.r, pos.c, relativeTime)))
    {
        return null;
    }
    

    int nextRelativeTime = (absoluteTime + 1) % repeat;
    var newBlizzards = blizzardPositionCache[nextRelativeTime];

    int? minDist = null;
    int? r;

    foreach (var nextPos in moves.Select(d => (r: pos.r + d.Value.r, c: pos.c + d.Value.c)).Append(pos)
        .Where(p => !newBlizzards.Contains(p) && ((p.r > 0 && p.r < height - 1 && p.c > 0 && p.c < width - 1) || p == start || p == end))
        .OrderBy(p => dist(p, end)))
    {
        r = FindShortestPath(nextPos, absoluteTime + 1);
        if (r != null)
        {
            minDist = minDist == null ? r.Value : Math.Min(r.Value, minDist.Value);
        }
    }

    visited[(pos.r, pos.c, relativeTime)] = minDist != null ? ++minDist : null;
    return visited[(pos.r, pos.c, relativeTime)];
}


int lcm(int a, int b)
{
    if (a < b)
    {
        var temp = a;
        a = b;
        b = temp;
    }

    if (a % b == 0)
        return a;
    else
    {
        for (int i = 1; i < b; i++)
        {
            a += a;
            if (a % b == 0)
                return a;
        }
    }

    return a * b;
}

class State
{
    public (int r, int c) pos { get; init; }
    public int t { get; init; }
}


//Stack<State> stateStack = new();
//stateStack.Push(new State()
//{
//    pos = start,
//    t = 0
//});

//int? FindShortestPath()
//{
//var minTime = int.MaxValue;

//while (stateStack.Count > 0)
//{
//    loopCount++;

//    var s = stateStack.Pop();

//    if (s.pos == end)
//    {
//        var newMinTime = Math.Min(minTime, s.t);
//        Console.WriteLine(newMinTime);
//        minTime = newMinTime;
//        continue;
//    }

//    if (dist(s.pos, end) + s.t >= minTime)
//    {
//        continue;
//    }

//    int key = s.t % repeat;

//    if (!visited.ContainsKey(s.pos))
//    {
//        visited[s.pos] = new();
//    }

//    var visitedList = visited[s.pos];

//    if(visitedList.ContainsKey(key))
//    {
//        if (visitedList[key].Count() > 0)
//        {
//            var vmin = visitedList[key].Min();
//            if (vmin < s.t)
//                continue;
//        }
//    }
//    else
//    {
//        visitedList[key] = new();
//    }

//    visitedList[key].Add(s.t);

//    //if (!visited[s.pos].Add(s.t % repeat))
//    //{     
//    //    continue;
//    //}

//    var newBlizzards = blizzardPositionCache[(s.t+1) % repeat];

//    if (!newBlizzards.Contains(s.pos))
//    {
//        stateStack.Push(new State()
//        {
//            pos = s.pos,
//            t = s.t + 1
//        });
//    }

//    foreach (var checkPos in bmoves
//        .Select(d => (r: s.pos.r + d.Value.r, c: s.pos.c + d.Value.c))
//        .Where(p => !newBlizzards.Contains(p) && p.r > 0 && p.r < height - 1 && p.c > 0 && p.c < width - 1)
//        .OrderByDescending(p => dist(p, end))
//        )
//    {
//        stateStack.Push(new State()
//        {
//            pos = checkPos,
//            t = s.t + 1
//        });
//    }
//}

//return minTime;
//return 0;
//}