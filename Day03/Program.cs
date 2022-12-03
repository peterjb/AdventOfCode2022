Console.WriteLine($"Part 1: {File.ReadAllLines("input.txt").Sum(pack =>
{
    var c1 = pack.Take(pack.Length / 2).ToHashSet();
    foreach(var c in pack.Skip(pack.Length / 2))
    {
        if (c1.Contains(c))
        {
            return priority(c);
        }
    }
    throw new Exception("whoops");
})}");

var packs = File.ReadAllLines("input.txt");
var prioritySum = 0;
for(var i = 0; i < packs.Length; i+= 3)
{
    var p1 = packs[i].ToHashSet();
    var p2 = packs[i+1].ToHashSet();
    foreach(var c in packs[i+2])
    {
        if(p1.Contains(c) && p2.Contains(c))
        {
            prioritySum += priority(c);
            break;
        }
    }
}

Console.WriteLine($"Part 2: {prioritySum}");

int priority(char c) => char.IsLower(c) ? (int)c - 96 : (int)c - 38; 

