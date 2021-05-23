use std::collections::HashMap;
use std::vec::Vec;

type Address = i64;
type InstructionByte = i64;
type IOData = i64;

#[derive(Debug)]
enum ParameterMode
{
    Positional,
    Immediate,
    Relative
}

#[derive(Debug)]
enum Direction
{
    North,
    East,
    South,
    West
}

#[derive(Debug)]
enum Instruction
{
    Exit,
    Add(ParameterMode, ParameterMode, ParameterMode),
    Multiply(ParameterMode, ParameterMode, ParameterMode),
    Input(ParameterMode),
    Output(ParameterMode),
    JumpIfTrue(ParameterMode, ParameterMode),
    JumpIfFalse(ParameterMode, ParameterMode),
    LessThan(ParameterMode, ParameterMode, ParameterMode),
    Equals(ParameterMode, ParameterMode, ParameterMode),
    AdjustRelativeBase(ParameterMode)
}

struct IntProgram<T> where T: IOHandler
{
    memory: HashMap<Address, InstructionByte>,
    relative_base: InstructionByte,
    io: T
}

trait IOHandler
{
    fn input_handler(& mut self) -> IOData;
    fn output_handler(& mut self, data: IOData);
}

struct Program
{
    position: (IOData, IOData),
    direction: Direction,
    map: HashMap<(IOData, IOData), IOData>,
    next_data_point_is_paint_instruction: bool
}

// ------------------ //
//  -->>  Main  <<--  //
// ------------------ //

