use std::vec::Vec;
type Instruction = i32;
type Address = usize;

fn main()
{
    let mut program = Program::new(vec![1,12,2,3,1,1,2,3,1,3,4,3,1,5,0,3,2,6,1,19,2,19,9,23,1,23,5,27,2,6,27,31,1,31,5,35,1,35,5,39,2,39,6,43,2,43,10,47,1,47,6,51,1,51,6,55,2,55,6,59,1,10,59,63,1,5,63,67,2,10,67,71,1,6,71,75,1,5,75,79,1,10,79,83,2,83,10,87,1,87,9,91,1,91,10,95,2,6,95,99,1,5,99,103,1,103,13,107,1,107,10,111,2,9,111,115,1,115,6,119,2,13,119,123,1,123,6,127,1,5,127,131,2,6,131,135,2,6,135,139,1,139,5,143,1,143,10,147,1,147,2,151,1,151,13,0,99,2,0,14,0]);
    program.execute();
    println!("{}", program.memory[0]);
}

struct Program
{
    memory: Vec<Instruction>
}

impl Program
{
    fn new(instructions: Vec<Instruction>) -> Self
    {
        let mut program = Program
        {
            memory: Vec::<Instruction>::new()
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
            instr_ptr_wrapped = match self.memory[instr_ptr]
            {
                1 => self.add(instr_ptr),
                2 => self.multiply(instr_ptr),
                99 => self.exit(),
                _ => panic!("Invalid instruction @ {}: {}", instr_ptr, self.memory[instr_ptr])
            };
        }
    }

    fn add(& mut self, instr_ptr: Address) -> Option<Address>
    {
        let arg_pos_1:  Address = self.memory[instr_ptr + 1] as Address;
        let arg_pos_2:  Address = self.memory[instr_ptr + 2] as Address;
        let result_pos: Address = self.memory[instr_ptr + 3] as Address;

        self.memory[result_pos] =
            self.memory[arg_pos_1] +
            self.memory[arg_pos_2];

        return Some(instr_ptr + 4);
    }

    fn multiply(& mut self, instr_ptr: Address) -> Option<Address>
    {
        let arg_pos_1:  Address = self.memory[instr_ptr + 1] as Address;
        let arg_pos_2:  Address = self.memory[instr_ptr + 2] as Address;
        let result_pos: Address = self.memory[instr_ptr + 3] as Address;

        self.memory[result_pos] =
            self.memory[arg_pos_1] *
            self.memory[arg_pos_2];

        return Some(instr_ptr + 4);
    }

    fn exit(&self) -> Option<Address>
    {
        return None;
    }
}
