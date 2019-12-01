package main

import (
    "bufio"
    "fmt"
    "log"
    "os"
    "sort"
    "strconv"
    "strings"
)

type Scanner struct {
    Pos int
    Max int
    Dir int
}

func main() {
    file, ok := os.Open("Day13.txt")
    if ok != nil {
        log.Fatal("Could not open input file")
    }
    defer file.Close()

    lastIdx := 0
    scanner := bufio.NewScanner(file)

    data := map[int]*Scanner {}

    for scanner.Scan() {
        split := strings.Split(scanner.Text(), ": ")

        currIdx, err := strconv.Atoi(split[0])
        if err != nil {
            log.Fatal(err)
        }

        currMax, err := strconv.Atoi(split[1])
        if err != nil {
            log.Fatal(err)
        }

        for lastIdx < currIdx {
            data[lastIdx] = &Scanner{0, 0, 0}
            lastIdx++
        }

        data[lastIdx] = &Scanner{Pos: 0, Max: currMax, Dir: 1}
        lastIdx++
    }

    severity := 0
    debug := false
    
    if (debug) {
        fmt.Println("Initial state:")
        PrintState(data)
        fmt.Println()
    }
    for i := 1; i < lastIdx; i++ {
        UpdateScanners(data)
        if debug {
            fmt.Printf("State %d:\n", i)
            PrintState(data)
        }
        severity += CalcSeverity(data, i, debug)
        if debug { fmt.Println() }
    }

    fmt.Printf("Severity: %d\n", severity)
}

func CalcSeverity(data map[int]*Scanner, pos int, debug bool) int {
    scanner := data[pos]
    if scanner.Max == 0 || scanner.Pos != 0 { return 0 }

    if debug {
        fmt.Printf("%d, %d\n", pos, scanner.Max)
    }

    return pos * scanner.Max
}

func UpdateScanners(data map[int]*Scanner) {
    for _, scanner := range data {
        scanner.Pos += scanner.Dir
        if scanner.Pos >= scanner.Max {
            scanner.Pos = scanner.Max - 2
            scanner.Dir = -1
        } else if scanner.Pos < 0 {
            scanner.Pos = 1
            scanner.Dir = 1
        }
    }
}

func PrintState(data map[int]*Scanner) {
    keys := []int {}
    for k, _ := range data {
        keys = append(keys, k)
    }
    sort.Ints(keys)

    for key := range keys {
        if (key < 10) { fmt.Print(" ") }
        if (key < 100) { fmt.Print(" ")}
        fmt.Printf("%d: ", key)

        scanner := data[key]
        if scanner.Max < 2 {
            fmt.Println()
            continue
        }

        for i := 0; i < scanner.Pos; i++ {
            fmt.Print("_ ")
        }
        fmt.Print("S ")
        for i := scanner.Pos+1; i < scanner.Max; i++ {
            fmt.Print("_ ")
        }

        fmt.Println()
    }
}