fn main()
{
    let program = Program::new();
    let mut int_program = IntProgram::new(
        vec![
        //  -->>  Day 11 puzzle input  <<--
        // --------------------------------
        3,8,1005,8,329,1106,0,11,0,0,0,104,1,104,0,3,8,102,-1,8,10,1001,10,1,10,4,10,1008,8,0,10,4,10,1002,8,1,29,2,1102,1,10,1,1009,16,10,2,4,4,10,1,9,5,10,3,8,1002,8,-1,10,101,1,10,10,4,10,108,0,8,10,4,10,101,0,8,66,2,106,7,10,1006,0,49,3,8,1002,8,-1,10,101,1,10,10,4,10,108,1,8,10,4,10,1002,8,1,95,1006,0,93,3,8,102,-1,8,10,1001,10,1,10,4,10,108,1,8,10,4,10,102,1,8,120,1006,0,61,2,1108,19,10,2,1003,2,10,1006,0,99,3,8,1002,8,-1,10,1001,10,1,10,4,10,1008,8,0,10,4,10,101,0,8,157,3,8,102,-1,8,10,1001,10,1,10,4,10,1008,8,1,10,4,10,1001,8,0,179,2,1108,11,10,1,1102,19,10,3,8,102,-1,8,10,1001,10,1,10,4,10,1008,8,1,10,4,10,101,0,8,209,2,108,20,10,3,8,1002,8,-1,10,101,1,10,10,4,10,108,1,8,10,4,10,101,0,8,234,3,8,102,-1,8,10,101,1,10,10,4,10,108,0,8,10,4,10,1002,8,1,256,2,1102,1,10,1006,0,69,2,108,6,10,2,4,13,10,3,8,102,-1,8,10,101,1,10,10,4,10,1008,8,0,10,4,10,1002,8,1,294,1,1107,9,10,1006,0,87,2,1006,8,10,2,1001,16,10,101,1,9,9,1007,9,997,10,1005,10,15,99,109,651,104,0,104,1,21101,387395195796,0,1,21101,346,0,0,1105,1,450,21101,0,48210129704,1,21101,0,357,0,1105,1,450,3,10,104,0,104,1,3,10,104,0,104,0,3,10,104,0,104,1,3,10,104,0,104,1,3,10,104,0,104,0,3,10,104,0,104,1,21101,0,46413147328,1,21102,404,1,0,1106,0,450,21102,179355823323,1,1,21101,415,0,0,1105,1,450,3,10,104,0,104,0,3,10,104,0,104,0,21102,1,838345843476,1,21101,0,438,0,1105,1,450,21101,709475709716,0,1,21101,449,0,0,1105,1,450,99,109,2,22102,1,-1,1,21102,40,1,2,21101,0,481,3,21101,0,471,0,1105,1,514,109,-2,2105,1,0,0,1,0,0,1,109,2,3,10,204,-1,1001,476,477,492,4,0,1001,476,1,476,108,4,476,10,1006,10,508,1101,0,0,476,109,-2,2106,0,0,0,109,4,2101,0,-1,513,1207,-3,0,10,1006,10,531,21101,0,0,-3,21201,-3,0,1,21201,-2,0,2,21101,1,0,3,21101,550,0,0,1105,1,555,109,-4,2106,0,0,109,5,1207,-3,1,10,1006,10,578,2207,-4,-2,10,1006,10,578,21201,-4,0,-4,1105,1,646,22101,0,-4,1,21201,-3,-1,2,21202,-2,2,3,21101,597,0,0,1105,1,555,22102,1,1,-4,21101,0,1,-1,2207,-4,-2,10,1006,10,616,21101,0,0,-1,22202,-2,-1,-2,2107,0,-3,10,1006,10,638,22102,1,-1,1,21101,638,0,0,106,0,513,21202,-2,-1,-2,22201,-4,-2,-4,109,-5,2106,0,0
        // Paint robot, expected result depends on input
        // --------------------------------

        //  -->>  Day 9 puzzle input  <<--
        // --------------------------------
        // 1102,34463338,34463338,63,1007,63,34463338,63,1005,63,53,1101,3,0,1000,109,988,209,12,9,1000,209,6,209,3,203,0,1008,1000,1,63,1005,63,65,1008,1000,2,63,1005,63,904,1008,1000,0,63,1005,63,58,4,25,104,0,99,4,0,104,0,99,4,17,104,0,99,0,0,1101,37,0,1013,1101,426,0,1027,1101,36,0,1000,1101,0,606,1023,1102,34,1,1011,1102,1,712,1029,1102,1,27,1007,1101,831,0,1024,1102,32,1,1002,1102,1,1,1021,1101,429,0,1026,1102,1,826,1025,1101,0,717,1028,1102,1,20,1018,1101,0,24,1004,1102,31,1,1009,1101,22,0,1015,1102,38,1,1014,1102,613,1,1022,1102,29,1,1017,1102,0,1,1020,1102,1,21,1008,1102,33,1,1012,1101,0,30,1006,1101,0,28,1016,1102,1,26,1005,1102,35,1,1019,1101,25,0,1003,1102,1,23,1001,1102,1,39,1010,109,-3,2102,1,5,63,1008,63,34,63,1005,63,205,1001,64,1,64,1106,0,207,4,187,1002,64,2,64,109,-2,1201,7,0,63,1008,63,34,63,1005,63,227,1105,1,233,4,213,1001,64,1,64,1002,64,2,64,109,21,21102,40,1,3,1008,1019,37,63,1005,63,257,1001,64,1,64,1106,0,259,4,239,1002,64,2,64,109,-4,21101,41,0,2,1008,1014,38,63,1005,63,279,1105,1,285,4,265,1001,64,1,64,1002,64,2,64,109,-10,1201,4,0,63,1008,63,30,63,1005,63,307,4,291,1105,1,311,1001,64,1,64,1002,64,2,64,109,6,1207,0,22,63,1005,63,329,4,317,1105,1,333,1001,64,1,64,1002,64,2,64,109,-5,1207,5,20,63,1005,63,353,1001,64,1,64,1106,0,355,4,339,1002,64,2,64,109,8,2108,29,-5,63,1005,63,375,1001,64,1,64,1105,1,377,4,361,1002,64,2,64,109,15,1206,-6,395,4,383,1001,64,1,64,1105,1,395,1002,64,2,64,109,-11,21107,42,43,4,1005,1019,413,4,401,1106,0,417,1001,64,1,64,1002,64,2,64,109,6,2106,0,6,1105,1,435,4,423,1001,64,1,64,1002,64,2,64,109,-15,1208,-3,24,63,1005,63,455,1001,64,1,64,1105,1,457,4,441,1002,64,2,64,109,-13,1208,10,25,63,1005,63,475,4,463,1106,0,479,1001,64,1,64,1002,64,2,64,109,21,21108,43,42,3,1005,1017,495,1106,0,501,4,485,1001,64,1,64,1002,64,2,64,109,-14,2107,31,2,63,1005,63,519,4,507,1106,0,523,1001,64,1,64,1002,64,2,64,109,-4,1202,8,1,63,1008,63,24,63,1005,63,549,4,529,1001,64,1,64,1105,1,549,1002,64,2,64,109,1,2108,23,4,63,1005,63,567,4,555,1105,1,571,1001,64,1,64,1002,64,2,64,109,2,2101,0,5,63,1008,63,21,63,1005,63,591,1105,1,597,4,577,1001,64,1,64,1002,64,2,64,109,28,2105,1,-4,1001,64,1,64,1105,1,615,4,603,1002,64,2,64,109,-10,1205,4,633,4,621,1001,64,1,64,1106,0,633,1002,64,2,64,109,2,1206,2,645,1106,0,651,4,639,1001,64,1,64,1002,64,2,64,109,-4,1202,-6,1,63,1008,63,28,63,1005,63,671,1105,1,677,4,657,1001,64,1,64,1002,64,2,64,109,-9,21102,44,1,4,1008,1010,44,63,1005,63,699,4,683,1105,1,703,1001,64,1,64,1002,64,2,64,109,31,2106,0,-9,4,709,1105,1,721,1001,64,1,64,1002,64,2,64,109,-30,21108,45,45,6,1005,1013,743,4,727,1001,64,1,64,1106,0,743,1002,64,2,64,109,2,21101,46,0,3,1008,1012,46,63,1005,63,765,4,749,1106,0,769,1001,64,1,64,1002,64,2,64,109,-5,2101,0,0,63,1008,63,24,63,1005,63,795,4,775,1001,64,1,64,1105,1,795,1002,64,2,64,109,6,2107,32,-1,63,1005,63,815,1001,64,1,64,1106,0,817,4,801,1002,64,2,64,109,19,2105,1,-5,4,823,1106,0,835,1001,64,1,64,1002,64,2,64,109,-12,21107,47,46,-1,1005,1016,851,1105,1,857,4,841,1001,64,1,64,1002,64,2,64,109,-2,1205,5,873,1001,64,1,64,1105,1,875,4,863,1002,64,2,64,109,-6,2102,1,-8,63,1008,63,23,63,1005,63,897,4,881,1105,1,901,1001,64,1,64,4,64,99,21101,0,27,1,21101,0,915,0,1106,0,922,21201,1,44808,1,204,1,99,109,3,1207,-2,3,63,1005,63,964,21201,-2,-1,1,21101,942,0,0,1105,1,922,21201,1,0,-1,21201,-2,-3,1,21102,957,1,0,1105,1,922,22201,1,-1,-2,1106,0,968,21202,-2,1,-2,109,-3,2105,1,0
        // Expected result for part 1 (input 1): 2941952859
        // Expected result for part 2 (input 2): 66113
        // --------------------------------

        //  -->>  Day 9 assorted examples  <<--
        // -------------------------------------
        // 109,1,204,-1,1001,100,1,100,1008,100,16,101,1006,101,0,99
        // Expected reult: a copy of itself
        //
        // 1102,34915192,34915192,7,4,7,99,0
        // Expected reult: 1219070632396864
        //
        // 104,1125899906842624,99
        // Expected result: 1125899906842624
        // -------------------------------------

        //  -->>  Day 7 puzzle input  <<--
        // --------------------------------
        // 3,8,1001,8,10,8,105,1,0,0,21,34,59,68,89,102,183,264,345,426,99999,3,9,102,5,9,9,1001,9,5,9,4,9,99,3,9,101,3,9,9,1002,9,5,9,101,5,9,9,1002,9,3,9,1001,9,5,9,4,9,99,3,9,101,5,9,9,4,9,99,3,9,102,4,9,9,101,3,9,9,102,5,9,9,101,4,9,9,4,9,99,3,9,1002,9,5,9,1001,9,2,9,4,9,99,3,9,1002,9,2,9,4,9,3,9,101,2,9,9,4,9,3,9,1001,9,2,9,4,9,3,9,101,1,9,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,102,2,9,9,4,9,99,3,9,1001,9,1,9,4,9,3,9,102,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,101,1,9,9,4,9,3,9,101,1,9,9,4,9,3,9,102,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,101,1,9,9,4,9,3,9,1001,9,2,9,4,9,3,9,1001,9,2,9,4,9,99,3,9,1002,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,101,1,9,9,4,9,3,9,1001,9,1,9,4,9,3,9,1002,9,2,9,4,9,3,9,102,2,9,9,4,9,99,3,9,101,1,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,101,1,9,9,4,9,3,9,101,2,9,9,4,9,99,3,9,1001,9,1,9,4,9,3,9,1001,9,2,9,4,9,3,9,101,1,9,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,101,1,9,9,4,9,3,9,1001,9,1,9,4,9,3,9,1001,9,2,9,4,9,3,9,1002,9,2,9,4,9,99
        // Expected result: ???
        // --------------------------------

        //  -->>  Day 5 puzzle input  <<--
        // --------------------------------
        // 3,225,1,225,6,6,1100,1,238,225,104,0,1001,210,88,224,101,-143,224,224,4,224,1002,223,8,223,101,3,224,224,1,223,224,223,101,42,92,224,101,-78,224,224,4,224,1002,223,8,223,1001,224,3,224,1,223,224,223,1101,73,10,225,1102,38,21,225,1102,62,32,225,1,218,61,224,1001,224,-132,224,4,224,102,8,223,223,1001,224,5,224,1,224,223,223,1102,19,36,225,102,79,65,224,101,-4898,224,224,4,224,102,8,223,223,101,4,224,224,1,224,223,223,1101,66,56,224,1001,224,-122,224,4,224,102,8,223,223,1001,224,2,224,1,224,223,223,1002,58,82,224,101,-820,224,224,4,224,1002,223,8,223,101,3,224,224,1,223,224,223,2,206,214,224,1001,224,-648,224,4,224,102,8,223,223,101,3,224,224,1,223,224,223,1102,76,56,224,1001,224,-4256,224,4,224,102,8,223,223,1001,224,6,224,1,223,224,223,1102,37,8,225,1101,82,55,225,1102,76,81,225,1101,10,94,225,4,223,99,0,0,0,677,0,0,0,0,0,0,0,0,0,0,0,1105,0,99999,1105,227,247,1105,1,99999,1005,227,99999,1005,0,256,1105,1,99999,1106,227,99999,1106,0,265,1105,1,99999,1006,0,99999,1006,227,274,1105,1,99999,1105,1,280,1105,1,99999,1,225,225,225,1101,294,0,0,105,1,0,1105,1,99999,1106,0,300,1105,1,99999,1,225,225,225,1101,314,0,0,106,0,0,1105,1,99999,8,226,677,224,102,2,223,223,1005,224,329,101,1,223,223,1008,677,677,224,1002,223,2,223,1006,224,344,1001,223,1,223,107,226,677,224,102,2,223,223,1005,224,359,1001,223,1,223,1108,677,677,224,1002,223,2,223,1006,224,374,101,1,223,223,1107,677,677,224,1002,223,2,223,1006,224,389,101,1,223,223,108,226,677,224,102,2,223,223,1006,224,404,101,1,223,223,7,677,677,224,102,2,223,223,1006,224,419,101,1,223,223,108,677,677,224,102,2,223,223,1006,224,434,1001,223,1,223,7,226,677,224,102,2,223,223,1006,224,449,1001,223,1,223,108,226,226,224,102,2,223,223,1005,224,464,101,1,223,223,8,226,226,224,1002,223,2,223,1006,224,479,101,1,223,223,1008,226,226,224,102,2,223,223,1005,224,494,1001,223,1,223,1008,677,226,224,1002,223,2,223,1005,224,509,101,1,223,223,7,677,226,224,102,2,223,223,1006,224,524,101,1,223,223,1007,677,226,224,1002,223,2,223,1006,224,539,1001,223,1,223,1108,677,226,224,102,2,223,223,1005,224,554,1001,223,1,223,8,677,226,224,1002,223,2,223,1005,224,569,101,1,223,223,1108,226,677,224,1002,223,2,223,1005,224,584,101,1,223,223,1107,677,226,224,102,2,223,223,1006,224,599,101,1,223,223,107,226,226,224,102,2,223,223,1006,224,614,1001,223,1,223,107,677,677,224,1002,223,2,223,1005,224,629,1001,223,1,223,1107,226,677,224,1002,223,2,223,1006,224,644,101,1,223,223,1007,677,677,224,102,2,223,223,1006,224,659,1001,223,1,223,1007,226,226,224,1002,223,2,223,1006,224,674,1001,223,1,223,4,223,99,226
        // Expected result for part 1 (input 1): 7259358 after a bunch of 0:es
        // Expected result for part 2 (input 5): 11826654
        // --------------------------------

        //  -->>  Day 5 assorted examples  <<--
        // -------------------------------------
        // 3,9,8,9,10,9,4,9,99,-1,8
        // using position mode, input is equal to 8 => output 1, else output 0
        //
        // 3,9,7,9,10,9,4,9,99,-1,8
        // using position mode, input is less than 8 => output 1, else output 0
        //
        // 3,3,1108,-1,8,3,4,3,99
        // using immediate mode, input is equal to 8 => output 1, esle output 0
        //
        // 3,3,1107,-1,8,3,4,3,99
        // using immediate mode, input is less than 8 => output 1, esle output 0
        //
        // 3,12,6,12,15,1,13,14,13,4,13,99,-1,0,1,9
        // using position mode, jump tests input non-zero => 1, else 0
        //
        // 3,3,1105,-1,9,1101,0,0,12,4,12,99,1
        // using immediate mode, jump tests input non-zero => 1, else 0
        //
        // 3,21,1008,21,8,20,1005,20,22,107,8,21,20,1006,20,31,1106,0,36,98,0,0,1002,21,125,20,4,20,1105,1,46,104,999,1105,1,46,1101,1000,1,20,4,20,1105,1,46,98,99
        // inputs a single number. The program will then output:
        //  - 999 if the input value is below 8
        //  - 1000 if the input value is equal to 8
        //  - 1001 if the input value is greater than 8
        // ---------------------------------------------

        //  -->>  Day 2 puzzle input  <<--
        // --------------------------------
        // 1,12,2,3,1,1,2,3,1,3,4,3,1,5,0,3,2,6,1,19,2,19,9,23,1,23,5,27,2,6,27,31,1,31,5,35,1,35,5,39,2,39,6,43,2,43,10,47,1,47,6,51,1,51,6,55,2,55,6,59,1,10,59,63,1,5,63,67,2,10,67,71,1,6,71,75,1,5,75,79,1,10,79,83,2,83,10,87,1,87,9,91,1,91,10,95,2,6,95,99,1,5,99,103,1,103,13,107,1,107,10,111,2,9,111,115,1,115,6,119,2,13,119,123,1,123,6,127,1,5,127,131,2,6,131,135,2,6,135,139,1,139,5,143,1,143,10,147,1,147,2,151,1,151,13,0,99,2,0,14,0
        // Expected result: memory[0] = 4576384
        // --------------------------------
        ],
        program
    );
    int_program.execute();
    let mut min_x = 1000000;
    let mut min_y = 1000000;
    let mut max_x = 0;
    let mut max_y = 0;

    let mut map_copy = HashMap::new();
    for ((x, y), value) in int_program.io.map
    {
        if x < min_x
        {
            min_x = x;
        }
        if y < min_y
        {
            min_y = y;
        }
        if x > max_x
        {
            max_x = x;
        }
        if y > max_y
        {
            max_y = y;
        }

        map_copy.insert((x, y), value);
    }

    max_x += 1;
    max_y += 1;

    for y in min_y..max_y
    {
        for x in min_x..max_x
        {
            let data = map_copy.get(&(x, y));
            let pr_chr = match data
            {
                Some(data) => match data { 0 => ' ', 1 => '#', _ => panic!() },
                None => ' '
            };
            print!("{}", pr_chr);
            //for ((x, y), _) in int_program.io.map
            //println!("({}, {}) ({}, {})", min_x, min_y, max_x, max_y);
        }
        println!();
    }
}

