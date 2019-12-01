using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using static System.Console;

namespace Day7
{
    public enum Operators
    {
        Transfer,
        Not,
        And,
        Or,
        LShift,
        RShift
    }

    public class GateCollection
    {
        private readonly Dictionary<string, List<Gate>> _inputGates;
        private readonly Dictionary<string, UInt16> _signals;

        public GateCollection()
        {
            _inputGates = new Dictionary<string, List<Gate>>();
            _signals = new Dictionary<string, UInt16>();
        }

        public void Add(Gate gate)
        {
            Process(gate);

            if (gate.InputA is GateRegister inputA)
            {
                AddInputGate(inputA.Name, gate);
                if (_signals.TryGetValue(inputA.Name, out var value))
                {
                    inputA.Value = value;
                }
            }
            if (gate.InputB is GateRegister inputB)
            {
                AddInputGate(inputB.Name, gate);
                if (_signals.TryGetValue(inputB.Name, out var value))
                {
                    inputB.Value = value;
                }
            }
        }

        public void ProcessAll()
        {
            var gates = new List<Gate>(_inputGates.Values.SelectMany(x => x).Distinct());

            int index = 0;
            var lastGatesLeft = 0;
  
            while (gates.Any())
            {
                if (index > gates.Count - 1)
                {
                    index = 0;
                    if (lastGatesLeft == gates.Count)
                    {
                        var processedGates = gates.Where(x => x.IsProcessed);

                        WriteLine("Got stuck :(");
                        WriteLine("Gates left to process:");
                        foreach (var gg in gates)
                        {
                            WriteLine(gg);
                        }
                        WriteLine("===============================");

                        return;
                    }
                    lastGatesLeft = gates.Count;
                    WriteLine($"Gates left: {gates.Count}");
                }

                Gate g = gates[index];
                Process(g);

                if (g.IsProcessed)
                {
                    gates.Remove(g);
                }
                else
                {
                    index++;
                }
            }
        }

        private void AddInputGate(string name, Gate gate)
        {
            List<Gate> gateList;
            if (!_inputGates.TryGetValue(name, out gateList))
            {
                gateList = new List<Gate>();
                _inputGates.Add(name, gateList);
                if (_inputGates.Count % 100_000 == 0)
                {
                    WriteLine($"InputGates count: {_inputGates.Count}");
                }
            }

            gateList.Add(gate);
        }

        private void Process(params Gate[] gatesArray)
        {
            foreach (var gate in gatesArray)
            {
                if (gate.Process())
                {
                    var name = gate.Output.Name;
                    var value = gate.Output.Value.Value;
                    _signals.Add(name, value);

                    if (_inputGates.TryGetValue(name, out var inputGates))
                    {
                        foreach (var ig in inputGates)
                        {
                            if (ig.InputA is GateRegister grA && grA.Name == name)
                            {
                                grA.Value = value;
                            }
                            else if (ig.InputB is GateRegister grB && grB.Name == name)
                            {
                                grB.Value = value;
                            }
                        }
                    }
                }
            }
        }

        public void PrintResult()
        {
            foreach (var kvp in _signals)
            {
                WriteLine($"{kvp.Key}: {kvp.Value}");
            }
        }
    }

    public interface IGateType { }

    public class GateValue : IGateType
    {
        public GateValue(UInt16 value)
        {
            Value = value;
        }

