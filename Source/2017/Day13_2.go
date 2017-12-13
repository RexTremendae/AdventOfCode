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

    state := map[int]*Scanner {}

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
            state[lastIdx] = &Scanner{0, 0, 0}
            lastIdx++
        }

        state[lastIdx] = &Scanner{Pos: 0, Max: currMax, Dir: 1}
        lastIdx++
    }

    debug := false

    if (debug) {
        fmt.Println("Initial state:")
        PrintState(&state)
        fmt.Println()
    }

    states := map[int]*(map[int]*Scanner){}
    states[0] = &state
    for delay := 0; ; delay++ {
        ps := delay
        caught := false

        if delay > 0 { delete(states, delay-1) }

        if debug { fmt.Printf("Delay: %d\n", delay) }

        if delay % 10000 == 0 { fmt.Printf("%d...\n", delay) }

        for i := 0; i < lastIdx; i++ {
            currIdx := ps+i
            if _, ok := states[currIdx]; !ok {
                copy := CopyState(states[currIdx-1])
                UpdateScanners(copy)
                states[currIdx] = &copy
            }

            if debug {
                PrintState(states[currIdx]) 
                fmt.Println()
            }

            if CalcSeverity(states[currIdx], i, debug) != 0 {
                caught = true
                continue
            }
        }

        if !caught {
            fmt.Printf("Not caught when delay = %d\n", delay)
            return
        }
    }
}

func CopyState(state *map[int]*Scanner) map[int]*Scanner {
    copy := map[int]*Scanner {}
    for key, scanner := range *state {
        copy[key] = &Scanner { Pos: scanner.Pos, Max: scanner.Max, Dir: scanner.Dir }
    }

    return copy
}

func CalcSeverity(state *map[int]*Scanner, pos int, debug bool) int {
    scanner := (*state)[pos]
    if scanner.Max == 0 || scanner.Pos != 0 { return 0 }

    if debug {
        fmt.Printf("%d, %d\n", pos, scanner.Max)
    }

    return (1+pos) * scanner.Max
}

func UpdateScanners(state map[int]*Scanner) {
    for _, scanner := range state {
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

func PrintState(state *map[int]*Scanner) {
    keys := []int {}
    for k, _ := range *state {
        keys = append(keys, k)
    }
    sort.Ints(keys)

    for key := range keys {
        if (key < 10) { fmt.Print(" ") }
        if (key < 100) { fmt.Print(" ")}
        fmt.Printf("%d: ", key)

        scanner := (*state)[key]
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