impl Program
{
    fn new() -> Self
    {
        let mut map = HashMap::new();
        map.insert((0, 0), 1);

        let program = Program {
            position: (0, 0),
            direction: Direction::North,
            map: map,
            next_data_point_is_paint_instruction: true
        };
        return program;
    }
}

impl IOHandler for Program
{
    fn input_handler(& mut self) -> IOData
    {
        return match self.map.get(&self.position)
        {
            Some(value) => *value,
            None => 0 as IOData
        };
    }

    fn output_handler(& mut self, data: IOData)
    {
        if self.next_data_point_is_paint_instruction
        {
            if data < 0 || data > 1
            {
                panic!();
            }

            *self.map.entry(self.position).or_insert(0) = data;
            self.next_data_point_is_paint_instruction = false;
        }
        else
        {
            self.direction = match data
            {
                0 => match self.direction
                {
                    Direction::North => Direction::West,
                    Direction::West => Direction::South,
                    Direction::South => Direction::East,
                    Direction::East => Direction::North
                },
                1 => match self.direction
                {
                    Direction::North => Direction::East,
                    Direction::East => Direction::South,
                    Direction::South => Direction::West,
                    Direction::West => Direction::North
                },
                _ => panic!()
            };

            let x = self.position.0;
            let y = self.position.1;
            self.position = match self.direction
            {
                Direction::North => (x, y-1),
                Direction::East => (x+1, y),
                Direction::South => (x, y+1),
                Direction::West => (x-1, y)
            };

            self.next_data_point_is_paint_instruction = true;
        }
    }
}