        public UInt16 Value { get; }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class GateRegister : IGateType
    {
        public GateRegister(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public UInt16? Value { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class Gate
    {
        private static int idCounter = 0;

        public Gate(Operators @operator, string outputRegister, string inputA, string inputB = null)
        {
            if (outputRegister == null)
            {
                throw new ArgumentNullException(outputRegister);
            }

            Output = new GateRegister(outputRegister);
            Operator = @operator;

            InputA = CreateGateType(inputA);
            InputB = CreateGateType(inputB);

            Id = idCounter;
            idCounter++;
        }

        private IGateType CreateGateType(string input)
        {
            Regex valueRegex = new Regex(@"\d+");
            Regex registerRegex = new Regex(@"[a-zA-Z]+");

            if (input == null)
            {
                return null;
            }

            if (valueRegex.IsMatch(input))
            {
                return new GateValue(UInt16.Parse(input));
            }
            else if (registerRegex.IsMatch(input))
            {
                return new GateRegister(input);
            }

            return null;
        }

        public int Id { get; }

        public Operators Operator { get; }

        public IGateType InputA { get; }

        public IGateType InputB { get; }

        public GateRegister Output { get; }

        public bool IsProcessed => Output?.Value != null;

        public override string ToString()
        {
            string output = $"[{Id}] ";

            if (InputB == null)
            {
                output += $"{Operator.ToString().ToUpper()} {InputA}";
            }
            else
            {
                output += $"{InputA} {Operator.ToString().ToUpper()} {InputB}";
            }

            output += " => " + Output.Name;

            return output;
        }

        public bool Process()
        {
            if (IsProcessed)
            {
                return false;
            }

            if (InputA == null)
            {
                return false;
            }

            if (InputB == null && Operator != Operators.Not && Operator != Operators.Transfer)
            {
                return false;
            }

            switch (Operator)
            {
                case Operators.Not:
                {
                    if (InputA is GateRegister inputValue && inputValue.Value.HasValue)
                    {
                        Output.Value = (UInt16)(~inputValue.Value);
                    }
                    else
                    {
                        return false;
                    }
                    break;
                }

                case Operators.Transfer:
                {
                    if (InputA is GateValue inputValue)
                    {
                        Output.Value = inputValue.Value;
                    }
                    else if (InputA is GateRegister inputAR && inputAR.Value.HasValue)
                    {
                        Output.Value = inputAR.Value.Value;
                    }
                    else
                    {
                        return false;
                    }
                    break;
                }

                case Operators.And:
                case Operators.Or:
                case Operators.LShift:
                case Operators.RShift:
                {
                    var result = EvaluateBinary(Operator, InputA, InputB);
                    if (result == null)
                    {
                        return false;
                    }

                    Output.Value = result;
                    break;
                }
            }

            return true;
        }

        private ushort? EvaluateBinary(Operators @operator, IGateType inputA, IGateType inputB)
        {
            UInt16 inputAValue;
            UInt16 inputBValue;

            if (inputA is GateRegister inputAGR && inputAGR.Value.HasValue)
            {
                inputAValue = inputAGR.Value.Value;
            }
            else if (InputA is GateValue inputAV)
            {
                inputAValue = inputAV.Value;
            }
            else
            {
                return null;
            }

            if (inputB is GateRegister inputBGR && inputBGR.Value.HasValue)
            {
                inputBValue = inputBGR.Value.Value;
            }
            else if (InputB is GateValue inputBV)
            {
                inputBValue = inputBV.Value;
            }
            else
            {
                return null;
            }

            switch (@operator)
            {
                case Operators.And:
                    return (UInt16)(inputAValue & inputBValue);

                case Operators.Or:
                    return (UInt16)(inputAValue | inputBValue);

                case Operators.LShift:
                    return (UInt16)(inputAValue << inputBValue);

                case Operators.RShift:
                    return (UInt16)(inputAValue >> inputBValue);

                default:
                    return null;
            }
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var start = DateTime.Now;
            new Program().Run(args);
            var end = DateTime.Now;

            var elapsed = end - start;
            WriteLine($"Elapsed time: {elapsed.Minutes}:{elapsed.Seconds}");
        }

        public void Run(string[] args)
        {
            IReader reader;
            if (args.Length > 0)
            {
                reader = new FileReader(args[0]);
            }
            else
            {
                reader = new ConsoleReader();
            }

            var gates = new GateCollection();
            var inputList = new List<string>();

            while (!reader.EndOfStream)
            {
                var input = reader.ReadLine();
                inputList.Add(input);
            }

            for (int i = 0; i < inputList.Count; i++)
            {
                var result = ParseInput(inputList[i]);

                if (result.Item2 != null)
                {
                    WriteLine($"Error: {result.Item2}");
                }
                else
                {
                    gates.Add(result.Item1);
                    WriteLine($"[{i + 1}/{inputList.Count}] Adding {result.Item1}");
                }
            }

            gates.ProcessAll();
            gates.PrintResult();
        }

        private (Gate, string) ParseInput(string input)
        {
            var split = input.Split("->");
            var output = split[1];
            var inputs = split[0].Split(" ");

            Gate gate = null;
            string error = null;

            if (inputs.Length == 1)
            {
                gate = new Gate(@operator: Operators.Transfer, inputA: inputs[0], outputRegister: output);
            }
            else if (inputs.Length == 2)
            {
                if (!inputs[0].Equals("NOT", StringComparison.OrdinalIgnoreCase))
                {
                    error = $"ERROR! NOT is the only valid unary operator: '{input}'";
                }
                else
                {
                    gate = new Gate(@operator: Operators.Not, inputA: inputs[1], outputRegister: output);
                }
            }
            else if (inputs.Length == 3)
            {
                if (!Enum.TryParse(inputs[1], true, out Operators op))
                {
                    error = $"ERROR! Could not parse operator: '{input}'";
                }
                else
                {
                    gate = new Gate(@operator: op, inputA: inputs[0], inputB: inputs[2], outputRegister: output);
                }
            }

            return (gate, error);
        }
    }

    static class StringExtensions
    {
        public static string[] Split(this string input, string token)
        {
            int tokenStart = 0;
            List<string> result = new List<string>();

            var processed = input;

            while (true)
            {
                tokenStart = processed.IndexOf(token);

                if (tokenStart == -1)
                {
                    processed = processed.Trim();

                    if (!string.IsNullOrEmpty(processed))
                    {
                        result.Add(processed);
                    }

                    break;
                }

                result.Add(processed.Substring(0, tokenStart).Trim());
                processed = processed.Substring(tokenStart + token.Length);
            }

            return result.ToArray();
        }
    }

    interface IReader
    {
        string ReadLine();
        bool EndOfStream { get; }
    }

    public class ConsoleReader : IReader
    {
        public string ReadLine()
        {
            return Console.ReadLine();
        }

        public bool EndOfStream => false;
    }

    public class FileReader : IReader
    {
        private readonly StreamReader _reader;

        public FileReader(string filename)
        {
            _reader = new StreamReader(filename);
        }

        public string ReadLine()
        {
            return _reader.ReadLine();
        }

        public bool EndOfStream
        {
            get
            {
                return _reader.EndOfStream;
            }
        }
    }
}