package main

import (
	"strconv"
    "bufio"
    "fmt"
    "log"
    "os"
)

func main() {
	file, err := os.Open("input.txt")
    if err != nil {
        log.Fatal(err)
    }
    defer file.Close()
    scanner := bufio.NewScanner(file)

    instructions := []int { }

    for scanner.Scan() {
        currInstr, err := strconv.Atoi(scanner.Text())
        if err != nil {
            log.Fatal(err)
        }
        instructions = append(instructions, currInstr);
    }

    lastIdx := len(instructions) - 1
    stepCounter := 0
    currIdx := 0

    for {
        prevIdx := currIdx
        offset := instructions[currIdx]
        currIdx += offset
        if offset >= 3 {
            instructions[prevIdx] --
        } else {
            instructions[prevIdx] ++
        }

        stepCounter ++

        if currIdx > lastIdx {
            break;
        }
    }

    fmt.Println(stepCounter)
}