impl<T: IOHandler> IntProgram<T>
{
    fn new(instructions: Vec<InstructionByte>, io: T) -> Self
    {
        let mut program = IntProgram
        {
            memory: HashMap::<Address,InstructionByte>::new(),
            relative_base: 0,
            io: io,
        };

        let mut addr: Address = 0;
        for instr in instructions
        {
            program.memory.insert(addr, instr);
            addr += 1;
        }

        return program;
    }

    fn execute(& mut self)
    {
        let mut instr_ptr: Address = 0;
        let mut instr_ptr_wrapped = Some(instr_ptr);

        while instr_ptr_wrapped != None
        {
            instr_ptr = instr_ptr_wrapped.unwrap();
            let instruction = self.decode_opcode(self.read_memory(instr_ptr));

            instr_ptr_wrapped = match instruction
            {
                Instruction::Add(p1_mode, p2_mode, p3_mode) => self.add(instr_ptr, p1_mode, p2_mode, p3_mode),
                Instruction::Multiply(p1_mode, p2_mode, p3_mode) => self.multiply(instr_ptr, p1_mode, p2_mode, p3_mode),
                Instruction::Input(p1_mode) => self.input(instr_ptr, p1_mode),
                Instruction::Output(p1_mode) => self.output(instr_ptr, p1_mode),
                Instruction::JumpIfTrue(p1_mode, p2_mode) => self.jump_if_true(instr_ptr, p1_mode, p2_mode),
                Instruction::JumpIfFalse(p1_mode, p2_mode) => self.jump_if_false(instr_ptr, p1_mode, p2_mode),
                Instruction::LessThan(p1_mode, p2_mode, p3_mode) => self.less_than(instr_ptr, p1_mode, p2_mode, p3_mode),
                Instruction::Equals(p1_mode, p2_mode, p3_mode) => self.equals(instr_ptr, p1_mode, p2_mode, p3_mode),
                Instruction::AdjustRelativeBase(p1_mode) => self.adjust_relative_base(instr_ptr, p1_mode),
                Instruction::Exit => self.exit()
            };
        }
    }


// ----------------------------------------- //
//  -->>  Instruction implementations  <<--  //
// ----------------------------------------- //

