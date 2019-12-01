package main

import (
    "bufio"
    "fmt"
    "log"
    "os"
    "strconv"
    "strings"
)

type Parameter struct {
    IsNumeric bool
    NumericValue int
    StringValue string
}

type Instruction struct {
    Name string
    Parameter1 *Parameter
    Parameter2 *Parameter
}

func main() {
    instructions, registers := GetInput()

    fmt.Println("Instructions:")
    for _, instr := range instructions {
        PrintInstruction(&instr)
    }

    fmt.Println()
    fmt.Println("Registers:")
    for key, value := range registers {
        fmt.Printf("%s: %d\n", key, value)
    }

    fmt.Println()
    fmt.Println("Executing...")
    Execute(instructions, registers)
}

func Execute(instructions []Instruction, registers map[string]int) {
    idx := 0
    count := 0
    max := 100000
    mulCounter := 0

    for {
        if idx >= len(instructions) {
            fmt.Println("Last instruction reached, quitting")
            break
        }

        if count > max {
            fmt.Println("Max instruction count reached, quitting")
            break
        }

        instr := instructions[idx]
        p1 := instr.Parameter1
        p2 := instr.Parameter2

        reg1 := ""
        if !p1.IsNumeric {
            reg1 = p1.StringValue
        }

        reg2 := ""
        if p2 != nil && !p2.IsNumeric {
            reg2 = p2.StringValue
        }

        base := 0
        if reg1 != "" {
            base = registers[reg1]
        }

        switch instr.Name {
        case "set":
            if p2.IsNumeric {
                registers[reg1] = p2.NumericValue
            } else {
                registers[reg1] = registers[reg2]
            }
            fmt.Printf("SET: Set register %s to %d\n", reg1, registers[reg1])

        case "sub":
            if p2.IsNumeric {
                registers[reg1] = base - p2.NumericValue
            } else {
                registers[reg1] = base - registers[reg2]
            }
            fmt.Printf("SUB: Stored subtraction result %d in register %s\n", registers[reg1], reg1)

        case "mul":
            if p2.IsNumeric {
                registers[reg1] = base * p2.NumericValue
            } else {
                registers[reg1] = base * registers[reg2]
            }
            fmt.Printf("MUL: Stored multiplication result %d in register %s\n", registers[reg1], reg1)
            mulCounter++

        case "jnz":
            offset := -1
            condition := 0

            if p1.IsNumeric {
                condition = p1.NumericValue
            } else if !p1.IsNumeric {
                condition = registers[reg1]
            }

            if p2.IsNumeric {
                offset += p2.NumericValue
            } else {
                offset += registers[reg2]
            }

            if (condition != 0) {
                oldIdx := idx
                idx += offset
                fmt.Printf("JGZ: Next instruction will be %d instead of %d, condition %d was met\n", idx+1, oldIdx+1, condition)
            } else {
                fmt.Printf("JGZ: No jump since condition %d was not met\n", condition)
            }

        default:
            log.Fatal("Invalid instruction: " + instr.Name)
        }

        idx++
        count++
    }

    fmt.Printf("Number of MUL instructions performed: %d", mulCounter)
}

func GetInput() ([]Instruction, map[string]int) {
    file, err := os.Open("Day23.txt")
    if (err != nil) {
        log.Fatal("Could not open input file.")
    }
    defer file.Close()

    scanner := bufio.NewScanner(file)
    instructions := []Instruction {}
    registers := map[string]int {}

    for scanner.Scan() {
        raw := scanner.Text()
        split := strings.Split(raw, " ")
        if len(split) < 2 { log.Fatal("At least one parameter required for any instruction: " + raw) }

        instr := Instruction { Name: split[0] }

        param1 := CreateParamter(split[1])
        instr.Parameter1 = param1

        if !param1.IsNumeric {
            if _, ok := registers[param1.StringValue]; !ok {
                registers[param1.StringValue] = 0
            }
        }

        if len(split) > 2 {
            param2 := CreateParamter(split[2])
            instr.Parameter2 = param2
        }

        instructions = append(instructions, instr)
    }

    return instructions, registers
}

func CreateParamter(input string) *Parameter {
    param := &Parameter { }

    if (input[0] >= '0' && input[0] <= '9') || input[0] == '-' {
        param.IsNumeric = true
        val, err := strconv.Atoi(input)
        if err != nil {
            log.Fatal("Invalid parameter: " + input)
        }
        param.NumericValue = val
    } else {
        param.StringValue = input
    }

    return param
}

func PrintInstruction(instr *Instruction) {
    fmt.Printf("%s ", instr.Name)
    param := instr.Parameter1
    if param.IsNumeric {
        fmt.Printf("%d <numeric> ", param.NumericValue)
    } else {
        fmt.Printf("%s <string> ", param.StringValue)
    }

    if instr.Parameter2 != nil {
        param := instr.Parameter2
        if param.IsNumeric {
            fmt.Printf(", %d <numeric> ", param.NumericValue)
        } else {
            fmt.Printf(", %s <string> ", param.StringValue)
        }    
    }

    fmt.Println()
}
