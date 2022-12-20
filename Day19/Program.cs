using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;

var lines = File.ReadAllLines("test.txt");

var blueprints = new List<Blueprint>();


foreach (var l in lines)
{
    var ms = Regex.Matches(l, @"\d+");
    blueprints.Add(new Blueprint(ms.Select(m => int.Parse(m.Value)).ToList()));
}

var x = 100;
var blueprintid = 0;

Dictionary<State, State> stateCache = new();
char[] robotTypes = new char[]
{
    'o', 'c', 's','g'
};

var results = new List<int>();

for (var bs = 0; bs < blueprints.Count(); bs++)
{
    blueprintid = bs;
    var g = doWork();
    var bg = (bs + 1) * g;
    results.Add(bg);
    Console.WriteLine($"Blueprint {bs + 1} => {g} => {bg}");
}

Console.WriteLine(results.Sum());

int doWork()
{
    stateCache = new();
    var state = new State() { minute = 0, oreMachines = 1 };
    var r = workHelper(state);
    //Console.WriteLine("hi");
    return r.geodes;
}


State workHelper(State state)
{
    if (stateCache.ContainsKey(state))
    {
        return stateCache[state];
    }


    if (state.clay < 0)
        throw new Exception();

    var nextState = state with
    {
        minute = state.minute + 1,
        ore = state.ore + state.oreMachines,
        clay = state.clay + state.clayMachines,
        obsidian = state.obsidian + state.obsidianMachines,
        geodes = state.geodes + state.geodeMachines
    };

    if (nextState.minute == 24)
    {
        return nextState;
    }
    var x = 24 - state.minute;
    var b = blueprints[blueprintid];

    int minTimeForClayMachine = (int)Math.Ceiling(Math.Max(b.clayOre - state.ore, 0) / (double)state.oreMachines) + 1;
    int minTimeForObsidianMachine = int.MaxValue;
    if(state.clayMachines == 0)
    {
       minTimeForObsidianMachine = Math.Max(
           minTimeForClayMachine + b.obsidianClay,
           (int)Math.Ceiling(Math.Max(b.obsidianOre - state.ore, 0) / (double)state.oreMachines)) + 1;
    }
    else
    {
        minTimeForObsidianMachine = Math.Max(
           (int)Math.Ceiling(Math.Max(b.obsidianClay - state.clay, 0) / (double)state.clayMachines),
           (int)Math.Ceiling(Math.Max(b.obsidianOre - state.ore, 0) / (double)state.oreMachines)) + 1;
    }
    int minTimeForGeodeMachine = int.MaxValue;
    if(state.obsidianMachines == 0)
    {
        minTimeForGeodeMachine = Math.Max(
            minTimeForObsidianMachine + b.geodeObsidian,
            (int)Math.Ceiling(Math.Max(b.geodeOre - state.ore, 0) / (double)state.oreMachines)) + 1;
    }
    else
    {
        minTimeForGeodeMachine = Math.Max(
            (int)Math.Ceiling(Math.Max(b.geodeObsidian - state.obsidian, 0) / (double)state.obsidianMachines),
            (int)Math.Ceiling(Math.Max(b.geodeOre - state.ore, 0) / (double)state.oreMachines)) + 1;
    }


    if (x < minTimeForGeodeMachine)
        //(state.geodeMachines == 0 && 
        //(state.obsidianMachines == 0 && x < b.geodeObsidian ||
        //state.obsidianMachines != 0 && x < Math.Max(Math.Max(0, b.geodeObsidian - state.obsidian)/ (float)state.obsidianMachines, Math.Max(0, (b.geodeOre - state.ore))/(float)state.oreMachines)) 
        //|| (state.obsidianMachines == 0 && state.clayMachines != 0 && x <= b.geodeObsidian + Math.Max(Math.Max(0, b.obsidianClay - state.clay)/(float)state.clayMachines, Math.Max(0, b.obsidianOre - state.ore)/(float)state.oreMachines))
        ////||
        ////(x - b.geodeObsidian - b.obsidianClay < Math.Max(0, (b.clayOre - state.ore)) && state.clayMachines == 0)
        //)
    {
        return nextState;
    }

    //if (x < (b.geodeObsidian - state.obsidian) + (b.geodeOre - state.ore)
    //    && state.geodeMachines == 0)
    //{
    //    return nextState;
    //}

    var result = workHelper(nextState);
    foreach (var c in robotTypes)
    {

        if (canCreateRobot(blueprintid, c, state))
        {
            if (c == 's')
            {
                var alkj = 100;
            }
            var t = workHelper(createRobot(blueprintid, c, nextState));
            if (t.geodes > result.geodes)
            {
                result = t;
            }
        }
    }
    if (!stateCache.ContainsKey(state))
        stateCache.Add(state, result);
    else
    {
        var y = 1200;
    }

    return result;
}


bool canCreateRobot(int blueprintId, char robotType, State state)
{
    switch (robotType)
    {
        case 'o':
            return state.ore >= blueprints[blueprintid].oreOre;
        case 'c':
            return state.ore >= blueprints[blueprintid].clayOre;
        case 's':
            return state.ore >= blueprints[blueprintid].obsidianOre && state.clay >= blueprints[blueprintid].obsidianClay;
        case 'g':
            return state.ore >= blueprints[blueprintid].geodeOre && state.obsidian >= blueprints[blueprintid].geodeObsidian;
        default:
            return false;
    }
}

State createRobot(int bluePrintId, char robotType, State state)
{
    switch (robotType)
    {
        case 'o':
            state.ore -= blueprints[blueprintid].oreOre;
            state.oreMachines++;
            break;
        case 'c':
            state.ore -= blueprints[blueprintid].clayOre;
            state.clayMachines++;
            break;
        case 's':
            state.ore -= blueprints[blueprintid].obsidianOre;
            state.clay -= blueprints[blueprintid].obsidianClay;
            state.obsidianMachines++;
            break;
        case 'g':
            state.ore -= blueprints[blueprintid].geodeOre;
            state.obsidian -= blueprints[blueprintid].geodeObsidian;
            state.geodeMachines++;
            break;
        default:
            break;
    }

    return state;
}

public record struct State(
    int minute,
    int ore,
    int clay,
    int obsidian,
    int geodes,
    int oreMachines,
    int clayMachines,
    int obsidianMachines,
    int geodeMachines
);

struct Blueprint
{
    public Blueprint(List<int> values)
    {
        id = values[0];
        oreOre = values[1];
        clayOre = values[2];
        obsidianOre = values[3];
        obsidianClay = values[4];
        geodeOre = values[5];
        geodeObsidian = values[6];
    }

    public int id;
    public int oreOre;
    public int clayOre;
    public int obsidianOre;
    public int obsidianClay;
    public int geodeOre;
    public int geodeObsidian;
}
