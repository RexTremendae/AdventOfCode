using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using static System.Console;

namespace AoC
{
    public class MainClass
    {
        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                WriteLine("Please provide the name of a file containing code to interpret.");
                return;
            }

            var program = Program.ParseSourceCode(args[0]);
            if (program == null)
            {
                WriteLine("Error occurred! Could not run program.");
                return;
            }

            program.Run();
        }
    }

    public abstract class InstructionBase
    {
        public InstructionBase(Program program)
        {
            Program = program;
        }

        protected Program Program { get; private set; }

        public abstract void Execute();
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ArgumentAttribute : Attribute
    {
        public ArgumentAttribute(int position)
        {
            Position = position;
        }

        public int Position { get; }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class IdentifierAttribute : Attribute
    {
        public IdentifierAttribute(string identifier)
        {
            Identifier = identifier;
        }

        public string Identifier;
    }

    [Identifier("cpy")]
    public class Copy : InstructionBase
    {
        public Copy(Program program) : base(program) {}

        public override void Execute()
        {
        }
    }

    [Identifier("inc")]
    public class Increase : InstructionBase
    {
        [Argument(position: 0)]
        public string Registry { get; set; }

        public Increase(Program program) : base(program) {}

        public override void Execute()
        {
            Program.UpdateRegistry(Registry, Program.ReadRegistry(Registry) + 1);
        }
    }

    [Identifier("dec")]
    public class Decrease : InstructionBase
    {
        [Argument(position: 0)]
        public string Registry { get; set; }

        public Decrease(Program program) : base(program) {}

        public override void Execute()
        {
            Program.UpdateRegistry(Registry, Program.ReadRegistry(Registry) - 1);
        }
    }

    public class Program
    {
        private readonly List<InstructionBase> _instructions;
        private int _instructionPointer;

        private readonly Dictionary<string, int> _registry;

        public IEnumerable<InstructionBase> Instructions => _instructions;

        private Program()
        {
            _instructions = new List<InstructionBase>();
            _registry = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        }

        public void Run()
        {
        }

        public int ReadRegistry(string name)
        {
            int value = 0;
            _registry.TryGetValue(name, out value);

            return value;
        }

        public void UpdateRegistry(string name, int value)
        {
            _registry[name] = value;
        }

        public static Program ParseSourceCode(string filepath)
        {
            var typeDictionary = PrepareInstructionDictionary();
            if (typeDictionary == null)
            {
                return null;
            }

            var program = new Program();

            WriteLine("Processing instructions in source file...");
            foreach (var lineIterator in File.ReadAllLines(filepath))
            {
                var line = lineIterator.Trim();
                var firstSpaceIdx = line.IndexOf(' ');
                if (firstSpaceIdx > 0)
                {
                    var instruction = line.Substring(0, firstSpaceIdx);
                    var arguments = line.Substring(firstSpaceIdx + 1).Split(",");
                    var instructionMetadata = typeDictionary[instruction];
                    var instructionInstance = Activator.CreateInstance(instructionMetadata.type);
                    program._instructions.Add(instructionInstance);

                    foreach (var arg in instructionMetadata.arguments)
                    {
                        switch (arg.type)
                        {
                            case typeof(string):
                                break;

                            case typeof(int):
                                break;

                            default:
                                break;
                        }
                    }
                }
            }

            return new Program();
        }

        public static Dictionary<string, (Type type, List<(string name, Type type)[]> arguments)> PrepareInstructionDictionary()
        {
            var error = false;
            var types = new Dictionary<string, (Type, List<(string, Type)[]>)>(StringComparer.OrdinalIgnoreCase);

            WriteLine("Analyzing available instructions...");
            var instructionTypes = typeof(Program).Assembly
                    .GetTypes().Where(x => typeof(InstructionBase)
                               .IsAssignableFrom(x) && !x.IsAbstract);

            foreach (var type in instructionTypes)
            {
                var identifier = (type.GetCustomAttributes(typeof(IdentifierAttribute), false)
                                      .SingleOrDefault() as IdentifierAttribute)?.Identifier;
                if (string.IsNullOrEmpty(identifier))
                {
                    WriteLine($"Instruction '{type.Name}' is missing identifier.");
                    error = true;
                }

                WriteLine($"Found {type.Name}");
                var args = new List<(int position, (string name, Type type) property)>();
                foreach (var property in type.GetProperties())
                {
                    var attribute = property.GetCustomAttributes(typeof(ArgumentAttribute), false)
                                            .SingleOrDefault() as ArgumentAttribute;
                    if (attribute != null)
                    {
                        args.Add((attribute.Position, (property.Name, property.PropertyType)));
                    }
                }

                int idx = 0;
                var argsDictionary = new Dictionary<int, List<(string name, Type type)>>();
                foreach (var item in args)
                {
                    if (!argsDictionary.TryGetValue(item.position, out var list))
                    {
                        list = new List<(string name, Type type)>();
                        argsDictionary.Add(item.position, list);
                    }

                    foreach (var existing in list)
                    {
                        if (existing.type == item.property.type)
                        {
                            WriteLine($"More than one argument of the same type at the same position for instruction '{type.Name}'.");
                            error = true;
                            break;
                        }
                    }
                    if (error) break;

                    list.Add(item.property);
                }

                var argsList = new List<(string name, Type type)[]>();
                foreach (var kvp in argsDictionary.OrderBy(x => x.Key))
                {
                    if (kvp.Key != idx)
                    {
                        WriteLine($"Instruction '{type.Name}' is missing argument at position {idx}.");
                        error = true;
                        break;
                    }

                    argsList.Add(kvp.Value.ToArray());
                    idx++;
                }

                if (error) break;

                types.Add(identifier, (type, argsList));
            }

            if (error) return null;

            return types;
        }
    }
}
