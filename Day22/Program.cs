using System.Text.RegularExpressions;


/* Calling it good here. because i did part 2 a stupid way, i just have a 
 * transformed r, c face and direction instead of the actual answer
 * so there's a little extra work to be done to calculate the password.
 */
var lines = File.ReadAllText("input.txt").Split("\r\n\r\n");

var map = lines[0].Split("\r\n").ToArray();

var instructions = Regex.Matches(lines[1], @"((\d+)|(R|L))").Select(m => m.Value).ToArray();
var cdirs = new char[] { '>', 'v', '<', '^' };
var dirs = new List<(int c, int r)>()
{
    (1,0),
    (0,1),
    (-1,0),
    (0,-1)
};

(string? name, string orientation)[,] inputFaceMap = new[,]
{
    { (null,null), ("top",null), ("right", "rotr"), (null,null) },
    { (null,null), ("front",null), (null,null), (null,null) },
    { ("left","rotr"), ("bottom","flipb"), (null,null), (null,null) },
    { ("back","rotr"), (null,null), (null, null), (null, null) }
};

(string? name, string? orientation)[,] testFaceMap = new[,]
{
    { (null,null), (null,null), ("top", null), (null,null) },
    { ("back",null), ("left",null), ("front",null), (null,null) },
    { (null,null), (null,null), ("bottom","flipb"), ("right","rotl") },
    { (null,null), (null,null), (null, null), (null, null) }
};


var s = 50;
(string? name, string? orientation)[,] faceMap = inputFaceMap;// testFaceMap;

var faces = new Dictionary<string, Face>();
for (var i = 0; i < map.Length; i += s)
{
    var fr = i / s;
    var start = map[i].ToList().FindIndex(c => c != ' ');
    if (start == -1)
        continue;

    for (var j = start; j < map[i].Length; j += s)
    {
        var fc = j / s;
        var o = faceMap[fr, fc];
        var grid = map.Skip(i).Take(s).Select(str => str.Substring(j, s)).ToArray();
        var v = new Face() { name = o.name, grid = new char[s, s] };

        for (var rr = 0; rr < s; rr++)
        {
            for (var cc = 0; cc < s; cc++)
            {
                switch (o.orientation)
                {
                    case "fliph":
                        v.grid[rr, cc] = grid[rr][s - cc - 1];
                        break;
                    case "flipv":
                        v.grid[rr, cc] = grid[s - rr - 1][cc];
                        break;
                    case "flipb":
                        v.grid[rr, cc] = grid[s - rr - 1][s - cc - 1];
                        break;
                    case "rotr":
                        v.grid[cc, s - rr - 1] = grid[rr][cc];
                        break;
                    case "rotl":
                        v.grid[s-cc-1, rr] = grid[rr][cc];
                        break;
                    default:
                        v.grid[rr, cc] = grid[rr][cc];
                        break;
                }
            }
        }
        faces[o.name] = v;
    }
}


Dictionary<string, List<(string n, Func<int, int, (int, int)> t, int d)>> naturalFaces = new()
{
    {
        "front", new() {
            (n: "right" , t: (int r, int c) => (r, 0), 0),
            (n: "bottom", t: (int r, int c) => (s - 1, s - c - 1), 3),
            (n: "left"  , t: (int r, int c) => (r, s - 1), 2),
            (n: "top"   , t: (int r, int c) => (s - 1, c), 3)
        }
    },
    {
        "top", new() {
            ("right", (int r, int c) => (0, s - r - 1), 1),
            ("front", (int r, int c) => (0, c), 1),
            ("left" , (int r, int c) => (0, r), 1),
            ("back" , (int r, int c) =>(0, s - c - 1) , 1)
        }
    },
    {
        "right" , new() {
            ("back"   , (int r, int c) => (r,0), 0),
            ("bottom" , (int r, int c) => (s-c-1, 0), 0),
            ("front"  , (int r, int c) => (r, s - 1), 2),
            ("top"    , (int r, int c) => (s -c - 1, s - 1), 2)
        }
    },
    {
    "bottom", new() {
        ("left", (int r, int c) => (s-1,r), 3),
        ("front" , (int r, int c) => (s-1,s-c-1), 3),
        ("right" , (int r, int c) => (s-1,s-r-1), 3),
        ("back" , (int r, int c) => (s-1, c), 3)
    }
    },
    {
    "left", new() {
        ("front", (int r, int c) => (r,0), 0),
        ("bottom", (int r, int c) => (c,s-1), 2),
        ("back" , (int r, int c) => (r,s-1), 2),
        ("top"   , (int r, int c) => (c,0), 0)
    }
    },
    {
    "back", new() {
        ("left", (int r, int c) => (r,0), 0),
        ("bottom", (int r, int c) => (0,c), 1),
        ("right" , (int r, int c) => (r,s-1), 2),
        ("top"   , (int r, int c) => (0, s - c - 1), 1)
    }
    },
};

//setup part 1
//r not in set
var rbounds = new Dictionary<int, (int left, int right)>();
var cbounds = new Dictionary<int, (int top, int bottom)>();

Dictionary<string, Dictionary<string, bool>> seen = new();
foreach(var x in faces)
{
    seen.Add(x.Key, new());
    foreach(var y in faces)
    {
        seen[x.Key].Add(y.Key, x.Key == y.Key);
    }
}

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



var doprint = false;
dir = 0; //right
r = 0;
c = 0;
var face = faces["top"];
int printcount = 0;
int totalprintcount = 0;
for (var ii = 0; ii < instructions.Length; ii += 2)
{
    var dist = int.Parse(instructions[ii]);
    //if(true)
    //{
    //    totalprintcount++;
    //    print();
    //    Console.ReadKey(); printcount++;
    //    //if (printcount > 1)
    //    //{
    //    //    doprint = false;
    //    //    printcount = 0;
    //    //}
    //}
    for (var mi = 0; mi < dist; mi++)
    {
        var (f, (newr, newc), newdir) = moveWithWrap(face, r, c, dir);
        
        if (f.grid[newr, newc] == '#')
            break;
        else
        {
            //if (!seen[face.name][f.name] || doprint)
            //{
            //    seen[face.name][f.name] = true;
            //    totalprintcount++;
            //    doprint = true;
                //print();
                //printcount++;
                //Console.ReadKey();
            //    if (printcount > 1)
            //    {
            //        doprint = false;
            //        printcount = 0;
            //    }
            //}
            face = f;
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

Console.WriteLine($"Part 2: {face.name}: ({r + 1},{c + 1}) Direction: {dir}");

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

void print()
{
    foreach (var f in faces)
    {
        if (f.Value.name != face.name)
            continue;
        Console.WriteLine($"{totalprintcount}: {f.Key}: ({r},{c}): {dir}");
        for (var row = 0; row < s; row++)
        {
            for (var col = 0; col < s; col++)
            {
                if (f.Key == face.name && r == row && c == col)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.Write(cdirs[dir]);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                else 
                {
                    Console.Write(f.Value.grid[row, col]);
                }
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }
}

(Face f, (int r, int c), int d) moveWithWrap(Face f, int r, int c, int d)
{
    var nf = naturalFaces[f.name][d];
    //up, down, left, right
    if ((r == 0 && d == 3) || (r == s - 1 && d == 1) || (c == 0 && d == 2) || (c == s - 1 && d == 0))
    {
        return (faces[nf.n], nf.t(r, c), nf.d);
    }

    var delta = dirs[dir];

    return (f, (r + delta.r, c + delta.c), d);
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
    public char[,] grid;
    public string? top;
    public string? right;
    public string? bottom;
    public string? left;
}