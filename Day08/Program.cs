var input = File.ReadAllLines("input.txt").ToArray();

var map = input.Select(x => x.Select(y => int.Parse(y.ToString())).ToArray()).ToArray();
var w = map[0].Length;
var h = map.Length;
var vismap = new bool[h, w];

for(var r = 0; r < h; r++)
{
    for(var c = 0; c < w; c++)
    {
        vismap[r, c] = true;
    }
}

var maxScore = 0;
for (var r = 1; r < h - 1; r++)
{
    for (var c = 1; c < w - 1; c++)
    {
        var score = 1;
        var blockedCount = 0;
        int r2, c2;

        var th = map[r][c];

        for (c2 = c-1; c2 >= 0; c2--)
        {
            if (map[r][c2] >= th)
            {
                blockedCount++;
                break;
            }
        }
        score *= c - Math.Max(c2, 0);

        for (c2 = c + 1; c2 < w; c2++)
        {
            if (map[r][c2] >= th)
            {
                blockedCount++;
                break;
            }
        }
        score *= Math.Min(c2,w-1) - c;

        for (r2 = r-1; r2 >= 0; r2--)
        {
            if (map[r2][c] >= th)
            {
                blockedCount++;
                break;
            }
        }
        score *= r - Math.Max(r2, 0);

        for (r2 = r + 1; r2 < h; r2++)
        {
            if (map[r2][c] >= th)
            {
                blockedCount++;
                break;
            }
        }
        score *= Math.Min(r2, h-1) - r;
        maxScore = Math.Max(score, maxScore);

        vismap[r, c] = blockedCount < 4;
    }
}
Console.WriteLine($"Part 1: {vismap.Cast<bool>().Count(x => x)}");
Console.WriteLine($"Part 2: {maxScore}");