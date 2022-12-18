var windDirections = File.ReadAllText("input.txt");

var shapes = new List<char[,]> {
new[,]
{
    { '#','#','#','#'}
},
new[,]
{
    { '.','#','.'},
    { '#','#','#'},
    { '.','#','.'}
},
    new[,]
{
    { '.','.','#'},
    { '.','.','#'},
    { '#','#','#'}
},
    new[,]
{
    { '#' },
    { '#' },
    { '#' },
    { '#' }
}, new[,]
{
    { '#','#' },
    { '#','#' }
}
};

long floor = 0;

int pieceIndex = -1;
char[] pieceTypes = {
 '-','+','l','|','s'
};

HashSet<(char, char, int, long)> imemo = new();
int memohitcount = 0;

SortedDictionary<long, List<piece>> spatialPartition = new();

var t = 0;

    for (var rocks = 0L; rocks < 1000000000000; rocks++) //1000000000000
{
    if(rocks % 100 == 0)
    {
        var ks = spatialPartition.Keys.Take(spatialPartition.Count()/2).ToList();
        foreach(var k in ks)
        {
            spatialPartition.Remove(k);
        }
    }

    var p = nextPiece();

    while (true)
    {
        //draw();
        //Console.ReadKey();

        var wind = windDirections[t];

        switch (wind)
        {
            case '<':
                tryMoveLeftRight(p, -1);
                break;
            case '>':
                tryMoveLeftRight(p, 1);
                break;
            default:
                break;
        }
        //draw();
        //Console.ReadKey();
        t++;
        if (t == windDirections.Length)
        {
            t = 0;
        }

        if (!canMoveDown(p))
        {
            for(var h = p.bottom; h <= p.top; h++)
            {
                if (!spatialPartition.ContainsKey(h))
                {
                    spatialPartition.Add(h, new List<piece>());
                }
                spatialPartition[h].Add(p);
            }
            floor = Math.Max(floor, p.top);
            break;
        }
    }
}
var x = 100;

bool tryMoveLeftRight(piece p, int x)
{
    if (x == 1 && (p.right == 7))
    {
        return false;
    }
    else if (x == -1 && (p.left == 1))
    {
        return false;
    }
    else
    {
        p.left += x;
        for (var h = p.bottom; h <= floor; h++)
        {
            if (spatialPartition[h].Any(op => intersect(op, p)))
            {
                p.left -= x;
                return false;
            }
        }
    }
    return true;
}

bool canMoveDown(piece p)
{
    if (p.bottom == 1)
        return false;

    p.bottom -= 1;

    for (var h = p.bottom; h <= floor; h++)
    {
        if (spatialPartition[h].Any(op => intersect(op, p)))
        {
            p.bottom += 1;
            return false;
        }
    }

    return true;
}

bool intersect(piece one, piece two)
{
    if (imemo.Contains((one.type, two.type, one.left - two.left, one.bottom - two.bottom)))
    {
        return true;
    }

    if(one.right < two.left || one.left > two.right || one.bottom > two.top || one.top < two.bottom) return false;

    var startC = Math.Max(one.left, two.left);
    var endC = Math.Min(one.right, two.right);

    var startR = Math.Max(one.bottom, two.bottom); 
    var endR = Math.Min(one.top, two.top);

    for (var r = startR; r <= endR; r++)
    {
        for (var c = startC; c <= endC; c++)
        {
            if (one.shape[one.top - r, c - one.left] == '#' && two.shape[two.top - r, c - two.left] == '#')
            {
                imemo.Add((one.type, two.type, one.left - two.left, one.bottom - two.bottom));
                return true;
            }
        }
    }

    return false;
}

piece nextPiece()
{
    pieceIndex++;
    if (pieceIndex == shapes.Count)
        pieceIndex = 0;


    return new piece()
    {
        shape = shapes[pieceIndex],
        type = pieceTypes[pieceIndex],
        bottom = floor + 4,
        left = 3
    };
}


//void draw()
//{
//    var h = 25;

//    char[,] grid = new char[h, 9];

//    for (var r = 0; r < h; r++)
//    {
//        for (var c = 0; c < 9; c++)
//        {
//            if (r == h-1 && (c == 0 || c == 8))
//                grid[r, c] = '+';
//            else if (r == h-1)
//                grid[r, c] = '-';
//            else if (c == 0 || c == 8)
//                grid[r, c] = '|';
//            else
//                grid[r, c] = '.';
//        }
//    }


//    foreach (var p in pieces)
//    {
//        for(var r = 0; r < p.dims.h; r++)
//        {
//            for(var c = 0; c < p.dims.w; c++)
//            {
//                if (p.shape[r,c] == '#')
//                {
//                    grid[h - p.bottom - p.dims.h + r, p.left + c] = '#';
//                }
//            }
//        }
//    }

//    for (var r = 0; r < h; r++)
//    {
//        for (var c = 0; c < 9; c++)
//        {
//            Console.Write(grid[r, c]);
//        }
//        Console.WriteLine();
//    }
//}

class piece
{
    public char[,] shape;
    public char type;
    public int left;
    public long bottom;
    public int right => left + dims.w - 1;
    public long top => bottom + dims.h - 1;
    public (int w,int h) dims { get { return extents[type]; } }

    private static Dictionary<char, (int w, int h)> extents = new()
    {
        { '-', (4,1) },
        { '+', (3,3) },
        { 'l', (3,3) },
        { '|', (1,4) },
        { 's', (2,2) }
    };
}
