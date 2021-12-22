using System.Reflection;
using static System.Console;

public record Cube ((int min, int max) x, (int min, int max) y, (int min, int max) z);

public static class CubeExtensions
{
    public static IEnumerable<Cube> Union(this Cube cube1, Cube cube2)
    {
        if (cube1.x == cube2.x && cube1.y == cube2.y)
        {
            if (cube1.z.max < cube2.z.min-1 || cube2.z.max < cube1.z.min-1)
            {
                return new[] { cube1, cube2 };
            }

            var z = (Math.Min(cube1.z.min, cube2.z.min), Math.Max(cube1.z.max, cube2.z.max));
            return new[] { new Cube(cube1.x, cube1.y, z) };
        }
        if (cube1.x == cube2.x && cube1.z == cube2.z)
        {
            if (cube1.y.max < cube2.y.min-1 || cube2.y.max < cube1.y.min-1)
            {
                return new[] { cube1, cube2 };
            }

            var y = (Math.Min(cube1.y.min, cube2.y.min), Math.Max(cube1.y.max, cube2.y.max));
            return new[] { new Cube(cube1.x, y, cube1.z) };
        }
        if (cube1.y == cube2.y && cube1.z == cube2.z)
        {
            if (cube1.x.max < cube2.x.min-1 || cube2.x.max < cube1.x.min-1)
            {
                return new[] { cube1, cube2 };
            }

            var x = (Math.Min(cube1.x.min, cube2.x.min), Math.Max(cube1.x.max, cube2.x.max));
            return new[] { new Cube(x, cube1.y, cube1.z) };
        }

        return Enumerable.Empty<Cube>();
    }

    public static int Size(this Cube cube) =>
        (cube.x.max - cube.x.min + 1) *
        (cube.y.max - cube.y.min + 1) *
        (cube.x.max - cube.x.min + 1);
}

public class Day22
{
    public static void Main()
    {
        new Day22Tests().RunTests();
    }
}

public class Day22Tests
{
    [Test]
    void SimpleXUnion1()
    {
/*
   0   1   2   3   4

1  |---|---o---o---|
   |   |   |   |   |
2  |---|---|---|---|
   |   |   |   |   |
3  |---|---o---o---|
*/
        var cube1 = new Cube((0, 3), (1, 3), (0, 3));
        var cube2 = new Cube((2, 4), (1, 3), (0, 3));

        var expectedResult = new Cube[]
        {
            new Cube((0, 4), (1, 3), (0, 3))
        };

        AssertUnionForAllDimensions(
            cube1: cube1,
            cube2: cube2,
            expectedResult: expectedResult
        );
    }

    [Test]
    void SimpleXUnion2()
    {
/*
   0   1   2   3   4

1  |---|   |---|---|
   |   |   |   |   |
2  |---|   |---|---|
   |   |   |   |   |
3  |---|   |---|---|
*/
        var cube1 = new Cube((0, 1), (1, 3), (0, 3));
        var cube2 = new Cube((2, 4), (1, 3), (0, 3));

        var expectedResult = new Cube[]
        {
            new Cube((0, 4), (1, 3), (0, 3))
        };

        AssertUnionForAllDimensions(
            cube1: cube1,
            cube2: cube2,
            expectedResult: expectedResult
        );
    }

    [Test]
    void SimpleXUnion3()
    {
/*
   0   1   2   3   4   5

1  |---|       |---|---|
   |   |       |   |   |
2  |---|       |---|---|
   |   |       |   |   |
3  |---|       |---|---|
*/
        var cube1 = new Cube((0, 1), (1, 3), (0, 3));
        var cube2 = new Cube((3, 5), (1, 3), (0, 3));

        var expectedResult = new Cube[]
        {
            new Cube((0, 1), (1, 3), (0, 3)),
            new Cube((3, 5), (1, 3), (0, 3))
        };

        AssertUnionForAllDimensions(
            cube1: cube1,
            cube2: cube2,
            expectedResult: expectedResult
        );
    }

