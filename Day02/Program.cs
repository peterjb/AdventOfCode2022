//Part 1 states
var states = new Dictionary<string, int>()
{
    { "A X", 1 + 3 },
    { "A Y", 2 + 6 },
    { "A Z", 3 + 0 },
    { "B X", 1 + 0 },
    { "B Y", 2 + 3 },
    { "B Z", 3 + 6 },
    { "C X", 1 + 6 },
    { "C Y", 2 + 0 },
    { "C Z", 3 + 3 },
};

Console.WriteLine($"Part 1: {File.ReadAllLines("input.txt").Sum(x => states[x])}");

//Part 2, convert new interpretation of state to equivalent part 1 state
Console.WriteLine($"Part 2: {File.ReadAllLines("input.txt").Select(x =>
{
    return x switch
    {
        "A X" => "A Z",
        "A Y" => "A X",
        "A Z" => "A Y",
        "C X" => "C Y",
        "C Y" => "C Z",
        "C Z" => "C X",
        _ => x,
    };
}).Sum(x => states[x])}");