using static System.Console;

public record struct Point (int X, int Y, int Z);
public record struct Scanner (string label, Point[] points);

public static class PointExtensions
{
    public static long SquareDistanceTo(this Point point, Point other)
    {
        var dx = Math.Abs(point.X - other.X);
        var dy = Math.Abs(point.Y - other.Y);
        var dz = Math.Abs(point.Z - other.Z);

        return dx*dx + dy*dy + dz*dz;
    }
}

public class Day19
{
    public void Part1()
    {
        var scanners = ParseInput(data);
        var squareDistanceList = CalculateSquareDistances(scanners);
        var squareDistancePairs = PairSquareDistances(squareDistanceList);

        var maxKey = (-1, -1);
        var maxVal = 0;
        foreach (var (key, val) in squareDistancePairs)
        {
            if (maxVal < val.Count)
            {
                maxVal = val.Count;
                maxKey = key;
            }
        }

        if (maxKey == (-1, -1))
        {
            WriteLine("-");
        }
        else
        {
            WriteLine($"{maxKey}: {squareDistancePairs[maxKey].Count}");
            WriteLine();

            foreach (var _ in squareDistancePairs[maxKey].Take(3))
            {
                var p1 = scanners[_.s1].points[_.s1_p1];
                var p2 = scanners[_.s1].points[_.s1_p2];
                var s1Δ = new Point(
                    X: Math.Abs(p1.X-p2.X),
                    Y: Math.Abs(p1.Y-p2.Y),
                    Z: Math.Abs(p1.Z-p2.Z)
                );
                WriteLine(s1Δ);

                p1 = scanners[_.s2].points[_.s2_p1];
                p2 = scanners[_.s2].points[_.s2_p2];
                var s2Δ = new Point(
                    X: Math.Abs(p1.X-p2.X),
                    Y: Math.Abs(p1.Y-p2.Y),
                    Z: Math.Abs(p1.Z-p2.Z)
                );
                WriteLine(s2Δ);
                WriteLine();
            }
        }
    }

    private Dictionary<(int, int), List<(int s1, int s2, int s1_p1, int s1_p2, int s2_p1, int s2_p2)>>
    PairSquareDistances(List<(int scanner, int p1, int p2, long dist)> squareDistanceList)
    {
        (int scanner, int p1, int p2, long dist) last = (0, 0, 0, 0);

        var candidates = new Dictionary<(int, int), List<(int s1, int s2, int s1_p1, int s1_p2, int s2_p1, int s2_p2)>>();

        foreach (var (s, p1, p2, dst) in squareDistanceList.OrderBy(_ => _.dist).ThenBy(_ => _.scanner))
        {
            if (last.dist == dst)
            {
                var key = (last.p1, p1);
                if (!candidates.TryGetValue(key, out var candidateList))
                {
                    candidateList = new();
                    candidates.Add(key, candidateList);
                }
                candidateList.Add((s, last.scanner, p1, p2, last.p1, last.p2));
            }

            last = (s, p1, p2, dst);
        }

        return candidates;
    }

    private List<(int scanner, int p1, int p2, long dist)> CalculateSquareDistances(Scanner[] scanners)
    {
        List<(int scanner, int p1, int p2, long dist)> distList = new();

        for (int s_idx = 2; s_idx < 4; s_idx++)
        for (var p1_idx = 0; p1_idx < scanners[s_idx].points.Length; p1_idx++)
        {
            for (var p2_idx = p1_idx+1; p2_idx < scanners[s_idx].points.Length; p2_idx++)
            {
                var p1 = scanners[s_idx].points[p1_idx];
                var p2 = scanners[s_idx].points[p2_idx];
                distList.Add((s_idx, p1_idx, p2_idx, p1.SquareDistanceTo(p2)));
            }
        }

        return distList;
    }

    private Scanner[] ParseInput(string data)
    {
        var scannerList = new List<Scanner>();
        var points = new List<Point>();
        var label = string.Empty;

        foreach (var line in data.Replace("\r", "").Split('\n'))
        {
            if (string.IsNullOrEmpty(line)) continue;

            if (line.StartsWith("---"))
            {
                if (!string.IsNullOrEmpty(label))
                {
                    scannerList.Add(new(label: label, points: points.ToArray()));
                }

                label = line[4..^4];
                points.Clear();
            }
            else
            {
                var coords = line.Split(",").Select(_ => int.Parse(_)).ToArray();
                points.Add(new(X: coords[0], Y: coords[1], Z: coords[2]));
            }
        }
        scannerList.Add(new(label: label, points: points.ToArray()));

        return scannerList.ToArray();
    }

    const string data = """
--- scanner 0 ---
404,-588,-901
528,-643,409
-838,591,734
390,-675,-793
-537,-823,-458
-485,-357,347
-345,-311,381
-661,-816,-575
-876,649,763
-618,-824,-621
553,345,-567
474,580,667
-447,-329,318
-584,868,-557
544,-627,-890
564,392,-477
455,729,728
-892,524,684
-689,845,-530
423,-701,434
7,-33,-71
630,319,-379
443,580,662
-789,900,-551
459,-707,401

--- scanner 1 ---
686,422,578
605,423,415
515,917,-361
-336,658,858
95,138,22
-476,619,847
-340,-569,-846
567,-361,727
-460,603,-452
669,-402,600
729,430,532
-500,-761,534
-322,571,750
-466,-666,-811
-429,-592,574
-355,545,-477
703,-491,-529
-328,-685,520
413,935,-424
-391,539,-444
586,-435,557
-364,-763,-893
807,-499,-711
755,-354,-619
553,889,-390

--- scanner 3 ---
-589,542,597
605,-692,669
-500,565,-823
-660,373,557
-458,-679,-417
-488,449,543
-626,468,-788
338,-750,-386
528,-832,-391
562,-778,733
-938,-730,414
543,643,-506
-524,371,-870
407,773,750
-104,29,83
378,-903,-323
-778,-728,485
426,699,580
-438,-605,-362
-469,-447,-387
509,732,623
647,635,-688
-868,-804,481
614,-800,639
595,780,-596

--- scanner 4 ---
727,592,562
-293,-554,779
441,611,-461
-714,465,-776
-743,427,-804
-660,-479,-426
832,-632,460
927,-485,-438
408,393,-506
466,436,-512
110,16,151
-258,-428,682
-393,719,612
-211,-452,876
808,-476,-593
-575,615,604
-485,667,467
-680,325,-822
-627,-443,-432
872,-547,-609
833,512,582
807,604,487
839,-516,451
891,-625,532
-652,-548,-490
30,-46,-14
""";
}