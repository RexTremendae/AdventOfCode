using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using static System.Console;

TestClass.TestAll(true);
WriteLine();

bool fillRuleSet = true;
var ruleSet = new List<string>();
var expressions = new List<string>();

foreach (var line in File
    .ReadAllLines("Day19.txt"))
{
    if (string.IsNullOrWhiteSpace(line))
    {
        fillRuleSet = false;
        continue;
    }

    if (fillRuleSet)
    {
        ruleSet.Add(line.Replace("\"", "'"));
    }
    else
    {
        expressions.Add(line);
    }
}

var evaluator = new Evaluator(ruleSet.ToArray());
WriteLine("------");
WriteLine(expressions.Sum(xp => evaluator.Evaluate(xp) ? 1 : 0));
WriteLine("------");
WriteLine();

public class RuleTreeNode
{
    private static int _nextId = 0;

    public RuleTreeNode()
    {
        Data = string.Empty;
        _children = new();
        Id = _nextId;
        _nextId++;
        Mode = ChildMode.Sequential;
    }

    public RuleTreeNode(int index, ChildMode mode) : this()
    {
        Index = index;
        Mode = mode;
    }

    public RuleTreeNode(int index) : this()
    {
        Index = index;
    }

    public RuleTreeNode(int index, string data) : this()
    {
        Index = index;
        Data = data;
    }

    public void AddChild(RuleTreeNode child)
    {
        child.Parent = this;
        _children.Add(child);
    }

    public int Id { get; }
    public int Index { get; }
    public string Data { get; }
    public ChildMode Mode { get; }
    private List<RuleTreeNode> _children;
    public IEnumerable<RuleTreeNode> Children => _children;
    public RuleTreeNode? Parent { get; private set; }

    public enum ChildMode
    {
        Sequential,
        SelectOne
    }
}

public class Evaluator
{
    RuleTreeNode _rules = new RuleTreeNode(int.MinValue);
    Dictionary<int, object> _ruleLookup = new();

    public Evaluator(params string[] ruleSet)
    {
        LoadRules(ruleSet);
        //PrintRules();
    }

    public bool Evaluate(string expression)
    {
        var result = Evaluate(expression, new[] {("", new List<int>())}.ToList(), _rules);
/*
        foreach (var r in result)
        {
            WriteLine($"'{expression}' matched by '{r.Item1}' ({string.Join(" ", r.Item2)})");
        }
*/
        return result.Any(xp => xp.Item1.Length == expression.Length);
    }

    public List<(string, List<int>)> Evaluate(string expression, List<(string, List<int>)> matches, RuleTreeNode node)
    {
        matches = matches.ToList();
        var result = new List<(string, List<int>)>();
        if (node.Data != string.Empty)
        {
            foreach (var m in matches)
            {
                if (m.Item1.Length < expression.Length && expression[m.Item1.Length] == node.Data[0])
                {
                    result.Add((m.Item1 + node.Data[0], m.Item2.Append(node.Id).ToList()));
                }
            }
            return result;
        }

        if (node.Mode == RuleTreeNode.ChildMode.Sequential)
        {
            foreach (var child in node.Children)
            {
                //WriteLine($"[{node.Id}] Evaluating [{child.Id}]: {expression}: {string.Join(", ", matches)} (SEQ)");
                matches = Evaluate(expression, matches, child);
                /*
                if (!matches.Any())
                    WriteLine($"[{child.Id}] {expression} => <no matches> (SEQ)");
                foreach (var m in matches)
                    WriteLine($"[{child.Id}] {expression} => {child.Index}: {m} (SEQ)");
                */
            }

            return matches;
        }
        else
        {
            var candidates = new List<(string, List<int>)>();
            foreach (var child in node.Children)
            {
                var candidateMatches = Evaluate(expression, matches, child);
                candidates.AddRange(candidateMatches);
                /*
                foreach (var m in candidateMatches)
                    WriteLine($"[{child.Id}] {expression} => {child.Index}: {m} (OPT)");
                */
            }

            return candidates;
        }
    }

    private void PrintRuleLookup()
    {
        foreach ((var idx, var data) in _ruleLookup)
        {
            if (data is char ch) WriteLine($"{idx}: [{ch}]");
            else if (data is List<int> list) WriteLine($"{idx}: {string.Join(" ", list.Select(l => l.ToString()))}");
            else if (data is List<List<int>> listOfLists)
            {
                var output = "";
                bool first = true;
                foreach(var innerList in listOfLists)
                {
                    if (!first) output += " | ";
                    output += string.Join(" ", innerList.Select(l => l.ToString()));
                    first = false;
                }
                WriteLine($"{idx}: {output}");
            }
            else WriteLine($"{idx}: ??");
        }
        WriteLine();
    }

    private void PrintRules()
    {
        Print(_rules, 0);
        WriteLine();
    }

    private void Print(RuleTreeNode node, int level)
    {
        for (int i = 0; i < level; i ++)
            Write("| ");
        WriteLine($"+[{node.Id}] {node.Index}: {(node.Data != "" ? $"<{node.Data}>" : "")} ({(node.Mode == RuleTreeNode.ChildMode.Sequential ? "SEQ" : "OPT")})");
        foreach (var ch in node.Children)
        {
            Print(ch, level+1);
        }
    }

    private void LoadRules(params string[] ruleSet)
    {
        BuildRuleLookup(ruleSet);
        InitRuleTree();
    }

