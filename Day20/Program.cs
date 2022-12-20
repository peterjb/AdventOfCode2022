Console.WriteLine($"Part 1: {decrypt(1, 1)}");
Console.WriteLine($"Part 2: {decrypt(811589153, 10)}");

long decrypt(long key, int times)
{
    var a = File.ReadAllLines("input.txt").Select((x, i) => (i, long.Parse(x) * key)).ToArray();
    var length = a.Length;

    var ll = new LinkedList<(int index, long value)>(a);
    var d = new Dictionary<(int index, long value), LinkedListNode<(int index, long value)>>();

    var n = ll.First;
    while (n != null)
    {
        d.Add(n.Value, n);
        n = n.Next;
    }
    for (var ii = 0; ii < times; ii++)
    {
        foreach (var num in a)
        {
            var orig = d[num];
            var node = orig;
            var value = node.Value.value;

            if (value < 0)
            {
                var l = Math.Abs(value) % (length - 1);

                for (var i = 0; i < l; i++)
                {
                    node = node.Previous;
                    if (node == null)
                        node = ll.Last;
                    if (i == 0)
                        ll.Remove(orig);
                }
                d[orig.Value] = ll.AddBefore(node, orig.Value);
            }
            else if (value > 0)
            {
                var l = value % (length - 1);
                for (var i = 0; i < l; i++)
                {
                    node = node.Next;
                    if (node == null)
                        node = ll.First;
                    if (i == 0)
                        ll.Remove(orig);
                }
                d[orig.Value] = ll.AddAfter(node, orig.Value);
            }
        }
    }

    var zero = d.Where(x => x.Value.Value.value == 0).First().Value;
    long aa = go(ll, zero, 1000).value;
    long bb = go(ll, zero, 2000).value;
    long cc = go(ll, zero, 3000).value;
    return aa + bb + cc;
}


T go<T>(LinkedList<T> list, LinkedListNode<T> start, int amount)
{
    var node = start;
    for (var i = 0; i < amount; i++)
    {
        node = node.Next;
        if (node == null)
            node = list.First;
    }
    return node.Value;
}