    fn add(& mut self, instr_ptr: Address, p1_mode: ParameterMode, p2_mode: ParameterMode, p3_mode: ParameterMode) -> Option<Address>
    {
        self.set_value(
            self.read_memory(instr_ptr + 3 as Address),
            self.get_value(instr_ptr+1, p1_mode) +
            self.get_value(instr_ptr+2, p2_mode),
            p3_mode
        );

        return Some(instr_ptr + 4);
    }

    fn multiply(& mut self, instr_ptr: Address, p1_mode: ParameterMode, p2_mode: ParameterMode, p3_mode: ParameterMode) -> Option<Address>
    {
        self.set_value(
            self.read_memory(instr_ptr + 3 as Address),
            self.get_value(instr_ptr+1, p1_mode) *
            self.get_value(instr_ptr+2, p2_mode),
            p3_mode
        );

        return Some(instr_ptr + 4);
    }

    fn input(& mut self, instr_ptr: Address, p1_mode: ParameterMode) -> Option<Address>
    {
        let data = self.io.input_handler();

        self.set_value(
            self.read_memory(instr_ptr + 1),
            data,
            p1_mode
        );

        return Some(instr_ptr + 2);
    }

    fn output(& mut self, instr_ptr: Address, p1_mode: ParameterMode) -> Option<Address>
    {
        let value = self.get_value(instr_ptr+1, p1_mode);
        self.io.output_handler(value);
        return Some(instr_ptr + 2);
    }

