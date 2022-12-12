var input = File.ReadAllLines("input.txt").ToArray();

List<Monkey> monkeys = new List<Monkey>();

for(var i = 0; i < input.Length; i+=6)
{
    monkeys.Add(new Monkey(input[i + 1])
    {
        Items = input[i].Split(',').Select(j => new Item(int.Parse(j))).ToList(),
        TestValue = int.Parse(input[i + 2]),
        TestTrueMonkey = int.Parse(input[i + 3]),
        TestFalseMonkey = int.Parse(input[i + 4])
    }); 
}

for(var round = 0; round < 10000; round++)
{
    foreach(var m in monkeys)
    {
        turn(m);
    }
}

var top2 = monkeys.OrderByDescending(m => m.ExaminedCount).Take(2).ToList();

Console.WriteLine(top2[0].ExaminedCount * top2[1].ExaminedCount);

void turn(Monkey m)
{
    foreach(var item in m.Items)
    {
        m.WorryOperation(item);
        if(item.TestWorry(m.TestValue))
        {
            monkeys[m.TestTrueMonkey].Items.Add(item);
        }
        else
        {
            monkeys[m.TestFalseMonkey].Items.Add(item);
        }

        m.ExaminedCount++;
    }
    m.Items.Clear();
}

class Monkey
{
    public Monkey(string worryOperation)
    {
        var o = worryOperation.Split(' ').ToArray();

        int x = 0;

        if (o[2] != "old")
        {
            x = int.Parse(o[2]);
        }

        switch (o[1])
        {
            case "+":
                if (o[2] == "old")
                {
                    WorryOperation = (Item item) =>  item.MultiplyWorryBy(2);
                }
                else
                {
                    WorryOperation = (Item item) => item.AddToWorry(x); 
                }
                break;
            case "*":
                if ((o[2] == "old"))
                {
                    WorryOperation = (Item item) => item.SquareWorry();
                }
                else
                {
                    WorryOperation = (Item item) => item.MultiplyWorryBy(x);
                }
                break;
            default:
                throw new Exception("whoops");
        }

    }

    public List<Item> Items = new();
    public Action<Item> WorryOperation;
    public int TestValue;
    public int TestTrueMonkey;
    public int TestFalseMonkey;
    public long ExaminedCount;

    public bool check(Item item) => item.TestWorry(TestValue);
}


class Item
{
    Dictionary<int, int> Remainders;

    public Item(int worry)
    {
        Remainders = new()
        {
            { 11, worry % 11 },
            { 17, worry % 17 },
            { 5, worry % 5 },
            { 13, worry % 13 },
            { 19, worry % 19 },
            { 2, worry % 2 },
            { 3, worry % 3 },
            { 7, worry % 7 },
        };
    }

    public void AddToWorry(int x)
    {
        foreach(var d in Remainders)
        {
            Remainders[d.Key] = (d.Value + (x % d.Key)) % d.Key;
        }
    }

    public void MultiplyWorryBy(int x)
    {
        foreach (var d in Remainders)
        {
            Remainders[d.Key] = (d.Value * (x % d.Key)) % d.Key;
        }
    }

    public void SquareWorry()
    {
        foreach (var d in Remainders)
        {
            Remainders[d.Key] = (d.Value * d.Value) % d.Key;
        }
    }

    public bool TestWorry(int x)
    {
        return Remainders[x] == 0;
    }
}