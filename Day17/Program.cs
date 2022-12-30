var windDirections = File.ReadAllText("input.txt");
int boardWidth = 7;
char[] pieceTypes = {
 '-','+','L','|','s'
};

HashSet<(int x, long y)> board;
long pieceIndex = 0;
long height = 0;
long pieceCount = 0;

Console.WriteLine($"Part 1: {Simulate(2022)}");
Console.WriteLine($"Part 2: {Simulate(1000000000000)}");

long Simulate(long numToDrop)
{
    //create board
    board = new();
    pieceIndex = 0;
    height = 0;
    pieceCount = 0;
    Dictionary<long, (long pieceIndex, long pieceCount, long height, HashSet<(int x, long y)> board)> boards = new();

    long absoluteTime = 0;
    long relativeTime = 0;

    for (var i = 0L; i < numToDrop; i++) //1000000000000
    {
        boards.Add(absoluteTime, (pieceIndex, pieceCount, height, new(board)));

        //i'm sure there's a better way to memoize this, but idea is to check the board status at each relative time that
        //matches this absolute time in the memo. If we are dropping the same piece and the board looks the same
        //we've found our repeat, so just calculate the total height.
        var xyz = absoluteTime / windDirections.Length;
        if (xyz > 0)
        {
            for (var a = xyz - 1; a >= 0; a--)
            {
                var ind = relativeTime + a * windDirections.Length;
                if (boards.ContainsKey(ind) && boards[ind].pieceIndex == pieceIndex)
                {
                    //check if the top of each board state is the same (normalize heights), this is probably wrong in general, maybe can memo
                    //the board diffs and normalize heights?
                    var b1 = boards[ind].board.Where(p => p.y >= boards[ind].height - 10).Select(p => (p.x, p.y - boards[ind].height));
                    var b2 = boards[absoluteTime].board.Where(p => p.y >= boards[absoluteTime].height - 10).Select(p => (p.x, p.y - boards[absoluteTime].height));

                    if (b1.SequenceEqual(b2))
                    {
                        //if we are here, we've found our repeat
                        var initialPieceCount = boards[ind].pieceCount;
                        var initialHeight = boards[ind].height;
                        var piecesPerLoop = boards[absoluteTime].pieceCount - initialPieceCount;
                        var heightPerLoop = boards[absoluteTime].height - initialHeight;
                        var numberOfLoops = (numToDrop - initialPieceCount) / piecesPerLoop;
                        var leftover = (numToDrop - initialPieceCount) % piecesPerLoop;

                        //last piece is the height of the leftovers
                        //   height of the board where the piece count = leftover piece count + initial piece count
                        // - height of intial
                        var h = initialHeight 
                            + (numberOfLoops * heightPerLoop)
                            + boards.Where(x => x.Value.pieceCount == boards[ind].pieceCount + leftover).First().Value.height - boards[ind].height;

                        return h;
                    }
                }
            }
        }

        var piece = createPiece();

        //move new piece until it hits ground
        while (true)
        {
            HashSet<(int x, long y)> p2;
            var wind = windDirections[(int)relativeTime];
            if (wind == '<')
            {
                p2 = MoveX(piece, -1);
            }
            else
            {
                p2 = MoveX(piece, 1);
            }

            if (!p2.Intersect(board).Any())
            {
                piece = p2;
            }

            absoluteTime++;
            relativeTime = absoluteTime % windDirections.Length;

            p2 = MoveY(piece);
            if (p2.Intersect(board).Any() || p2.Any(p => p.y == 0))
            {
                board.UnionWith(piece);
                height = Math.Max(height, piece.MaxBy(n => n.y).y);
                break;
            }
            else
            {
                piece = p2;
            }
        }
    }
    return height;
}

HashSet<(int x, long y)> createPiece()
{
    char stype = pieceTypes[pieceIndex];

    pieceIndex = (pieceIndex + 1) % pieceTypes.Length;
    pieceCount++;

    HashSet<(int x, long y)> shape = new();
    int x = 3;
    long y = height + 4;

    switch (stype)
    {
        case '-':
            {
                shape.Add((x, y));
                shape.Add((x + 1, y));
                shape.Add((x + 2, y));
                shape.Add((x + 3, y));
                break;
            }
        case '+':
            {
                shape.Add((x + 1, y));
                shape.Add((x, y + 1));
                shape.Add((x + 1, y + 1));
                shape.Add((x + 2, y + 1));
                shape.Add((x + 1, y + 2));
                break;
            }
        case 'L':
            {
                shape.Add((x, y));
                shape.Add((x + 1, y));
                shape.Add((x + 2, y));
                shape.Add((x + 2, y + 1));
                shape.Add((x + 2, y + 2));
                break;
            }
        case '|':
            {
                shape.Add((x, y));
                shape.Add((x, y + 1));
                shape.Add((x, y + 2));
                shape.Add((x, y + 3));
                break;
            }
        case 's':
            {
                shape.Add((x, y));
                shape.Add((x + 1, y));
                shape.Add((x, y + 1));
                shape.Add((x + 1, y + 1));
                break;
            }
        default:
            break;
    }

    return shape;
}

HashSet<(int x, long y)> MoveX(HashSet<(int x, long y)> piece, int dir)
{
    HashSet<(int x, long y)> newPiece = new();
    foreach (var (x, y) in piece)
    {
        var newX = x + dir;
        if (newX > boardWidth || newX < 1)
        {
            return piece;
        }
        newPiece.Add((newX, y));
    }
    return newPiece;
}

HashSet<(int x, long y)> MoveY(HashSet<(int x, long y)> piece)
{
    HashSet<(int x, long y)> newPiece = new();
    foreach (var (x, y) in piece)
    {
        var newY = y - 1;
        newPiece.Add((x, newY));
    }
    return newPiece;
}