    fn jump_if_true(& mut self, instr_ptr: Address, p1_mode: ParameterMode, p2_mode: ParameterMode) -> Option<Address>
    {
        let compare_value = self.get_value(instr_ptr+1, p1_mode);

        if compare_value == 0
        {
            return Some(instr_ptr + 3)
        }

        let target_address = self.get_value(instr_ptr+2, p2_mode) as Address;
        return Some(target_address);
    }

    fn jump_if_false(& mut self, instr_ptr: Address, p1_mode: ParameterMode, p2_mode: ParameterMode) -> Option<Address>
    {
        let compare_value = self.get_value(instr_ptr+1, p1_mode);

        if compare_value != 0
        {
            return Some(instr_ptr + 3)
        }

        let target_address = self.get_value(instr_ptr+2, p2_mode) as Address;
        return Some(target_address);
    }

    fn less_than(& mut self, instr_ptr: Address, p1_mode: ParameterMode, p2_mode: ParameterMode, p3_mode: ParameterMode) -> Option<Address>
    {
        self.set_value(
            self.read_memory(instr_ptr + 3),
            match
                self.get_value(instr_ptr+1, p1_mode) <
                self.get_value(instr_ptr+2, p2_mode)
            {
                true => 1,
                false => 0
            },
            p3_mode);

        return Some(instr_ptr + 4);
    }

