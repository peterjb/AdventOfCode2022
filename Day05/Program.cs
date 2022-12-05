using System.Text.RegularExpressions;

//read input top down into a set of lists, then create the stacks from the reverse lists
//then manipulate stacks according to problem
//there's probably a more concise way to do this...

var lines = File.ReadAllLines("input.txt").ToList();

var stacks1 = new Stack<char>[9];
var stacks2 = new Stack<char>[9];

var lists = new List<char>[9];

for(var i = 0; i < lists.Length; i++)
{
    lists[i] = new List<char>();
}

var index = 0;
while (!string.IsNullOrWhiteSpace(lines[index]))
{
    for (var i = 0; i < 9; i++)
    {
        var nextBox = i * 4 + 1;

        if (nextBox >= lines[index].Length)
            break;

        var box = lines[index][nextBox];
        if (box != ' ')
        {
            lists[i].Add(box);
        }
    }
    index++;
}

for (var i = 0; i < stacks1.Length; i++)
{
    stacks1[i] = new Stack<char>(lists[i].AsEnumerable().Reverse());
    stacks2[i] = new Stack<char>(lists[i].AsEnumerable().Reverse());
}

foreach (var line in lines.Skip(index+1)) 
{
    var g = Regex.Matches(line, "\\d+").Select(m => int.Parse(m.Value)).ToArray();
    var temp = new List<char>();
    for(var j = 0; j < g[0]; j++)
    {
        //part 1
        stacks1[g[2] - 1].Push(stacks1[g[1] - 1].Pop());

        //part 2
        temp.Add(stacks2[g[1] - 1].Pop());
    }
    foreach(var j in temp.AsEnumerable().Reverse())
    {
        //part 2
        stacks2[g[2] - 1].Push(j);
    }
}

Console.Write("Part 1: ");
foreach(var s in stacks1)
{
    Console.Write(s.Peek());
}
Console.WriteLine();
Console.Write("Part 2: ");
foreach (var s in stacks2)
{
    Console.Write(s.Peek());
}