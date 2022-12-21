using System.Text.RegularExpressions;

var lines = File.ReadAllLines("input.txt");

var blueprints = new List<Blueprint>();

foreach (var l in lines)
{
    var ms = Regex.Matches(l, @"\d+");
    blueprints.Add(new Blueprint(ms.Select(m => int.Parse(m.Value)).ToList()));
}

var blueprintid = 0;

Dictionary<State, State> stateCache = new();
char[] robotTypes = new char[]
{
    'o', 'c', 's','g'
};

var results = new List<int>();
var results2 = new List<int>();

int memoCount = 0;
int maxGeodes = int.MinValue;


int time = 24;

for (var bs = 0; bs < blueprints.Count(); bs++)
{
    maxGeodes = int.MinValue;
    blueprintid = bs;
    var g = doWork();
    var bg = (bs + 1) * g;
    results2.Add(bg);
}

Console.WriteLine($"Part 1: {results2.Sum()}");

time = 32;

for (var bs = 0; bs < 3; bs++)
{
    maxGeodes = int.MinValue;
    blueprintid = bs;
    var g = doWork();
    results.Add(g);
}

Console.WriteLine($"Part 2: {results.Aggregate(1, (agg, x) => x * agg)}");

int doWork()
{
    stateCache.Clear();
    var state = new State() { minute = 0, oreMachines = 1 };
    var r = workHelper(state);
    return r.geodes;
}

State workHelper(State state)
{
    if (stateCache.ContainsKey(state))
    {
        memoCount++;
        return stateCache[state];
    }

    var nextState = state with
    {
        minute = state.minute + 1,
        ore = state.ore + state.oreMachines,
        clay = state.clay + state.clayMachines,
        obsidian = state.obsidian + state.obsidianMachines,
        geodes = state.geodes + state.geodeMachines
    };

    if (nextState.minute == time)
    {
        maxGeodes = Math.Max(maxGeodes, nextState.geodes);
        return nextState;
    }

    var x = time - state.minute;
    var b = blueprints[blueprintid];

    //to limit the search space, these are loose upper bound calculations that calculate how much of a substance we will have in the time remaining
    //if we could create a machine of that type every tick the rest of the way, which is the max possible
    //these upper bounds are very loose, but good enough to finish in a reasonable time/space
    // MaxSubstance(t) = initialSubstanceCount + initialSubstanceMachineCount * t + Sum(1..(t-1)) 
    // takes about a minute and 2gb ram. I'm sure much tighter bounds could be found
    var tmaxGeodes = state.geodes + (state.geodeMachines * x) + sumN(x - 1);
    var tmaxObsidian = state.obsidian + (state.obsidianMachines * x) + sumN(x - 1);
    var tmaxClay = state.clay + (state.clayMachines * x) + sumN(x - 1);
    var tmaxOre = state.ore + (state.oreMachines * x) + sumN(x - 1);

    //upper bound on geodes with the remaining time
    if (tmaxGeodes < maxGeodes)
    {
        return state;
    }

    //upper bound on obsidian to see if we can create a geode machine in the remaining time
    if (state.geodeMachines == 0 && tmaxObsidian < b.geodeObsidian)
    {
        return state;
    }

    //upper bound on clay to see if we can create an obsidian machine in the remaining time
    if (tmaxClay < b.obsidianClay && state.obsidianMachines == 0)
    {
        return state;
    }

    State temp = default;
    State result = default;
    foreach (var c in robotTypes)
    {
        if (canCreateRobot(blueprintid, c, state))
        {
            temp = workHelper(createRobot(blueprintid, c, nextState));
            if (temp.geodes > result.geodes)
            {
                result = temp;
            }
        }
    }
    temp = workHelper(nextState);
    if(temp.geodes > result.geodes)
    {
        result = temp;
    }

    stateCache.Add(state, result);

    return result;
}

int sumN(int n)
{
    return n * (n + 1) / 2;
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
