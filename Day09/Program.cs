using System.Globalization;
using System.Net;

var input = File.ReadAllLines("input.txt").ToArray();

var rope1 = new (int x, int y)[]
{
    (0, 0),
    (0, 0)
};

var rope2 = new (int x, int y)[]
{
    (0, 0),
    (0, 0),
    (0, 0),
    (0, 0),
    (0, 0),
    (0, 0),
    (0, 0),
    (0, 0),
    (0, 0),
    (0, 0)
};

Console.WriteLine($"Part 1: {simulate(rope1)}");
Console.WriteLine($"Part 2: {simulate(rope2)}");

int simulate((int x, int y)[] rope)
{

    HashSet<(int, int)> tailVisited = new HashSet<(int, int)>
    {
        (0,0)
    };

    foreach (var line in input)
    {
        var inst = line.Split(' ');
        var d = int.Parse(inst[1]);
        switch (inst[0])
        {
            case "U":
                for (var i = 0; i < d; i++)
                {
                    rope[0].y += 1;
                    for (var j = 1; j < rope.Length; j++)
                    {
                        move(j, rope);
                    }
                    tailVisited.Add(rope.Last());
                    print();
                }
                break;

            case "D":
                for (var i = 0; i < d; i++)
                {
                    rope[0].y -= 1;
                    for (var j = 1; j < rope.Length; j++)
                    {
                        move(j, rope);
                    }
                    tailVisited.Add(rope.Last());
                    print();
                }
                break;

            case "L":
                for (var i = 0; i < d; i++)
                {
                    rope[0].x -= 1;
                    for (var j = 1; j < rope.Length; j++)
                    {
                        move(j, rope);
                    }
                    tailVisited.Add(rope.Last());
                    print();
                }
                break;

            case "R":
                for (var i = 0; i < d; i++)
                {
                    rope[0].x += 1;
                    for (var j = 1; j < rope.Length; j++)
                    {
                        move(j, rope);
                    }
                    tailVisited.Add(rope.Last());
                    print();
                }
                break;
            default:
                break;
        }
    }

    return tailVisited.Count();
}
void move(int j, (int x, int y)[] rope)
{
    var dy = rope[j - 1].y - rope[j].y;
    var dx = rope[j - 1].x - rope[j].x;

    if (dy > 1)
    {
        rope[j].y += 1;
    }
    else if (dy < -1)
    {
        rope[j].y -= 1;
    }

    if (Math.Abs(dy) > 1 && Math.Abs(dx) > 0)
    {
        rope[j].x += dx/Math.Abs(dx);
    }

    if (Math.Abs(dy) > 1)
        return;

    if (dx > 1)
        rope[j].x += 1;
    else if(dx < -1)
        rope[j].x -= 1;


    if (Math.Abs(dx) > 1 && Math.Abs(dy) > 0)
        rope[j].y += dy/Math.Abs(dy);

}


void print()
{
    if (false)
    {
        //int ox = 10, oy = 10;

        //var w = 20;// rope.Max(k => k.x) - rope.Min(k => k.x);
        //var h = 20; // rope.Max(k => k.y) - rope.Min(k => k.y);
        //var grid = new char[h, w];
        //for (var i = 0; i < h; i++)
        //{
        //    for (var j = 0; j < w; j++)
        //    {
        //        grid[i, j] = '.';
        //    }
        //}
        //for (var k = rope.Length - 1; k >= 0; k--)
        //{
        //    grid[rope[k].y + oy, rope[k].x + ox] = k == 0 ? 'H' : k == rope.Length - 1 ? 'T' : k.ToString()[0];
        //}
        //for (var i = h - 1; i >= 0; i--)
        //{
        //    for (var j = 0; j < w; j++)
        //    {
        //        Console.Write(grid[i, j]);
        //    }
        //    Console.WriteLine();
        //}
        //Console.WriteLine();
        Console.ReadKey();
    }
}