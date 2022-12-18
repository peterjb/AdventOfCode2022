var lines = File.ReadAllLines("input.txt");

HashSet<(int x, int y, int z)> cubes = new();
foreach (var l in lines)
{
    var ls = l.Split(',').Select(int.Parse).ToArray();
    cubes.Add((ls[0], ls[1], ls[2]));

}
var sides = new List<(int x, int y, int z)>()
    {
        (1,0,0),
        (-1,0,0),
        (0,1,0),
        (0,-1,0),
        (0,0,1),
        (0,0,-1)
    };

int open = 0;
foreach (var c in cubes)
{
    foreach (var s in sides)
    {
        if (!cubes.Contains((c.x + s.x, c.y + s.y, c.z + s.z)))
        {
            open++;
        }
    }

}
Console.WriteLine($"Part 1: {open}");

Dictionary<(int x, int y), (int zMin, int zMax)> xys = new();
Dictionary<(int x, int z), (int yMin, int yMax)> xzs = new();
Dictionary<(int y, int z), (int xMin, int xMax)> yzs = new();

foreach (var c in cubes)
{
    var xy = (c.x, c.y);
    var z = c.z;

    if (xys.ContainsKey(xy))
    {
        var i = xys[xy];
        xys[xy] = (Math.Min(i.zMin, z), Math.Max(i.zMax, z));
    }
    else
    {
        xys[xy] = (z, z);
    }

    var xz = (c.x, c.z);
    var y = c.y;

    if (xzs.ContainsKey(xz))
    {
        var i = xzs[xz];
        xzs[xz] = (Math.Min(i.yMin, y), Math.Max(i.yMax, y));
    }
    else
    {
        xzs[xz] = (y, y);
    }

    var yz = (c.y, c.z);
    var x = c.x;

    if (yzs.ContainsKey(yz))
    {
        var i = yzs[yz];
        yzs[yz] = (Math.Min(i.xMin, x), Math.Max(i.xMax, x));
    }
    else
    {
        yzs[yz] = (x, x);
    }

}

HashSet<(int x, int y, int z)> outside = new();
int open2 = 0;
foreach (var c in cubes)
{
    foreach (var s in sides)
    {
        var check = (c.x + s.x, c.y + s.y, c.z + s.z);
        if (!cubes.Contains(check) && isOutside(check, new HashSet<(int x, int y, int z)>()))
        {
            open2++;
        }
    }
}

Console.WriteLine($"Part 2: {open2}");

bool isOutside((int x, int y, int z) point, HashSet<(int x, int y, int z)> visited)
{
    if (outside.Contains(point))
        return true;

    var xy = (point.x, point.y);
    var xz = (point.x, point.z);
    var yz = (point.y, point.z);

    if (!xys.ContainsKey(xy) || !xzs.ContainsKey(xz) || !yzs.ContainsKey(yz))
    {
        outside.Add(point);
        return true;
    }

    var xyInterval = xys[xy];
    var xzInterval = xzs[xz];
    var yzInterval = yzs[yz];

    if (point.z >= xyInterval.zMax || point.z <= xyInterval.zMin || point.y >= xzInterval.yMax || point.y <= xzInterval.yMin || point.x >= yzInterval.xMax || point.x <= yzInterval.xMin)
    {
        outside.Add(point);
        return true;
    }
    else
    {
        visited.Add(point);
        foreach (var s in sides)
        {
            var next = (point.x + s.x, point.y + s.y, point.z + s.z);
            if (!cubes.Contains(next) && !visited.Contains(next) && isOutside(next, visited))
            {
                outside.Add(point);
                return true;
            }
        }
        return false;
    }
}


