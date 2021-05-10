use std::vec::Vec;

type Address = usize;
type InstructionByte = i32;

#[derive(Debug)]
enum ParameterMode
{
    Positional,
    Immediate
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
    Equals(ParameterMode, ParameterMode, ParameterMode)
}

// ------------------ //
//  -->>  Main  <<--  //
// ------------------ //

fn main()
{
    let mut program = Program::new(vec![

        //  -->>  Day 2 puzzle input  <<--
        // --------------------------------
        // 1,12,2,3,1,1,2,3,1,3,4,3,1,5,0,3,2,6,1,19,2,19,9,23,1,23,5,27,2,6,27,31,1,31,5,35,1,35,5,39,2,39,6,43,2,43,10,47,1,47,6,51,1,51,6,55,2,55,6,59,1,10,59,63,1,5,63,67,2,10,67,71,1,6,71,75,1,5,75,79,1,10,79,83,2,83,10,87,1,87,9,91,1,91,10,95,2,6,95,99,1,5,99,103,1,103,13,107,1,107,10,111,2,9,111,115,1,115,6,119,2,13,119,123,1,123,6,127,1,5,127,131,2,6,131,135,2,6,135,139,1,139,5,143,1,143,10,147,1,147,2,151,1,151,13,0,99,2,0,14,0
        // Expected result: memory[0] = 4576384
        // ------------------------------

        //  -->>  Day 5 puzzle input  <<--
        // --------------------------------
           3,225,1,225,6,6,1100,1,238,225,104,0,1001,210,88,224,101,-143,224,224,4,224,1002,223,8,223,101,3,224,224,1,223,224,223,101,42,92,224,101,-78,224,224,4,224,1002,223,8,223,1001,224,3,224,1,223,224,223,1101,73,10,225,1102,38,21,225,1102,62,32,225,1,218,61,224,1001,224,-132,224,4,224,102,8,223,223,1001,224,5,224,1,224,223,223,1102,19,36,225,102,79,65,224,101,-4898,224,224,4,224,102,8,223,223,101,4,224,224,1,224,223,223,1101,66,56,224,1001,224,-122,224,4,224,102,8,223,223,1001,224,2,224,1,224,223,223,1002,58,82,224,101,-820,224,224,4,224,1002,223,8,223,101,3,224,224,1,223,224,223,2,206,214,224,1001,224,-648,224,4,224,102,8,223,223,101,3,224,224,1,223,224,223,1102,76,56,224,1001,224,-4256,224,4,224,102,8,223,223,1001,224,6,224,1,223,224,223,1102,37,8,225,1101,82,55,225,1102,76,81,225,1101,10,94,225,4,223,99,0,0,0,677,0,0,0,0,0,0,0,0,0,0,0,1105,0,99999,1105,227,247,1105,1,99999,1005,227,99999,1005,0,256,1105,1,99999,1106,227,99999,1106,0,265,1105,1,99999,1006,0,99999,1006,227,274,1105,1,99999,1105,1,280,1105,1,99999,1,225,225,225,1101,294,0,0,105,1,0,1105,1,99999,1106,0,300,1105,1,99999,1,225,225,225,1101,314,0,0,106,0,0,1105,1,99999,8,226,677,224,102,2,223,223,1005,224,329,101,1,223,223,1008,677,677,224,1002,223,2,223,1006,224,344,1001,223,1,223,107,226,677,224,102,2,223,223,1005,224,359,1001,223,1,223,1108,677,677,224,1002,223,2,223,1006,224,374,101,1,223,223,1107,677,677,224,1002,223,2,223,1006,224,389,101,1,223,223,108,226,677,224,102,2,223,223,1006,224,404,101,1,223,223,7,677,677,224,102,2,223,223,1006,224,419,101,1,223,223,108,677,677,224,102,2,223,223,1006,224,434,1001,223,1,223,7,226,677,224,102,2,223,223,1006,224,449,1001,223,1,223,108,226,226,224,102,2,223,223,1005,224,464,101,1,223,223,8,226,226,224,1002,223,2,223,1006,224,479,101,1,223,223,1008,226,226,224,102,2,223,223,1005,224,494,1001,223,1,223,1008,677,226,224,1002,223,2,223,1005,224,509,101,1,223,223,7,677,226,224,102,2,223,223,1006,224,524,101,1,223,223,1007,677,226,224,1002,223,2,223,1006,224,539,1001,223,1,223,1108,677,226,224,102,2,223,223,1005,224,554,1001,223,1,223,8,677,226,224,1002,223,2,223,1005,224,569,101,1,223,223,1108,226,677,224,1002,223,2,223,1005,224,584,101,1,223,223,1107,677,226,224,102,2,223,223,1006,224,599,101,1,223,223,107,226,226,224,102,2,223,223,1006,224,614,1001,223,1,223,107,677,677,224,1002,223,2,223,1005,224,629,1001,223,1,223,1107,226,677,224,1002,223,2,223,1006,224,644,101,1,223,223,1007,677,677,224,102,2,223,223,1006,224,659,1001,223,1,223,1007,226,226,224,1002,223,2,223,1006,224,674,1001,223,1,223,4,223,99,226
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
    ]);
    program.execute();
    //println!("mem[0]: {}", program.memory[0]);
}

