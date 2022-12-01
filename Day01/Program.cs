var calorieSumList = new List<long>();

long sum = 0;

foreach (var i in File.ReadAllLines("input.txt"))
{
    if (string.IsNullOrWhiteSpace(i))
    {
        calorieSumList.Add(sum);
        sum = 0;
    }
    else
    {
        sum += long.Parse(i);
    }
}

Console.WriteLine($"Part 1: {calorieSumList.OrderDescending().First()}");
Console.WriteLine($"Part 2: {calorieSumList.OrderDescending().Take(3).Sum()}");