package main

import (
    "bufio"
    "fmt"
    "log"
    "math"
    "os"
    "strconv"
    "strings"
)

type tInt int64

type Machine struct {
    Instructions []Instruction
    Register map[string]tInt
    NextInstructionIndex int
    Id int
    IsWaiting bool
    WaitingRegister string
    MessageQueue Queue
    SendCounter int
}

type Parameter struct {
    IsNumeric bool
    NumericValue tInt
    StringValue string
}

type Instruction struct {
    Name string
    Parameter1 *Parameter
    Parameter2 *Parameter
}

type Queue struct {
    Data []tInt
}

// ExecutionResults
const (
    EndOfCode = iota
    InvalidInstruction
    MaxInstructionCountReached
    WaitingForSignal
)

func main() {
    machine := GetInput()

    fmt.Println("Code:")
    for _, instr := range machine.Instructions {
        PrintInstruction(&instr)
    }

    fmt.Println()
    fmt.Println("Register:")
    for key, value := range machine.Register {
        fmt.Printf("%s: %d\n", key, value)
    }

    machines := []*Machine { machine }
    machines = append(machines, Clone(machine, 1))

    mid := 0
    oid := 1

    prevPrevCount := 0
    prevCount := 0

    for {
        fmt.Println()
        fmt.Printf("Executing machine [%d]...\n", mid)
        count, result := Execute(machines[mid], &(machines[oid].MessageQueue))
        fmt.Printf("Execution results [%d]: count = %d, result = %d\n", mid, count, result)

        if (count == 0 && prevCount == 0 && prevPrevCount == 0) {
            fmt.Println()
            fmt.Println("Both machines is waiting, cannot continue execution.")
            break
        }

        tmp := mid
        mid = oid
        oid = tmp

        prevPrevCount = prevCount
        prevCount = count
    }

    fmt.Printf("Machine [1] sent data %d times in total.\n", machines[1].SendCounter)
}

func Execute(machine *Machine, messageQueue *Queue) (int, int) {
    if machine.IsWaiting {
        if len(machine.MessageQueue.Data) == 0 {
            fmt.Printf("[%d] Still waiting...\n", machine.Id)
            return 0, WaitingForSignal
        }

        data := machine.MessageQueue.Data[0]
        machine.MessageQueue.Data = machine.MessageQueue.Data[1:]
        machine.Register[machine.WaitingRegister] = data
        fmt.Printf("[%d] RCV: Received %d, storing in register %s\n", machine.Id, data, machine.WaitingRegister)
    }

    count := 0
    max := 100000
    register := &(machine.Register)
    machine.IsWaiting = false

    for {
        if machine.NextInstructionIndex >= len(machine.Instructions) {
            PrintMachinePrefix(machine)
            fmt.Printf("HALT: EOC reached\n")
            return count, EndOfCode
        }

        if count > max {
            PrintMachinePrefix(machine)            
            fmt.Printf("HALT: max instruction count reached\n")
            return count, MaxInstructionCountReached
        }

        instr := machine.Instructions[machine.NextInstructionIndex]
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

        base := tInt(0)
        if reg1 != "" {
            base = machine.Register[reg1]
        }

        switch instr.Name {
        case "snd":
            val := tInt(0)
            if p1.IsNumeric {
                val = p1.NumericValue
            } else {
                val = (*register)[reg1]
            }
            messageQueue.Data = append(messageQueue.Data, val)
            PrintMachinePrefix(machine)
            fmt.Printf("SND: Sending %d to other machine\n", val)
            machine.SendCounter++

        case "set":
            if p2.IsNumeric {
                (*register)[reg1] = p2.NumericValue
            } else {
                (*register)[reg1] = (*register)[reg2]
            }
            PrintMachinePrefix(machine)
            fmt.Printf("SET: Set register %s to %d\n", reg1, (*register)[reg1])

        case "add":
            if p2.IsNumeric {
                (*register)[reg1] = Add(base, p2.NumericValue)
            } else {
                (*register)[reg1] = Add(base, (*register)[reg2])
            }
            PrintMachinePrefix(machine)
            fmt.Printf("ADD: Stored addition result %d in register %s\n", (*register)[reg1], reg1)

        case "mul":
            if p2.IsNumeric {
                (*register)[reg1] = Mul(base, p2.NumericValue)
            } else {
                (*register)[reg1] = Mul(base, (*register)[reg2])
            }
            PrintMachinePrefix(machine)
            fmt.Printf("MUL: Stored multiplication result %d in register %s\n", (*register)[reg1], reg1)

        case "mod":
            if p2.IsNumeric {
                (*register)[reg1] = base % p2.NumericValue
            } else {
                (*register)[reg1] = base % (*register)[reg2]
            }
            PrintMachinePrefix(machine)
            fmt.Printf("MOD: Stored modulus result %d in register %s\n", (*register)[reg1], reg1)

        case "rcv":
            if len(machine.MessageQueue.Data) < 1 {
                PrintMachinePrefix(machine)
                fmt.Printf("RCV: Message queue empty\n")
                PrintMachinePrefix(machine)
                fmt.Printf("HALT: Waiting for signal\n")

                machine.IsWaiting = true
                machine.WaitingRegister = reg1
                machine.NextInstructionIndex++
                return count, WaitingForSignal
            }

            data := machine.MessageQueue.Data[0]
            machine.MessageQueue.Data = machine.MessageQueue.Data[1:]
            machine.Register[reg1] = data
            PrintMachinePrefix(machine)
            fmt.Printf("RCV: Received %d, storing in register %s\n", data, reg1)

        case "jgz":
            offset := -1
            condition := tInt(0)

            if p1.IsNumeric {
                condition = p1.NumericValue
            } else if !p1.IsNumeric {
                condition = (*register)[reg1]
            }

            if (condition > 0) {
                if p2.IsNumeric {
                    offset += int(p2.NumericValue)
                } else {
                    offset += int((*register)[reg2])
                }

                PrintMachinePrefix(machine)
                oldIdx := machine.NextInstructionIndex
                machine.NextInstructionIndex += offset
                fmt.Printf("JGZ: Next instruction will be %d instead of %d, condition %d was met\n", machine.NextInstructionIndex+1, oldIdx+1, condition)
            } else {
                PrintMachinePrefix(machine)
                fmt.Printf("JGZ: No jump since condition %d was not met\n", condition)
            }

        default:
            PrintMachinePrefix(machine)            
            fmt.Printf("HALT: Invalid instruction\n")
            return count, InvalidInstruction
        }

        machine.NextInstructionIndex++
        count++
    }
}