struct Program
{
    memory: Vec<InstructionByte>
}

impl Program
{
    fn new(instructions: Vec<InstructionByte>) -> Self
    {
        let mut program = Program
        {
            memory: Vec::<InstructionByte>::new()
        };

        for ins in instructions
        {
            program.memory.push(ins);
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
            let instruction = self.decode_opcode(self.memory[instr_ptr]);

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
                Instruction::Exit => self.exit()
            };
        }
    }


// ----------------------------------------- //
//  -->>  Instruction implementations  <<--  //
// ----------------------------------------- //

    fn add(& mut self, instr_ptr: Address, p1_mode: ParameterMode, p2_mode: ParameterMode, p3_mode: ParameterMode) -> Option<Address>
    {
        if matches!(p3_mode, ParameterMode::Immediate)
        {
            panic!("Third parameter to add() must be positional!");
        }

        let target_address: Address = self.memory[instr_ptr + 3] as Address;

        self.memory[target_address] =
            self.get_value(instr_ptr+1, p1_mode) +
            self.get_value(instr_ptr+2, p2_mode);

        return Some(instr_ptr + 4);
    }

    fn multiply(& mut self, instr_ptr: Address, p1_mode: ParameterMode, p2_mode: ParameterMode, p3_mode: ParameterMode) -> Option<Address>
    {
        if matches!(p3_mode, ParameterMode::Immediate)
        {
            panic!("Third parameter to multiply() must be positional!");
        }

        let target_address: Address = self.memory[instr_ptr + 3] as Address;

        self.memory[target_address] =
            self.get_value(instr_ptr+1, p1_mode) *
            self.get_value(instr_ptr+2, p2_mode);

        return Some(instr_ptr + 4);
    }

    fn input(& mut self, instr_ptr: Address, p1_mode: ParameterMode) -> Option<Address>
    {
        if matches!(p1_mode, ParameterMode::Immediate)
        {
            panic!("First parameter to input() must be positional!");
        }

        let mut line = String::new();
        let _ = std::io::stdin()
            .read_line(&mut line)
            .expect("Failed to read from stdin!");

        let input = line.trim()
            .parse::<InstructionByte>()
            .expect("Provided input is not an integer!");

        let arg_pos: Address = self.memory[instr_ptr + 1] as Address;
        self.memory[arg_pos] = input;

        return Some(instr_ptr + 2);
    }

    fn output(& mut self, instr_ptr: Address, p1_mode: ParameterMode) -> Option<Address>
    {
        let value = self.get_value(instr_ptr+1, p1_mode);

        println!("{}", value);
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
        if matches!(p3_mode, ParameterMode::Immediate)
        {
            panic!("Third parameter to less_than() must be positional!");
        }

        let target_address: Address = self.memory[instr_ptr + 3] as Address;

        self.memory[target_address] = match
            self.get_value(instr_ptr+1, p1_mode) <
            self.get_value(instr_ptr+2, p2_mode)
        {
            true => 1,
            false => 0
        };

        return Some(instr_ptr + 4);
    }

    fn equals(& mut self, instr_ptr: Address, p1_mode: ParameterMode, p2_mode: ParameterMode, p3_mode: ParameterMode) -> Option<Address>
    {
        if matches!(p3_mode, ParameterMode::Immediate)
        {
            panic!("Third parameter to equals() must be positional!");
        }

        let target_address: Address = self.memory[instr_ptr + 3] as Address;

        self.memory[target_address] = match
            self.get_value(instr_ptr+1, p1_mode) ==
            self.get_value(instr_ptr+2, p2_mode)
        {
            true => 1,
            false => 0
        };

        return Some(instr_ptr + 4);
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
            "99" => Instruction::Exit,
            _ => panic!("Invalid instruction {}", opcode_str)
        };
    }

    fn get_value(&self, address: Address, parameter_mode: ParameterMode) -> InstructionByte
    {
        let byte_data: InstructionByte = self.memory[address];

        return match parameter_mode
        {
            ParameterMode::Immediate => byte_data,
            ParameterMode::Positional => self.memory[byte_data as Address] as InstructionByte
        };
    }

    fn decode_parameter_mode(&self, input: &str) -> ParameterMode
    {
        let mode = match input
        {
            "0" => ParameterMode::Positional,
            "1" => ParameterMode::Immediate,
            _ => panic!("Invalid paramter mode!")
        };

        return mode;
    }
}