    fn equals(& mut self, instr_ptr: Address, p1_mode: ParameterMode, p2_mode: ParameterMode, p3_mode: ParameterMode) -> Option<Address>
    {
        self.set_value(
            self.read_memory(instr_ptr + 3),
            match
                self.get_value(instr_ptr+1, p1_mode) ==
                self.get_value(instr_ptr+2, p2_mode)
            {
                true => 1,
                false => 0
            },
            p3_mode);

        return Some(instr_ptr + 4);
    }

    fn adjust_relative_base(& mut self, instr_ptr: Address, p1_mode: ParameterMode) -> Option<Address>
    {
        self.relative_base += self.get_value(instr_ptr + 1, p1_mode);
        return Some(instr_ptr + 2);
    }

    fn exit(&self) -> Option<Address>
    {
        return None;
    }


// ------------------------------------------------- //
//  -->>  Instruction handling infrastructure  <<--  //
// ------------------------------------------------- //

    fn decode_opcode(&self, opcode: InstructionByte) -> Instruction
    {
        let full_opcode_str = format!("{:05}", opcode);
        let opcode_str = full_opcode_str.get(3..5).unwrap();

        let mode_1 = self.decode_parameter_mode(
            full_opcode_str.get(2..3)
            .unwrap());

        let mode_2 = self.decode_parameter_mode(
            full_opcode_str.get(1..2)
            .unwrap());

        let mode_3 = self.decode_parameter_mode(
            full_opcode_str.get(0..1)
            .unwrap());

        return match opcode_str
        {
            "01" => Instruction::Add(mode_1, mode_2, mode_3),
            "02" => Instruction::Multiply(mode_1, mode_2, mode_3),
            "03" => Instruction::Input(mode_1),
            "04" => Instruction::Output(mode_1),
            "05" => Instruction::JumpIfTrue(mode_1, mode_2),
            "06" => Instruction::JumpIfFalse(mode_1, mode_2),
            "07" => Instruction::LessThan(mode_1, mode_2, mode_3),
            "08" => Instruction::Equals(mode_1, mode_2, mode_3),
            "09" => Instruction::AdjustRelativeBase(mode_1),
            "99" => Instruction::Exit,
            _ => panic!("Invalid instruction {}", opcode_str)
        };
    }

