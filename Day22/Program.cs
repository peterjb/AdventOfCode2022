using System.Text.RegularExpressions;

var lines = File.ReadAllText("test.txt").Split("\r\n\r\n");

var map = lines[0].Split("\r\n").ToArray();

var instructions = Regex.Matches(lines[1], @"((\d+)|(R|L))").Select(m => m.Value).ToArray();

var dirs = new List<(int c, int r)>()
{
    (1,0),
    (0,1),
    (-1,0),
    (0,-1)
};

var s = 4;

var sides = new Dictionary<string, Side>()
{
    {
        "Top", new Side() { name = "Top", r1 = 0, r2 = s-1, c1 = s, c2 = 2*s-1  }
    },
    {
        "Right", new Side() { name = "Right", r1 = 0, r2 = s-1, c1 = 2*s, c2 = 3*s-1 }
    },
    {
        "Near", new Side() { name = "Near", r1 = s, r2 = 2*s-1, c1 = s, c2 = 2*s-1 }
    },
    {
        "Bottom", new Side() { name = "Bottom", r1 = 2*s, r2 = 3*s-1, c1 = s, c2 = 2*s-1 }
    },
    {
        "Left", new Side() { name = "Left",     r1 = 2*s, r2 = 3*s-1, c1 = 0, c2 = s-1 }
    },
    {
        "Far", new Side() { name = "Far",       r1 = 3*s, r2 = 4*s-1, c1 = 0, c2 = s-1 }
    },
};

var sideRef = new Dictionary<string, List<string>>()
{
    {"Top", new() {"Right", "Near", "Left", "Far" } },
    {"Right", new() {"Bottom", "Near", "Top", "Far" } },
    {"Near", new() {"Right", "Bottom", "Left", "Top" } },
    {"Bottom", new() {"Right", "Far", "Left", "Near" } },
    {"Left", new() {"Bottom", "Far", "Top", "Near" } },
    {"Far", new() {"Bottom", "Right", "Top", "Left" } },
};


Dictionary<string, List<string>> naturalFaces = new()
{
    {"Near", new() { "Right", "Bottom", "Left", "Top" } },
    {"Top", new() { "Right", "Near", "Left", "Far" } },
    {"Right", new() { "Far", "Bottom", "Near", "Top" } },
    {"Bottom", new() { "Right", "Far", "Left", "Near" } },
    {"Left", new() { "Right", "Bottom", "Left", "Top" } },
    {"Far", new() { "Right", "Bottom", "Left", "Top" } },
};


var faceGrid = new Face[4, 4];

for (var i = 0; i < map.Length; i += s)
{
    var fr = i / s;
    var start = map[i].ToList().FindIndex(c => c != ' ');
    for (var j = start; j < map[i].Length; j += s)
    {
        var fc = j / s;
        faceGrid[fr, fc] = new Face() { grid = map.Skip(i).Take(s).Select(str => str.Substring(j, s)).ToArray() };
    }
}


HashSet<(int r, int c)> visited = new();

Face[] FindOrder() {
    for (var r = 0; r < 4; r++)
    {
        for (var c = 0; c < 4; c++)
        {
            if (faceGrid[r,c] == null) continue;    

            visited = new();
            var order = GetOrder((r, c), new());
            if (order != null)
                return order;
        }
    }
    return null;
}



string[]? faceNames = new string[] { "front", "top", "back", "bottom", "left", "right" };
int orientations = 4;
var moves = new (int y, int x)[] { (0, 1), (1, 0), (0, -1), (-1, 0) };
Face[] GetOrder((int r, int c) pos, HashSet<string> faces)
{
    throw new NotImplementedException();
    //if(faces.Count() == 6)
    //    return Array.Empty<Face>();

    //foreach(var m in moves)
    //{
    //    var next = (r: pos.r + m.y, c: pos.c + m.x);
    //    if(next.r < 4 && next.r >= 0 && next.c < 4 && next.c >= 0)
    //    {
    //        var result = GetOrder(next, faces);
    //    }
    //}
}



//r not in set
var rbounds = new Dictionary<int, (int left, int right)>();
var cbounds = new Dictionary<int, (int top, int bottom)>();

for (var row = 0; row < map.Length; row++)
{
    var left = map[row].ToList().FindIndex(c => c != ' ');
    var right = map[row].Length;
    rbounds[row] = (left, right);

    for (var col = left; col < right; col++)
    {
        if (cbounds.ContainsKey(col))
        {
            cbounds[col] = (Math.Min(cbounds[col].top, row), Math.Max(cbounds[col].bottom, row + 1));
        }
        else
        {
            cbounds[col] = (row, row + 1);
        }
    }
}

var dir = 0; //right
var r = 0;
var c = rbounds[0].left;

