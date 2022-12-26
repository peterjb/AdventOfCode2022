using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

Dictionary<string, int> digits = new()
{
    {"2", 2 },
    {"1", 1 },
    {"0", 0 },
    {"-", -1 },
    {"=", -2 },
};

var additionTable = new Dictionary<(char lhs, char rhs), string>()
{
    {('2','2'), "1-"},
    {('2','1'), "1="},
    {('2','0'), "2"},
    {('2','-'), "1"},
    {('2','='), "0"},

    {('1','1'), "2"},
    {('1','0'), "1"},
    {('1','-'), "0"},
    {('1','='), "-"},

    {('0','0'), "0"},
    {('0','-'), "-"},
    {('0','='), "="},

    {('-','-'), "="},
    {('-','='), "-2"},

    {('=','='), "-1"}
};

//var tests = new List<int>()
//{
//    23,
//    1747,
//  906,
//  198,
//   11,
//  201,
//   31,
// 1257,
//   32,
//  353,
//  107,
//    7,
//    3,
//   37
//};
////var result = File.ReadAllLines("input.txt").Aggregate(add);

//foreach(var t in tests)
//{
//    Console.WriteLine($"{t}: {convertl(t)}");
//}

var result = File.ReadAllLines("input.txt").Aggregate(0L, (x, y) =>
{
    return x + convert(y);
});

Console.WriteLine($"Part 1: {convertl(result)}");

long convert(string snafu)
{
    long pow = 1;
    long sum = 0;
    for (var i = 0; i < snafu.Length; i++)
    {
        var d = snafu.Substring(snafu.Length - i - 1, 1);
        sum += digits[d] * pow;
        pow *= 5;
    }
    return sum;
}

string convertl(long dec)
{
    List<long> ranges = new();
    StringBuilder snafu = new();
    var max = 0L;
    long pow = 1;
    int exp = 0;
    while (true)
    {
        max = 2 * pow + max;
        ranges.Add(max);
        if (Math.Abs(dec) <= max)
        {
            break;
        }
        pow *= 5;
        exp++;
    }

    if (exp == 0)
    {
        return dec < 0 ? flip(dec.ToString()) : dec.ToString();
    }

    var rng = (min: ranges[exp - 1], max: ranges[exp]);
    var d = (Math.Abs(dec) + rng.min) / pow;
    var rem = Math.Abs(dec) - (d * pow);

    var nd = digits.First(x => x.Value == d).Key;

    if (dec < 0)
    {
        snafu.Append(flip(nd));
    }
    else
    {
        snafu.Append(digits.First(x => x.Value == d).Key);
    }

    string rest;
    if (rem < 0)
    {
        rest = flip(convertl(Math.Abs(rem)));
    }
    else
    {
        rest = convertl(rem);
    }

    return snafu + new string('0', exp - rest.Length) + rest;
}

string flip(string snafu)
{
    return new string(snafu.Select(c =>
    {
        switch (c)
        {
            case '2':
                return '=';
            case '1':
                return '-';
            case '-':
                return '1';
            case '=':
                return '2';
            default:
                return '0';
        }
    }).ToArray());
}
/*
string add(string lhs, string rhs)
{
    StringBuilder sum = new();
    bool iscarry = false;
    char carry = '0';
    string a, b;

    if (lhs.Length >= rhs.Length)
    {
        a = lhs;
        b = rhs;
    }
    else
    {
        a = rhs;
        b = lhs;
    }
    int i, j;
    for (i = b.Length - 1, j = a.Length - 1; i >= 0; i--, j--)
    {
        var bi = b[i];
        var aj = a[j];
        string digitSum;

        var pr = addD(aj, bi);

        if (iscarry && pr.c.HasValue)
        {
            var r = add(carry.ToString(), pr.c.ToString() + pr.r);

            if (r.Length == 2)
            {
                iscarry = true;
                carry = r[0];
                sum.Append(r[1]);
            }
            else
            {
                sum.Append(r[0]);
            }
        }
        else if (iscarry)
        {
            var ppr = addD(pr.r, carry);
            if (ppr.c.HasValue)
            {
                carry = ppr.c.Value;
                iscarry = true;
            }
            else
            {
                iscarry = false;
            }
            sum.Append(ppr.r);
        }
        else
        {
            if (pr.c.HasValue)
            {
                iscarry = true;
                carry = pr.c.Value;
            }
            else
            {
                iscarry = false;
            }
            sum.Append(pr.r);
        }
    }

    for (; j >= 0; j--)
    {
        if (iscarry)
        {
            var n = addD(a[j], carry);
            if (!n.c.HasValue)
                iscarry = false;
            sum.Append(n.r);
        }
        else
        {
            sum.Append(a[j]);
        }
    }

    return new string(sum.ToString().Reverse().ToArray());
}
*/
//(char? c, char r) addD(char a, char b)
//{
//    string s;
//    if (digits[a] < digits[b])
//    {
//        s = additionTable[(b, a)];
//    }
//    else
//    {
//        s = additionTable[(a, b)];
//    }
//    if (s.Length > 1)
//    {
//        return (s[0], s[1]);
//    }
//    else
//    {
//        return (null, s[0]);
//    }
//}