var input = File.ReadAllLines("input.txt").ToArray();

var instCycles = new Dictionary<string, int>()
{
    { "noop", 1 },
    { "addx", 2 }
};

var reg = 1;
var framebuf = new char[6, 40];

int cycle = 0;
int signalStrength = 0;

foreach (var i in input)
{
    var inst = i.Split(' ');
    var cycles = instCycles[inst[0]];

    for (var n = 0; n < cycles; n++)
    {
        int col = cycle % 40;
        int row = cycle / 40;

        cycle++;

        //part 2
        if (col >= reg - 1 && col <= reg + 1)
        {
            framebuf[row, col] = '#';
        }
        else
        {
            framebuf[row, col] = '.';
        }

        //part 1
        if ((cycle + 20) % 40 == 0)
        {
            signalStrength += cycle * reg;
        }

        //add happens at end of cycle
        if (n == 1 && inst[0] == "addx")
        {
            reg = reg + int.Parse(inst[1]);
        }
    }

    if (cycle == 240)
        break;
}
Console.WriteLine($"Part 1: {signalStrength}");
Console.WriteLine($"Part 2:");
for (var r = 0; r < 6; r++)
{
    for (var c = 0; c < 40; c++)
    {
        Console.Write(framebuf[r, c]);
    }
    Console.WriteLine();
}
