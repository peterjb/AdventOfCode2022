var lines = File.ReadAllLines("input.txt").ToArray();


Dictionary<string, node> nodes = new Dictionary<string, node>();
Dictionary<string, node> moreNodes = new();

foreach (var line in lines)
{
    int i = line.IndexOf(':');
    string name = line.Substring(0, i);
    var o = line.Substring(i + 1).Split(' ', StringSplitOptions.RemoveEmptyEntries);
    if (o.Length == 1)
        nodes.Add(name, new node() { name = name, number = long.Parse(o[0]) });
    else
        nodes.Add(name, new node() { name = name, lhs = o[0], op = o[1][0], rhs = o[2] });
}

Console.WriteLine(doOp("root"));

Console.WriteLine(solve());

long doOp(string nodeName)
{
    var node = nodes[nodeName];
    if (node.number.HasValue) 
        return node.number.Value;
    else
    {
        var lhs = doOp(node.lhs!);
        var rhs = doOp(node.rhs!);
        switch (node.op)
        {
            case '+':
                return lhs + rhs;
            case '-':
                return lhs - rhs;
            case '*':
                return lhs * rhs;
            case '/':
                return lhs / rhs;
            default:
                throw new Exception();
        }
    }
}

node buildVariableTree(string nodeName)
{
    var node = nodes[nodeName];
    var n = new node() { name = node.name };

    if (node.name == "humn")
        n.variable = true;

    else if (node.number.HasValue)
        n.number = node.number;
    else
    {
        var lhs = buildVariableTree(node.lhs!);
        var rhs = buildVariableTree(node.rhs!);
        if (lhs.number.HasValue && rhs.number.HasValue)
        {
            switch (node.op)
            {
                case '+':
                    n.number = lhs.number + rhs.number;
                    break;
                case '-':
                    n.number = lhs.number - rhs.number;
                    break;
                case '*':
                    n.number = lhs.number * rhs.number;
                    break;
                case '/':
                    n.number = lhs.number / rhs.number;
                    break;
                default:
                    throw new Exception();
            }
        } 
        else
        {
            n.lhs = lhs.name;
            n.rhs = rhs.name;
            n.op = node.op;
        }
    }

    moreNodes.Add(n.name, n);
    return n;
}

long solve()
{
    var root = buildVariableTree("root");
    var lhs = moreNodes[root.lhs];
    var rhs = moreNodes[root.rhs];

    long n;
    node expr;
    if (lhs.number.HasValue)
    {
        n = lhs.number.Value;
        expr = rhs;
    }
    else
    {
        n = rhs.number.Value;
        expr = lhs;
    }

    while (expr.name != "humn")
    {
        var nlhs = moreNodes[expr.lhs];
        var nrhs = moreNodes[expr.rhs];

        switch (expr.op)
        {
            case '+':
                if(nlhs.number.HasValue)
                {
                    n -= nlhs.number.Value;
                    expr = nrhs;
                }
                else
                {
                    n -= nrhs.number.Value;
                    expr = nlhs;
                }
                break;
            case '-':
                if (nlhs.number.HasValue)
                {
                    n -= nlhs.number.Value;
                    n *= -1;
                    expr = nrhs;
                }
                else
                {
                    n += nrhs.number.Value;
                    expr = nlhs;
                }
                break;
            case '*':
                if (nlhs.number.HasValue)
                {
                    n /= nlhs.number.Value;
                    expr = nrhs;
                }
                else
                {
                    n /= nrhs.number.Value;
                    expr = nlhs;
                }
                break;
            case '/':
                if (nlhs.number.HasValue)
                {
                    n = nlhs.number.Value / n;
                    expr = nrhs;
                }
                else
                {
                    n *= nrhs.number.Value;
                    expr = nlhs;
                }
                break;
            default:
                throw new Exception();
        }
    }

    return n;
}

class node
{
    public string? name;
    public long? number;
    public char? op;
    public string? lhs;
    public string? rhs;
    public bool variable;


}