func Add(a tInt, b tInt) tInt {
    ret := a + b

    if a > 0 && b > 0 {
        if ret < a || ret < b {
            log.Fatal(fmt.Sprintf("Addition overflowed: %d + %d = %d", a, b, ret))
        }
    } else if a < 0 && b < 0 {
        if ret > a || ret > b {
            log.Fatal(fmt.Sprintf("Addition overflowed: %d + %d = %d", a, b, ret))
        }
    } else if a < 0 && b > 0 {
        if ret > b || ret < a {
            log.Fatal(fmt.Sprintf("Addition overflowed: %d + %d = %d", a, b, ret))
        }
    } else if a > 0 && b < 0 {
        if ret < b || ret > a {
            log.Fatal(fmt.Sprintf("Addition overflowed: %d + %d = %d", a, b, ret))
        }
    }

    return ret
}

func Mul(a tInt, b tInt) tInt {
    ret := a * b

    if a > 0 && b > 0 {
        if ret < a || ret < b {
            log.Fatal(fmt.Sprintf("Multiplication overflowed: %d * %d = %d", a, b, ret))
        }
    } else if a < 0 && b < 0 {
        if float64(ret) > math.Abs(float64(a)) || float64(ret) > math.Abs(float64(b)) {
            log.Fatal(fmt.Sprintf("Multiplication overflowed: %d * %d = %d", a, b, ret))
        }
    }

    return ret
}

func PrintMachinePrefix(machine *Machine) {
    fmt.Printf("[%d, %d] ", machine.Id, machine.NextInstructionIndex)
}

func Clone(machine *Machine, newId int) *Machine {
    clone := CreateMachine(newId)

    for key, value := range machine.Register {
        clone.Register[key] = value
    }

    for _, instr := range machine.Instructions {
        clone.Instructions = append(clone.Instructions, instr)
    }

    clone.Register["p"] = tInt(newId)

    return clone
}

func GetInput() *Machine {
    file, err := os.Open("Day18.txt")
    if (err != nil) {
        log.Fatal("Could not open input file.")
    }
    defer file.Close()

    scanner := bufio.NewScanner(file)

    machine := CreateMachine(0)

    for scanner.Scan() {
        raw := scanner.Text()
        split := strings.Split(raw, " ")
        if len(split) < 2 { log.Fatal("At least one parameter required for all instruction types: " + raw) }

        instr := Instruction { Name: split[0] }

        param1 := CreateParamter(split[1])
        instr.Parameter1 = param1

        if !param1.IsNumeric {
            if _, ok := machine.Register[param1.StringValue]; !ok {
                machine.Register[param1.StringValue] = 0
            }
        }

        if len(split) > 2 {
            param2 := CreateParamter(split[2])
            instr.Parameter2 = param2
        }

        machine.Instructions = append(machine.Instructions, instr)
    }

    return machine
}

func CreateMachine(id int) *Machine {
    machine := &Machine { Id: id, Instructions: []Instruction {}, Register: map[string]tInt {}, MessageQueue: Queue { Data: []tInt {}} }

    return machine
}

func CreateParamter(input string) *Parameter {
    param := &Parameter { }

    if (input[0] >= '0' && input[0] <= '9') || input[0] == '-' {
        param.IsNumeric = true
        val, err := strconv.Atoi(input)
        if err != nil {
            log.Fatal("Invalid parameter: " + input)
        }
        param.NumericValue = tInt(val)
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
