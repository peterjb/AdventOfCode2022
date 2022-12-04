var pairs = File.ReadAllLines("input.txt")
    .Select(p => p.Split(',')
        .Select(r => r.Split('-')
            .Select(i => int.Parse(i)).ToList())
        .ToList());

Console.WriteLine($"Part 1: {pairs.Count(pair =>
{
    return (pair[0][0] >= pair[1][0] && pair[0][1] <= pair[1][1]) ||
        (pair[1][0] >= pair[0][0] && pair[1][1] <= pair[0][1]);
})}");

Console.WriteLine($"Part 2: {pairs.Count(pair =>
{
    return (pair[0][0] >= pair[1][0] && pair[0][1] <= pair[1][1]) ||
        (pair[1][0] >= pair[0][0] && pair[1][1] <= pair[0][1]) ||
        (pair[0][0] <= pair[1][0] && pair[1][0] <= pair[0][1]) ||
        (pair[1][0] <= pair[0][0] && pair[0][0] <= pair[1][1]);
})}");



