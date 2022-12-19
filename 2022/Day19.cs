using static System.Console;

namespace AoC_19;

using Blueprint = System.Collections.Generic.Dictionary<Product, (int amount, Product component)[]>;

public enum Product
{
    ore,
    clay,
    obsidian,
    geode
};

public class Day19
{
    Product[] products = Enum.GetValues<Product>().Reverse().ToArray();

    public async Task Part1()
    {
        await foreach (var (id, blueprint) in ReadInput("Day19_small.txt"))
        {
            var resources = products.ToDictionary(key => key, value => 0);
            var robots = products.ToDictionary(key => key, value => 0);
            robots[Product.ore] = 1;

            var nextMake = Product.ore;

            foreach (var cnt in 0.To(24))
            {
                var maxMakeCount = 0;
                foreach (var prd in products)
                {
                    var canMake = CanMake(prd, blueprint, resources, robots);
                    if (maxMakeCount < canMake && nextMake < prd)
                    {
                        maxMakeCount = canMake;
                        nextMake = prd;
                        WriteLine($" decided on next make: {nextMake}");
                    }
                }

                var nonOreReq = blueprint[nextMake].Where(_ => _.component != Product.ore);

                var toMake = nextMake;
                if (nonOreReq.Any())
                {
                    var nextReq = nonOreReq.Single();
                    if (nextReq.amount > resources[nextReq.component]) toMake = Product.ore;
                }

                var newRobot = (Product?)null;

                if (resources[Product.ore] >= blueprint[toMake].Single(_ => _.component == Product.ore).amount)
                {
                    foreach (var (amount, component) in blueprint[toMake])
                    {
                        resources[component] -= amount;
                    }

                    newRobot = toMake;
                    if (toMake == nextMake) nextMake = Product.ore;
                    WriteLine($" producing one {newRobot} robot");
                }

                foreach (var (component, count) in robots)
                {
                    resources[component] += count;
                }

                if (newRobot != null)
                {
                    robots[newRobot.Value]++;
                    newRobot = null;
                }
                Write($"{(cnt+1).ToString().PadLeft(2)}: ");
                foreach (var prd in products)
                {
                    Write($"[{prd}: {resources[prd].ToString().PadLeft(2)} (+{robots[prd].ToString().PadLeft(2)})]  ");
                }
                WriteLine();
            }

            WriteLine($"Blueprint {id}: {resources[Product.geode]}");
            WriteLine("---------------------------");
            break;
        }
    }

    public async Task Part1_fu()
    {
        await foreach (var (id, blueprint) in ReadInput("Day19_small.txt"))
        {
            var resources = products.ToDictionary(key => key, value => 0);
            var robots = products.ToDictionary(key => key, value => 0);
            robots[Product.ore] = 1;

            var nextMake = Product.ore;

            foreach (var cnt in 0.To(24))
            {
                var maxMakeCount = 0;
                foreach (var prd in products)
                {
                    var canMake = CanMake(prd, blueprint, resources, robots);
                    if (maxMakeCount < canMake && nextMake < prd)
                    {
                        maxMakeCount = canMake;
                        nextMake = prd;
                        WriteLine($" decided on next make: {nextMake}");
                    }
                }

                var nonOreReq = blueprint[nextMake].Where(_ => _.component != Product.ore);

                var toMake = nextMake;
                if (nonOreReq.Any())
                {
                    var nextReq = nonOreReq.Single();
                    if (nextReq.amount > resources[nextReq.component]) toMake = Product.ore;
                }

                var newRobot = (Product?)null;

                if (resources[Product.ore] >= blueprint[toMake].Single(_ => _.component == Product.ore).amount)
                {
                    foreach (var (amount, component) in blueprint[toMake])
                    {
                        resources[component] -= amount;
                    }

                    newRobot = toMake;
                    if (toMake == nextMake) nextMake = Product.ore;
                    WriteLine($" producing one {newRobot} robot");
                }

                foreach (var (component, count) in robots)
                {
                    resources[component] += count;
                }

                if (newRobot != null)
                {
                    robots[newRobot.Value]++;
                    newRobot = null;
                }
                Write($"{(cnt+1).ToString().PadLeft(2)}: ");
                foreach (var prd in products)
                {
                    Write($"[{prd}: {resources[prd].ToString().PadLeft(2)} (+{robots[prd].ToString().PadLeft(2)})]  ");
                }
                WriteLine();
            }

            WriteLine($"Blueprint {id}: {resources[Product.geode]}");
            WriteLine("---------------------------");
            break;
        }
    }

    private int CanMake(Product product, Blueprint blueprint, Dictionary<Product, int> resources, Dictionary<Product, int> robots)
    {
        if (product == Product.ore) return 0;

        var prdIdx = products.Length-1;
        while (products[prdIdx] != product) prdIdx--;
        prdIdx++;
        var rq = products[prdIdx];
        var result = (resources[rq] + robots[rq]) / blueprint[product].Single(_ => _.component == rq).amount;

        //WriteLine($" evaluating CanMake: {result} {products[prdIdx]}");
        return result;
    }

    public async IAsyncEnumerable<(int, Blueprint)> ReadInput(string filename)
    {
        await foreach(var line in System.IO.File.ReadLinesAsync(filename))
        {
            if (string.IsNullOrEmpty(line)) yield break;
            yield return Parse(line);
        }
    }

    private (int, Blueprint) Parse(string line)
    {
        var blueprint = new Blueprint();
        var labelEndIdx = line.IndexOf(":");
        var requirements = new List<(int, Product)>();

        foreach (var r in line[(labelEndIdx+1)..].Trim().Split(".", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
        {
            var tokens = r.Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            var product = Enum.Parse<Product>(tokens[1]);

            var component = Enum.Parse<Product>(tokens[5]);
            var amount = int.Parse(tokens[4]);
            requirements.Add((amount, component));

            if (tokens.Length > 6)
            {
                component = Enum.Parse<Product>(tokens[8]);
                amount = int.Parse(tokens[7]);
                requirements.Add((amount, component));
            }
            blueprint.Add(product, requirements.ToArray());
            requirements.Clear();
        }

        var id = int.Parse(line[..labelEndIdx].Split(" ")[1]);
        return (id, blueprint);
    }
}