    [Test]
    void ComplexUnion1()
    {
/*
   0   1    2   3   4

0  |---|    |---|

1  |---|    |---|---|
   |   |    |   |   |
2  |---|    |---|---|
   |   |    |   |   |
3  |---|    |---o---|
*/
        var cube1 = new Cube((0, 3), (0, 3), (0, 2));
        var cube2 = new Cube((2, 4), (1, 3), (0, 2));

        var expectedResult = new Cube[] {
            new Cube((0, 1), (0, 0), (0, 2)),
            new Cube((2, 3), (0, 0), (0, 2)),
            new Cube((0, 1), (1, 3), (0, 2)),
            new Cube((2, 4), (1, 3), (0, 2))
        };

        AssertUnionForAllDimensions(
            cube1: cube1,
            cube2: cube2,
            expectedResult: expectedResult
        );
    }

    public void RunTests()
    {
        var testMethods = typeof(Day22Tests).GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        foreach (var method in testMethods.Where(m => m.CustomAttributes.Any(attr => attr.AttributeType == typeof(TestAttribute))))
        {
            WriteLine($"Running {method.Name}: ");
            method.Invoke(this, new object[] {});
            WriteLine();
        }
    }

    public void AssertUnionForAllDimensions(Cube cube1, Cube cube2, params Cube[] expectedResult)
    {
        var dimensionMutations = new[]
        {
            "xyz", "xzy",
            "yxz", "yzx",
            "zxy", "zyx"
        };

        foreach (var mutation in dimensionMutations)
        {
            var cube1Mutation = Mutate(cube1, mutation);
            var cube2Mutation = Mutate(cube2, mutation);
            var mutatedExpectation = expectedResult.Select(x => Mutate(x, mutation));

            Assert(
                cube1Mutation.Union(cube2Mutation),
                expected: mutatedExpectation,
                $"{cube1Mutation} U {cube2Mutation}");

            Assert(
                cube2Mutation.Union(cube1Mutation),
                expected: mutatedExpectation,
                $"{cube2Mutation} U {cube1Mutation}");
        }
    }

    public Cube Mutate(Cube cube, string configuration)
    {
        if (configuration.Length != 3
            || !configuration.Contains('x')
            || !configuration.Contains('y')
            || !configuration.Contains('z'))
        {
            throw new InvalidOperationException();
        }

        return new Cube(
            x: configuration[0] switch { 'x' => cube.x, 'y' => cube.y, 'z' => cube.z, _ => throw new InvalidOperationException() },
            y: configuration[1] switch { 'x' => cube.x, 'y' => cube.y, 'z' => cube.z, _ => throw new InvalidOperationException() },
            z: configuration[2] switch { 'x' => cube.x, 'y' => cube.y, 'z' => cube.z, _ => throw new InvalidOperationException() }
        );
    }

    void Assert(IEnumerable<Cube> actual, IEnumerable<Cube> expected, string? message = null)
    {
        var actualList = actual.ToList();
        var expectedList = expected.ToList();

        if (actualList.Count != expectedList.Count)
        {
            ForegroundColor = ConsoleColor.Red;
            WriteLine("FAILURE");
            ResetColor();
            Write($"Expected ");
            ForegroundColor = ConsoleColor.Cyan;
            Write(expectedList.Count);
            ResetColor();
            Write(" cubes but found ");
            ForegroundColor = ConsoleColor.Red;
            WriteLine(actualList.Count);
            ResetColor();

            if (message != null)
            {
                WriteLine(message);
            }

            return;
        }

        foreach (var expectedCube in expectedList)
        {
            if (!actualList.Any(actual =>
                actual.x == expectedCube.x &&
                actual.y == expectedCube.y &&
                actual.z == expectedCube.z))
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine("FAILURE");
                ResetColor();
                Write($"Expected cube ");
                ForegroundColor = ConsoleColor.Cyan;
                Write(expectedCube);
                ResetColor();
                WriteLine(" not found");
                ResetColor();

                if (message != null)
                {
                    WriteLine(message);
                }

                return;
            }
        }

        ForegroundColor = ConsoleColor.Green;
        WriteLine("SUCCESS");
        ResetColor();
    }
}

public class TestAttribute : Attribute { }