for (var ii = 0; ii < instructions.Length; ii += 2)
{
    var dist = int.Parse(instructions[ii]);
    var delta = dirs[dir];

    for (var mi = 0; mi < dist; mi++)
    {
        var newr = r + delta.r;
        var newc = c + delta.c;

        if (newc >= rbounds[r].right)
            newc = rbounds[r].left;
        else if (newc < rbounds[r].left)
            newc = rbounds[r].right - 1;

        if (newr >= cbounds[c].bottom)
            newr = cbounds[c].top;
        else if (newr < cbounds[c].top)
            newr = cbounds[c].bottom - 1;

        if (map[newr][newc] == '#')
            break;
        else
        {
            r = newr;
            c = newc;
        }
    }

    //rotate
    if (ii + 1 < instructions.Length)
    {
        dir = rotate(instructions[ii + 1]);
    }
}

Console.WriteLine($"Part 1: {1000 * (r + 1) + 4 * (c + 1) + dir}");




dir = 0; //right
r = 0;
c = rbounds[0].left;
var side = sides["Top"];

for (var ii = 0; ii < instructions.Length; ii += 2)
{
    var dist = int.Parse(instructions[ii]);

    for (var mi = 0; mi < dist; mi++)
    {
        var (newr, newc, newdir) = moveWithWrap(r, c, dir);

        if (map[newr][newc] == '#')
            break;
        else
        {
            r = newr;
            c = newc;
            dir = newdir;
        }
    }

    //rotate
    if (ii + 1 < instructions.Length)
    {
        dir = rotate(instructions[ii + 1]);
    }
}

Console.WriteLine($"Part 2: {1000 * (r + 1) + 4 * (c + 1) + dir}");

int rotate(string d)
{
    switch (d)
    {
        case "R":
            return (dir + 1) % 4;
        case "L":
            return (dir + 3) % 4;
        default:
            throw new Exception();
    }
}

(int r, int c, int d, Side s) moveWithWarp2(int r, int c, int dir, Side s)
{
    bool newSide = false;
    Side nextSide = sides[sideRef[s.name][dir]];

    if ((r == s.r1 && dir == 3 && nextSide.r2 != s.r1 - 1 && nextSide.c1 != s.c1) ||
        (r == s.r2 && dir == 1 && nextSide.r1 != s.r2 + 1 && nextSide.c1 != s.c1) ||
        (c == s.c1 && dir == 2 && nextSide.c2 != s.c1 - 1 && nextSide.r1 != s.r1) ||
        (c == s.c2 && dir == 0 && nextSide.c1 != s.c2 + 1 && nextSide.r1 != s.r1)
    )
    {
        newSide = true;

        int sr = sideRef[nextSide.name].FindIndex(str => str == s.name);

        //calc new pos
    }

    var delta = dirs[dir];

    return (r + delta.r, c + delta.c, dir, newSide ? nextSide : s);
}

(int r, int c, int d) moveWithWrap(int r, int c, int dir)
{
    //top to far
    if (r == 0 && dir == 3)
    {
        return (s, 3 * s - c - 1, 1);
    }

    //far to top
    if (r == s + 1 && c < 2 && dir == 3)
    {
        return (0, 3 * s + c - 1, 1);
    }

    //bottom to far
    if (r == 3 * s - 1 && dir == 1)
    {
        return (2 * s - 1, 3 * s - c - 1, 3);

    }

    //far to bottom
    if (r == 2 * s - 1 && c < 2 && dir == 1)
    {
        return (3 * s - 1, 3 * s + c - 1, 3);

    }

    //top to left
    if (c == 2 * s && r < s && dir == 2)
    {
        return (2, r + s, 1);
    }

    //left to top
    if (r == s && c >= s && c < 2 * s && dir == 3)
    {
        return (c - s, 2 * s, 0);
    }

    //bottom to left
    if (c == 2 * s && r >= 2 * s && dir == 2)
    {
        return (2 * s - 1, -r + 4 * s - 1, 3);
    }

    //left to bottom
    if (r == (2 * s - 1) && c >= s && c < 2 * s && dir == 1)
    {
        return (-c + 4 * s - 1, 2 * s, 0);
    }

    //near to right
    if (c == 3 * s - 1 && r >= s && r < 2 * s && dir == 0)
    {
        return (2 * s, -r + 5 * s - 1, 1);
    }

    //right to near
    if (r == 2 * s && c >= 3 * s && dir == 3)
    {
        return (-c + 5 * s - 1, 3 * s - 1, 2);
    }

    //top to right
    if (c == 3 * s - 1 && r < s && dir == 0)
    {
        return (-r + 3 * s - 1, 4 * s - 1, 2);
    }

    //right to top
    if (c == 4 * s - 1 && r >= 2 * s && dir == 0)
    {
        return (-r + 3 * s - 1, 3 * s - 1, 2);
    }

    //far to right
    if (c == 0 && r >= s && r < 2 * s && dir == 2)
    {
        return (3 * s - 1, -r + 5 * s - 1, 3);
    }

    if (r == 3 * s - 1 && c >= 3 * s && dir == 1)
    {
        return (-r + 5 * s - 1, 0, 0);
    }

    var delta = dirs[dir];

    return (r + delta.r, c + delta.c, dir);
}


struct Side
{
    public string name;
    public int r1;
    public int r2;
    public int c1;
    public int c2;
}

class Face
{
    public string name;
    public string[] grid;
    public string? top;
    public string? right;
    public string? bottom;
    public string? left;
}