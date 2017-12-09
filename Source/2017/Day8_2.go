package main

import (
    "bufio"
    "fmt"
    "log"
    "os"
    "strconv"
    "strings"
)

type Instruction struct {
    Register string
    Change int
    ComparisonOperator string
    ComparisonRegister string
    ComparisonValue int
}

func main() {
    file, err := os.Open("Day8.txt")
    if (err != nil) {
        log.Fatal("Could not open input file.")
    }
    defer file.Close()

    registers := map[string]int {}
    max := int(-1e6)

    scanner := bufio.NewScanner(file)
    for scanner.Scan() {
        instruction := Parse(scanner.Text())
        /*
        fmt.Printf("Register: %s += %d if %s %s %d\n",
            instruction.Register,
            instruction.Change,
            instruction.ComparisonRegister,
            instruction.ComparisonOperator,
            instruction.ComparisonValue)
        */

        if _, ok := registers[instruction.Register]; !ok {
            registers[instruction.Register] = 0
        }
        if _, ok := registers[instruction.ComparisonRegister]; !ok {
            registers[instruction.ComparisonRegister] = 0
        }

        evaluate := false

        compRegVal := registers[instruction.ComparisonRegister]
        compVal := instruction.ComparisonValue

        switch (instruction.ComparisonOperator) {
            case "==":
                evaluate = compRegVal == compVal
            case "!=":
                evaluate = compRegVal != compVal
            case "<=":
                evaluate = compRegVal <= compVal
            case ">=":
                evaluate = compRegVal >= compVal
            case "<":
                evaluate = compRegVal < compVal
            case ">":
                evaluate = compRegVal > compVal
        }

        if evaluate {
            registers[instruction.Register] += instruction.Change
            last := registers[instruction.Register]
            if last > max {
                max = last
            }
        }
    }

    for key, val := range registers {
        fmt.Printf("%s: %d\n", key, val)
    }
    fmt.Println("===")
    fmt.Printf("Max: %d\n", max)
}

func Parse(row string) Instruction {
    ifSplit := strings.Split(row, " if ")
    if len(ifSplit) < 2 {
        log.Fatal("Invalid input: " + row)
    }

    leftSplit := strings.Split(ifSplit[0], " ")
    rightSplit := strings.Split(ifSplit[1], " ")

    if len(leftSplit) < 3 || len(rightSplit) < 3 {
        log.Fatal("Invalid input: " + row)
    }

    incDec, err := strconv.Atoi(leftSplit[2])
    if err != nil {
        log.Fatal("Invalid input: " + row)
    }

    if leftSplit[1] == "dec" {
        incDec = -incDec
    }

    compVal, err := strconv.Atoi(rightSplit[2])
    if err != nil {
        log.Fatal("Invalid input: " + row)
    }

    op := rightSplit[1]

    if op != "<" &&
       op != ">" &&
       op != "<=" &&
       op != ">=" &&
       op != "==" &&
       op != "!=" {
        log.Fatal("Invalid input: " + row)
    }

    return Instruction {
        Register: leftSplit[0],
        Change: incDec,
        ComparisonOperator: op,
        ComparisonRegister: rightSplit[0],
        ComparisonValue: compVal }
}