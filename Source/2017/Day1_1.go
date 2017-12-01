package main

import (
    "fmt"
    "io/ioutil"
    "strconv"
)

func main() {
    input, err := ioutil.ReadFile("input.txt")
    if err != nil {
        fmt.Println("Error reading input file!")
        return
    }
    sum := 0

    fmt.Println("Evaluating " + string(input))

    for i := 0; i < len(input); i++ {
        compIdx := i+1
        if compIdx >= len(input) { compIdx = 0 }

        if (input[i] == input[compIdx]) { sum = sum + int(input[i] - '0') }
    }

    fmt.Println("Result: " + strconv.Itoa(sum))
}