    fn read_memory(&self, address: Address) -> InstructionByte
    {
        return match self.memory.get(&address)
        {
            Some(value) => *value,
            None => 0 as InstructionByte
        };
    }

    fn get_value(&self, address: Address, parameter_mode: ParameterMode) -> InstructionByte
    {
        let value: InstructionByte = self.read_memory(address);

        return match parameter_mode
        {
            ParameterMode::Immediate => value,
            ParameterMode::Positional => self.read_memory(value),
            ParameterMode::Relative => self.read_memory(self.relative_base + value)
        };
    }

    fn set_value(& mut self, address: Address, value: InstructionByte, parameter_mode: ParameterMode)
    {
        let target_address = match parameter_mode
        {
            ParameterMode::Immediate => panic!("Cannot write data in immediate mode!"),
            ParameterMode::Positional => address,
            ParameterMode::Relative => (self.relative_base + address)
        };

        *self.memory.entry(target_address).or_insert(1) = value;
    }

    fn decode_parameter_mode(&self, input: &str) -> ParameterMode
    {
        let mode = match input
        {
            "0" => ParameterMode::Positional,
            "1" => ParameterMode::Immediate,
            "2" => ParameterMode::Relative,
            _ => panic!("Invalid paramter mode!")
        };

        return mode;
    }
}
