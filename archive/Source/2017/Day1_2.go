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

    len := len(input)
    halfLen := len/2
    for i := 0; i < len; i++ {
        compIdx := i+halfLen
        if compIdx >= len { compIdx = compIdx-len }

        if (input[i] == input[compIdx]) { sum = sum + int(input[i] - '0') }
    }

    fmt.Println("Result: " + strconv.Itoa(sum))
}

