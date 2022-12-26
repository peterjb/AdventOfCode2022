var input = File.ReadAllLines("input.txt");

var allDirs = new (int r, int c)[]
{
    //N    NE      E     SE    S     SW     W      NW
    //0    1       2     3     4      5     6      7
    (-1,0),(-1,1),(0,1),(1,1),(1,0),(1,-1),(0,-1),(-1,-1)
};

Dictionary<int, (int r, int c)> elves = new();
HashSet<(int r, int c)> occupiedPositions = new();
int nextId = 0;
for (var r = 0; r < input.Length; r++)
{
    for (var c = 0; c < input[0].Length; c++)
    {
        if (input[r][c] == '#')
        {
            elves.Add(nextId++, (r, c));
            occupiedPositions.Add((r, c));
        }
    }
}

var checks = new (List<int> checkList, int dir)[]
{
    (new() {0,1,7},0),
    (new() {4,3,5},4),
    (new() {6,7,5},6),
    (new() {2,1,3},2)
};

for (var round = 0; true; round++)
{
    Dictionary<(int r, int c), List<int>> proposedPositions = new();

    foreach (var elf in elves)
    {
        var position = elf.Value;
        var id = elf.Key;

        if (!allDirs.Any(dir => occupiedPositions.Contains((position.r + dir.r, position.c + dir.c))))
        {
            continue;
        }
        else
        {
            foreach(var check in checks)
            {
                if (!check.checkList.Select(c => allDirs[c]).Any(dir => occupiedPositions.Contains((position.r + dir.r, position.c + dir.c))))
                {
                    var dir = allDirs[check.dir];
                    var newDir = (position.r + dir.r, position.c + dir.c);
                    if (!proposedPositions.ContainsKey(newDir))
                    {
                        proposedPositions[newDir] = new();
                    }
                    proposedPositions[newDir].Add(id);
                    break;
                }
            }
        }
    }

    if (proposedPositions.Count() == 0 || proposedPositions.All(x => x.Value.Count > 1))
    {
        Console.WriteLine($"Part 2: {round+1}");
        break;
    }

    foreach(var p in proposedPositions)
    {
        if(p.Value.Count == 1)
        {
            occupiedPositions.Remove(elves[p.Value.First()]);
            elves[p.Value.First()] = p.Key;
            occupiedPositions.Add(p.Key);
        }
    }

    if (round == 9)
        Console.WriteLine($"Part 1: {getEmpty()}");

    //rotate checks
    checks = checks.Skip(1).Append(checks[0]).ToArray();
}

int getEmpty()
{
    var wMin = occupiedPositions.MinBy(p => p.c).c;
    var wMax = occupiedPositions.MaxBy(p => p.c).c;

    var hMin = occupiedPositions.MinBy(p => p.r).r;
    var hMax = occupiedPositions.MaxBy(p => p.r).r;

    var area = (wMax - wMin + 1) * (hMax - hMin + 1);

    var empty = area - occupiedPositions.Count;

    return empty;
}

