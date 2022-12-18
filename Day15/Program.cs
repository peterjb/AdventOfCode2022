using System.Formats.Tar;

Dictionary<(int x, int y), int> sensors = new();
HashSet<(int x, int y)> beacons = new();

foreach(var sb in File.ReadAllLines("input.txt"))
{
    var x = sb.Split(":").Select(y => y.Split(",").Select(z => int.Parse(z)));
    var s = (x.First().First(), x.First().Last());
    var b = (x.Last().First(), x.Last().Last());
    sensors.Add(s, dist(s, b));
    beacons.Add(b);
}


HashSet<(int x, int y)> knockouts = new();

//test
var y = 10;
var min = 0;
var max = 20;
//input
//y = 2000000;
//min = 0;
//max = 4000000;

foreach (var s in sensors)
{
    var minDist = s.Value;
    var lineDist = Math.Abs(s.Key.y - y);

    if (minDist < lineDist)
        continue;

    var diff = minDist - lineDist;
    var start = s.Key.x - diff;

    for(var i = 0; i < 2*diff; i++)
    {
        knockouts.Add((start + i, y));
    }
}

Console.WriteLine(knockouts.Count());


HashSet<(int x, int y)> knockouts2 = new();
//for(var r = 0; r <= max; r++)
//{
//    for(var c = 0; c <= max; c++)
//    {
//        knockouts2.Add((c, r));
//    }
//}
//foreach (var s in sensors)
//{
//    var minDist = s.Value;
//    var sensor = s.Key;

//    for (var i = 0; i < minDist; i++)
//    {
//        for (var j = -minDist + i; j <= minDist - i; j++)
//        {
//            var ko = (x: sensor.x + j, y: sensor.y + i);
//            if (ko.x >= min && ko.x <= max && ko.y >= min && ko.y <= max)
//            {
//                var t = knockouts2.Add(ko);
//            }
//        }
//    }
//    for (var i = 1; i < minDist; i++)
//    {
//        for (var j = -minDist + i; j <= minDist - i; j++)
//        {
//            var ko = (x: sensor.x + j, y: sensor.y - i);
//            if (ko.x >= min && ko.x <= max && ko.y >= min && ko.y <= max)
//            {
//                var t = knockouts2.Add(ko);
//            }
//        }
//    }
//}

var sensorIntervals = sensors.Select(kvp =>
{
    var sensor = kvp.Key;
    var range = kvp.Value;

    var top = (x: sensor.x, y: sensor.y + range);
    var right = (x: sensor.x + range, y: sensor.y);
    var bottom = (x: sensor.x, y: sensor.y - range);
    var left = (x: sensor.x - range, y: sensor.y);

    return (xinterval: (left: left.x - left.y, right: bottom.x - bottom.y), yinterval: (bottom: left.x + left.y, top: top.x + top.y), sensor: kvp.Key);
});

SortedSet<int> xs = new();

foreach(var sensor in sensorIntervals)
{
    xs.Add(sensor.xinterval.left);
    xs.Add(sensor.xinterval.right);
}

var xs2 = xs.ToList();

List<(int left, int right)> xintervals = new();

for(var xi = 0; xi< xs2.Count-1;xi++)
{
    var first = xs2[xi];
    var next = xs2[xi + 1] - 1;

    xintervals.Add((first, next));
}

List<int> ys = new();

foreach (var sensor in sensorIntervals)
{
    ys.Add(sensor.xinterval.left);
    ys.Add(sensor.xinterval.right);
}

ys.Sort();

List<(int left, int right)> yintervals = new();

for (var xi = 0; xi < xs.Count - 1; xi++)
{
    var first = ys[xi];
    var next = ys[xi + 1];

    yintervals.Add((first, next));
}

bool done = false;

foreach (var interval in xintervals)
{
    var blocks = sensorIntervals.Where(i => i.xinterval.left <= interval.left && i.xinterval.right >= interval.right && !(i.yinterval.top < -interval.right))
        .OrderBy(i => i.yinterval.bottom).ToList();

    //if(blocks.First().yinterval.bottom > Math.Max(0,-interval.right) || blocks.Last().yinterval.top < max)
    //{
    //    throw new Exception();
    //}

    var bi = blocks[0];
    for(var i = 1; i < blocks.Count; i++)
    {
        if (blocks[i].yinterval.bottom > bi.yinterval.top + 1)
        {
            done = true;

            for (var xi = 0; xi <= interval.right - interval.left;xi++)
            {
                var tx = interval.left + xi;
                var ty = bi.yinterval.top + 1;

                long ttx = (tx + ty) / 2;
                long tty = (ty - tx) / 2;


            
                Console.WriteLine($"x:{ttx}; y:{tty} - {4000000 * ttx + tty}");
            }
            //throw new Exception();
            Console.WriteLine($"{blocks[i].yinterval}:{bi.yinterval}; {interval}");
            break;
        }
        else
        {
            bi.yinterval.top = Math.Max(bi.yinterval.top, blocks[i].yinterval.top);
        }
    }
    if (done)
        break;
}

var ssdfy = 100;



void union(KeyValuePair<(int x, int y), int> sensor, IEnumerable<((int x, int y) bl, (int x, int y) tr)> shapes)
{
    List<((int x, int y) bl, (int x, int y) tr)> newShapes = new();

    foreach (var s in shapes)
    {

    }
}

bool inSensorRange((int x, int y) sensor, int range, (int x, int y) point)
{
    var topLeft = (x: sensor.x, y: sensor.y + range);
    var bottomLeft = (x: sensor.x - range, y: sensor.y);
    var topRight = (x: sensor.x + range, y: sensor.y);

    var p = transform(point);

    return p.x >= topLeft.x && p.x <= topRight.x && p.y >= bottomLeft.y && p.y <= topLeft.y;
}



(double x, double y) transform((int x, int y) point)
{
    return (point.x / 2.0 - point.y / 2.0, point.x / 2.0 + point.y / 2.0);
}

int dist((int x, int y) lhs, (int x, int y) rhs)
{
    return Math.Abs(rhs.x - lhs.x) + Math.Abs(rhs.y - lhs.y);
}



