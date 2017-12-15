package main

import (
    "fmt"
)

func main() {
    genAFactor := int64(16807)
    genBFactor := int64(48271)

    // Example
    //genA := int64(65)
    //genB := int64(8921)

    // Puzzle input
    genA := int64(783)
    genB := int64(325)

    matchCount := int64(0)
    debug := false

    for i := 0; i < 40000000; i++ {
        genA = GenerateNext(genA, genAFactor)
        genB = GenerateNext(genB, genBFactor)

        isMatch := IsMatch(genA, genB)
        if isMatch { matchCount++ }

        if debug {
            match := ""
            if IsMatch(genA, genB) {
                match = "MATCH"
            }
            fmt.Printf("%12d  %12d   %s\n", genA, genB, match)    
        }
    }

    fmt.Printf("Match count: %d", matchCount)
}

func IsMatch(a int64, b int64) bool {
    for i := 0; i < 16; i++ {
        if a & 1 != b & 1 { return false }
        a >>= 1
        b >>= 1
    }

    return true
}

func GenerateNext(prev int64, factor int64) int64 {
    val := prev * factor
    divider := int64(2147483647)

    return val % divider
}
