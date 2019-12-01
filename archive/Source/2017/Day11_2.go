package main

import (
    "bufio"
    "fmt"
    "log"
    "os"
    "strings"
)

func main() {
    file, ok := os.Open("Day11.txt")
    if ok != nil {
        log.Fatal("Could not open input file")
    }
    defer file.Close()

    maxDist := 0
    scanner := bufio.NewScanner(file)

    for scanner.Scan() {
        if (len(scanner.Text()) == 0 || scanner.Text()[0] == '#') { continue }
        split := strings.Split(scanner.Text(), ":")
        label := split[0]
        split = strings.Split(strings.TrimSpace(split[1]), ",")
        x, y := 0, 0

        for _, move := range split {
            x, y = MoveXY(move, x, y)
            //fmt.Printf("%s: (%v, %v), ", move, x, y)
            currDist := DistXY(x, y)
            if currDist > maxDist { maxDist = currDist }
        }
        //fmt.Println()

        dist := DistXY(x, y)
        fmt.Printf("[%s] d: %v, X: %v, Y: %v\n", label, dist, x, y)
        fmt.Printf("Max dist: %v\n", maxDist)
        fmt.Println()
    }
}

func MoveXY(move string, x int, y int) (int, int) {
    switch move {
    case "n":
        y++

    case "s":
        y--

    case "ne":
        if (x % 2 == 0) {
            y++
        }
        x++

    case "nw":
        if (x % 2 == 0) {
            y++
        }
        x--

    case "se":
        if (x % 2 != 0) {
            y--
        }
        x++

    case "sw":
        if (x % 2 != 0) {
            y--
        }
        x--
    }

    return x, y
}

func DistXY(x int, y int) int {
    dist := 0
    for ;x != 0 || y != 0; {
        var move string
        if y > 0 {
            move = move + "s"
        } else if y < 0 {
            move = move + "n"
        }

        if x > 0 {
            move = move + "w"
        } else if x < 0 {
            move = move + "e"
        }

        if y == 0 {
            if x % 2 == 0 {
                move = "s" + move                
            } else {
                move = "n" + move
            }
        }

        x, y = MoveXY(move, x, y)
        dist++
    }

    return dist
}