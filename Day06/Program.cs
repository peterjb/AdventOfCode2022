var input = File.ReadAllLines("input.txt").ToList()[0];

Console.WriteLine($"Part 1: {findMarker(input, 4)}");
Console.WriteLine($"Part 2: {findMarker(input, 14)}");

int findMarker(string input, int lengthOfMarker)
{
    for (var i = 0; i < input.Length - lengthOfMarker; i++)
    {
        var marker = new HashSet<char>(input.Skip(i).Take(lengthOfMarker));
        if (marker.Count == lengthOfMarker)
        {
            return i + lengthOfMarker;
        }
    }
    throw new Exception("Whoops");
}