    private void InitRuleTree()
    {
        var childNodes = new List<RuleTreeNode>();
        var root = new RuleTreeNode(-1);
        var q = new List<(RuleTreeNode node, int[] ruleIndices)>();
        q.Add((node: root, ruleIndices: new [] { 0 }));

        while(q.Any())
        {
            (var parent, var indices) = q[0];
            q.RemoveAt(0);
            foreach (var idx in indices)
            {
                var rule = _ruleLookup[idx];
                if (rule is char ch)
                {
                    var node = new RuleTreeNode(idx, data: ch.ToString());
                    parent.AddChild(node);
                }
                else if (rule is List<int> list)
                {
                    var node = new RuleTreeNode(idx);
                    parent.AddChild(node);
                    q.Add((node: node, ruleIndices: list.ToArray()));
                }
                else if (rule is List<List<int>> listOfLists)
                {
                    var node = new RuleTreeNode(idx, RuleTreeNode.ChildMode.SelectOne);
                    parent.AddChild(node);
                    foreach (var innerList in listOfLists)
                    {
                        var childNode = new RuleTreeNode();
                        node.AddChild(childNode);
                        q.Add((node: childNode, ruleIndices: innerList.ToArray()));
                    }
                }
                else throw new InvalidOperationException("Invalid data stored in lookup dictionary!");
            }
        }

        _rules = root.Children.First();
    }

    private void BuildRuleLookup(params string[] ruleSet)
    {
        _ruleLookup.Clear();
        foreach (var rule in ruleSet.Where(r => !string.IsNullOrWhiteSpace(r)))
        {
            var parts = rule.Split(':').Select(p => p.Trim()).ToArray();
            if (parts[0] == "8")
            {
                parts[1] = "42 | 42 42 | 42 42 42 | 42 42 42 42 | 42 42 42 42 42";
            }
            else if (parts[0] == "11")
            {
                parts[1] = "42 31 | 42 42 31 31 | 42 42 42 31 31 31 | 42 42 42 42 31 31 31 31 | 42 42 42 42 42 31 31 31 31 31";
            }
            var index = int.Parse(parts[0]);
            if (parts[1][0] == '\'') _ruleLookup.Add(index, parts[1][1]);
            else
            {
                var lookupValue = new List<List<int>>();
                foreach (var list in parts[1].Split('|')
                                             .Select(s => s.Trim()
                                                           .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                                           .Select(i => int.Parse(i))))
                {
                    var value = new List<int>(list);
                    lookupValue.Add(value);
                }
                if (lookupValue.Count == 1)
                {
                    _ruleLookup.Add(index, lookupValue[0]);
                }
                else
                {
                    _ruleLookup.Add(index, lookupValue);
                }
            }
        }
    }
}

public class TestClass
{
    string[] ruleSet = new[] {
        "0: 4 1 5",
        "1: 2 3 | 3 2",
        "2: 4 4 | 5 5",
        "3: 4 5 | 5 4",
        "4: 'a'",
        "5: 'b'"
    };

    public void _1() => _evaluator.Evaluate("ababbb").ShouldBe(true);
    public void _2() => _evaluator.Evaluate("bababa").ShouldBe(false);
    public void _3() => _evaluator.Evaluate("abbbab").ShouldBe(true);
    public void _4() => _evaluator.Evaluate("aaabbb").ShouldBe(false);
    public void _5() => _evaluator.Evaluate("aaaabbb").ShouldBe(false);

    Evaluator _evaluator;

    public TestClass()
    {
        _evaluator = new Evaluator(ruleSet);
    }

    public static void TestAll(bool showStacktrace = false)
    {
        WriteLine();

        var successCount = 0;
        var failureCount = 0;

        var testClass = new TestClass();

        foreach (var method in typeof(TestClass)
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
        {
            try
            {
                method.Invoke(testClass, new Type[] {});
                successCount++;
            }
            catch (Exception ex)
            {
                failureCount++;
                Write($"{method.Name}: ");
                ForegroundColor = ConsoleColor.Red;
                WriteLine(ex?.InnerException?.Message);
                if (showStacktrace)
                {
                    WriteLine(ex?.InnerException?.StackTrace);
                }
                WriteLine();
                ResetColor();
            }
        }

        WriteResult(successCount, failureCount);
    }

    private static void WriteResult(int successCount, int failureCount)
    {
        if (failureCount == 0)
        {
            ForegroundColor = ConsoleColor.Green;
            Write(successCount);
            ResetColor();
            Write(" test cases ");
            ForegroundColor = ConsoleColor.Green;
            WriteLine("succeeded!");
            ResetColor();
        }
        else
        {
            ForegroundColor = ConsoleColor.Green;
            Write(successCount);
            ResetColor();
            Write(" out of ");
            ForegroundColor = ConsoleColor.Cyan;
            Write(successCount + failureCount);
            ResetColor();
            WriteLine(" test cases succeeded. ");

            ForegroundColor = ConsoleColor.Red;
            Write(failureCount);
            ResetColor();
            WriteLine(" failed!");
        }
    }

    private static void WriteFailure(int id, string actual, string expected, bool reversed = false)
    {
        Write("Test case ");
        ForegroundColor = ConsoleColor.Cyan;
        Write($"#{id} ");
        if (reversed) Write("(reversed) ");
        ForegroundColor = ConsoleColor.Red;
        Write("failed!");
        ResetColor();
        Write(" Expected '");
        ForegroundColor = ConsoleColor.Green;
        Write(expected);
        ResetColor();
        Write("' but got '");
        ForegroundColor = ConsoleColor.Red;
        Write(actual);
        ResetColor();
        WriteLine("'");
    }
}

public static class Extensions
{
    public static void ShouldBe(this object actual, object expected)
    {
        if (!actual.Equals(expected))
        {
            throw new Exception($"Expected {expected.ToString()}, but got {actual.ToString()}.");
        }
    }
}
