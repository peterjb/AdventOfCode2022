using System.Runtime.CompilerServices;
using System.Xml.Serialization;

var rock = File.ReadAllLines("test.txt");

List<(int x, int y)> points = new();
List<((int x, int y) start, (int x, int y) end)> lines = new();
HashSet<(int x, int y)> blocks = new();
HashSet<(int x, int y)> sandBlocks = new();

foreach(var formation in rock)
{
    var segments = formation.Split("->");

    var p = segments[0].Trim().Split(","); 
    var point = (int.Parse(p[0]), int.Parse(p[1]));
    points.Add(point);

    for(var i = 1; i < segments.Length; i++)
    {
        var np = segments[i].Trim().Split(",");
        var nextpoint = (int.Parse(np[0]), int.Parse(np[1]));
        points.Add(nextpoint);
        lines.Add((point, nextpoint));
        point = nextpoint;
    }
}

var topLeft = (x: points.MinBy(p => p.Item1).Item1, y: points.MinBy(p => p.Item2).Item2);
var bottomRight = (x: points.MaxBy(p => p.Item1).Item1, y: points.MaxBy(p => p.Item2).Item2);

var h = bottomRight.y + 1;
var w = bottomRight.x - topLeft.x + 1;

var grid = new char[h, w];

part2();
void part2()
{
    foreach (var line in lines)
    {
        bool horizontal = false;
        (int x, int y) start = line.start;
        (int x, int y) end = line.end;

        if (line.start.x != line.end.x)
        {
            horizontal = true;
            if (line.start.x > line.end.x)
            {
                start = line.end;
                end = line.start;
            }
        }
        else if (line.start.y > line.end.y)
        {
            start = line.end;
            end = line.start;
        }

        if (horizontal)
        {
            for (var x = start.x; x <= end.x; x += 1)
            {
                blocks.Add((x, start.y));
            }
        }
        else
        {
            for (var y = start.y; y <= end.y; y += 1)
            {
                blocks.Add((start.x, y));
            }
        }
    }

    var dirs = new List<(int x, int y)>()
    {
        (0,1),(-1,1),(1,1)
    };

    bool done = false;
    bool moved = true;
    int sandUnits = 0;

    while (!done)
    {
        var sand = (x: 500, y: 0);
        while (moved)
        {
            moved = false;
            foreach (var d in dirs)
            {
                var check = (x: sand.x + d.x, y: sand.y + d.y);

                if(!blocks.Contains(check) && !sandBlocks.Contains(check) && check.y != h + 1)
                {
                    moved = true;
                    sand = check;
                    break;
                }
            }
            if(!moved && sand == (500, 0))
            {
                sandBlocks.Add(sand);
                sandUnits++;
                done = true;
            }
        }
        if (!done)
        {
            moved = true;
            sandBlocks.Add(sand);
            sandUnits++;
        }
        printgrid2();
        Console.ReadKey();
    }
    Console.WriteLine(sandUnits);

}

void part1()
{

    foreach (var line in lines)
    {
        bool horizontal = false;
        (int x, int y) start = line.start;
        (int x, int y) end = line.end;

        if (line.start.x != line.end.x)
        {
            horizontal = true;
            if (line.start.x > line.end.x)
            {
                start = line.end;
                end = line.start;
            }
        }
        else if (line.start.y > line.end.y)
        {
            start = line.end;
            end = line.start;
        }

        if (horizontal)
        {
            for (var x = start.x - topLeft.x; x <= end.x - topLeft.x; x += 1)
            {
                grid[start.y, x] = '#';
            }
        }
        else
        {
            for (var y = start.y; y <= end.y; y += 1)
            {
                grid[y, start.x - topLeft.x] = '#';
            }
        }
    }

    var dirs = new List<(int x, int y)>()
    {
        (0,1),(-1,1),(1,1)
    };

    bool done = false;
    bool moved = true;
    int sandUnits = 0;

    while (!done)
    {
        var sand = (x: 500 - topLeft.x, y: 0);
        while (moved)
        {
            moved = false;
            foreach (var d in dirs)
            {
                var check = (x: sand.x + d.x, y: sand.y + d.y);
                if (check.y >= h || check.x < 0 || check.x >= w)
                {
                    done = true;
                    moved = false;
                    break;
                }
                if (grid[check.y, check.x] == 0)
                {
                    grid[check.y, check.x] = 'o';
                    grid[sand.y, sand.x] = (char)0;

                    moved = true;
                    sand = check;

                    //printgrid();
                    // Console.ReadKey();

                    break;
                }
            }

        }
        if (!done)
        {
            grid[sand.y, sand.x] = 'o';
            moved = true;
            sandUnits++;
        }
    }

    Console.WriteLine(sandUnits);

}
void printgrid()
{
    for(var r = 0; r < h; r++)
    {
        for(var c = 0; c < w; c++)
        {
            Console.Write(grid[r, c] == 0 ? '.' : grid[r, c]);
        }
        Console.WriteLine();
    }
}

void printgrid2()
{
    var tl = (x: Math.Min(blocks.MinBy(b => b.x).x, sandBlocks.MinBy(b => b.x).x), y: 0);
    var br = (x: Math.Max(blocks.MaxBy(b => b.x).x, sandBlocks.MaxBy(b => b.x).x), h + 1);

    var w = br.x - tl.x;


    for (var r = 0; r < h+1; r++)
    {
        for (var c = 0; c < w+1; c++)
        {
            Console.Write(blocks.Contains((c + tl.x, r)) ? '#' : sandBlocks.Contains((c + tl.x, r)) ? 'o' : '.');
        }
        Console.WriteLine();
    }
    for(var c = 0; c < w+1; c++)
    {
        Console.Write('#');
    }
    Console.WriteLine();
}


