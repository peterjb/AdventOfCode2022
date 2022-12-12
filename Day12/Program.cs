var input = File.ReadAllLines("input.txt").ToArray();

int h = input.Length;
int w = input[0].Length;

char[,] grid = new char[h, w];

(int, int) start = (0, 0);
(int, int) end = (0, 0);

for (var r = 0; r < h; r++)
{
    for (var c = 0; c < w; c++)
    {
        var elev = input[r][c];

        if (elev == 'S')
        {
            elev = 'a';
            start = (r, c);
        }
        if (elev == 'E')
        {
            elev = 'z';
            end = (r, c);
        }

        grid[r, c] = elev;
    }
}

List<(int, int)> startingSpots = new();

for (var r = 0; r < h; r++)
{
    for (var c = 0; c < w; c++)
    {
        if (input[r][c] == 'a')
        {
            startingSpots.Add((r, c));
        }
    }
}

int count = 0;

Console.WriteLine($"Part 1: {run(start)}");

//brute force but it only takes a minute or two

Dictionary<(int, int), int> values = new();

foreach (var s in startingSpots)
{
    values.Add(s, run(s));
}

var shortest = values.OrderBy(x => x.Value).First();
Console.WriteLine($"Part 2: {shortest.Key}: {shortest.Value}");

int run((int, int) a)
{
    count++;
    Dictionary<(int, int), int> visited = new()
    {
        { a, 0 }
    };

    visit(a, 0, visited);

    if (!visited.ContainsKey(end))
    {
        //Console.WriteLine($"{count}: {a}: No Path");
        return int.MaxValue;
    }

    //Console.WriteLine($"{count}: {a}: {visited[end]}");

    return visited[end];
}

void visit((int r, int c) pos, int pathLength, Dictionary<(int, int), int> visited)
{
    var dirs = new List<(int r, int c)>()
    {
        (-1,0),(1,0),(0,-1),(0,1)
    };

    foreach (var dir in dirs)
    {
        var newPos = (r: pos.r + dir.r, c: pos.c + dir.c);
        if (newPos.r >= 0 && newPos.r < h && newPos.c >= 0 && newPos.c < w)
        {
            var current = grid[pos.r, pos.c];
            var next = grid[newPos.r, newPos.c];

            if (next <= current + 1)
            {
                if (!visited.ContainsKey(newPos) || visited[newPos] > pathLength + 1)
                {
                    visited[newPos] = pathLength + 1;
                    if (next == 'E')
                        return; //pathLength + 1;
                    else
                        visit(newPos, pathLength + 1, visited);
                }
            }
        }
    }
}




