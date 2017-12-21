package main

import (
    "bufio"
    "fmt"
    "log"
    "os"
    "strings"
)

type Rule struct {
    In map[int]string
    Out map[int]string
}

func main() {
    rules2, rules3 := GetInput()
    rulesMap := map[int]([]Rule) {}
    rulesMap[2] = rules2
    rulesMap[3] = rules3
    debug := false

    art := map[int]string {
        0: ".#.",
        1: "..#",
        2: "###" }

    fmt.Println("Initial state:")
    Print(art)

    if debug {
        fmt.Println("Rotated:")
        Print(Rotate(art))
    }

    for i := 0; i < 5; i++ {
        size := len(art)
        cellSize, cellCount := 0, 0
        if size % 2 == 0 {
            cellSize = 2
            cellCount = size / 2
        } else if size % 3 == 0 {
            cellSize = 3
            cellCount = size / 3
        }

        newArt := map[int]string {}
        for y := 0; y < cellCount; y++ {
            for x := 0; x < cellCount; x++ {
                cell := Extract(art, cellSize, x, y)
                if debug {
                    fmt.Println("Extracted:")
                    Print(cell)
                }
                newCell := MatchAndApply(cell, rulesMap[cellSize])
                if debug {
                    fmt.Println("Match:")
                    Print(newCell)
                }
                newSize := len(newCell)
                for yRow := 0; yRow < newSize; yRow++ {
                    //fmt.Printf("%d, %d", yRow, y*cellSize + yRow)
                    line, ok := newArt[y*(cellSize+1) + yRow]
                    if !ok { line = "" }
                    newArt[y*(cellSize+1) + yRow] = line + newCell[yRow]
                    //fmt.Printf(": %s\n", newArt[yRow])
                }
            }
        }

        if debug {
            fmt.Println("Result: ")
            Print(newArt)
        }
        art = newArt
    }

    fmt.Println("End result:")
    Print(art)

    count := 0
    for _, line := range art {
        for _, c := range line {
            if string(c) == "#" { count++ }
        }
    }

    fmt.Println(count)
}

func Extract(art map[int]string, cellSize int, x int, y int) map[int]string {
/*
    fmt.Println("--- Extract ---")
    Print(art)
    fmt.Printf("Size: %d, CellSize: %d", len(art), cellSize)
    fmt.Println()
*/
    cell := map[int]string {}
    yCount := 0
    for yPos := y*cellSize; yPos < (y+1)*cellSize; yPos++ {
        xStart := x*cellSize
        cell[yCount] = art[yPos][xStart: xStart+cellSize]
        yCount++
    }
//    fmt.Println("---------------")

    return cell
}

func MatchAndApply(cell map[int]string, rules []Rule) map[int]string {
    matchingRules := []Rule {}
    size := len(cell)

    rotated := Rotate(cell)
    cells := []map[int]string { cell, rotated }

    for _, r := range rules {
        for _, currentCell := range cells {
            cell := currentCell
            LToRꜛMatch := true
            RToLꜛMatch := true
            LToRꜜMatch := true
            RToLꜜMatch := true

            for y := 0; y < len(r.In); y++ {
                for x := 0; x < len(r.In); x++ {
                    if r.In[y][x] != cell[y][x] {
                        LToRꜜMatch = false
                    }
                    if r.In[y][size-x-1] != cell[y][x] {
                        RToLꜜMatch = false
                    }
                    if r.In[size-y-1][x] != cell[y][x] {
                        LToRꜛMatch = false
                    }
                    if r.In[size-y-1][size-x-1] != cell[y][x] {
                        RToLꜛMatch = false
                    }
                }
            }

            if (LToRꜛMatch || RToLꜛMatch || LToRꜜMatch || RToLꜜMatch) {
                matchingRules = append(matchingRules, r)
                break
            }
        }
    }

    if len(matchingRules) == 0 { log.Fatal("No matching rules!") }
    if len(matchingRules) > 1 {
        fmt.Println("Multiple matching rules!")
        for _, r := range matchingRules {
            fmt.Printf("%s -> %s\n", r.In, r.Out)
        }
        log.Fatal("Aborting!")
    }

    return matchingRules[0].Out
}

func Rotate(input map[int]string) map[int]string {
    output := map[int]string {}
    size := len(input)

    for y := size-1; y >= 0; y-- {
        for x := 0; x < size; x++ {
            output[size-y-1] = output[size-y-1] + string(input[x][y])
        }
    }

    return output
}

func Print(art map[int]string) {
    for i := 0; i < len(art); i ++ {
        fmt.Println(art[i])
    }
    fmt.Println()
}

func GetInput() ([]Rule,[]Rule) {
    file, err := os.Open("Day21.txt")
    if err != nil { log.Fatal("Input file could not be opened") }
    defer file.Close()

    scanner := bufio.NewScanner(file)
    rules2 := []Rule {}
    rules3 := []Rule {}

    for scanner.Scan() {
        newRule := Rule {}
        ruleRaw := strings.Split(scanner.Text(), " => ")
        newRule.In = map[int]string{}
        for key, value := range strings.Split(ruleRaw[0], "/") {
            newRule.In[key] = value
        }
        newRule.Out = map[int]string{}
        for key, value := range strings.Split(ruleRaw[1], "/") {
            newRule.Out[key] = value
        }
        if len(newRule.In) == 2 && len(newRule.Out) == 3 {
            rules2 = append(rules2, newRule)
        } else if len(newRule.In) == 3 && len(newRule.Out) == 4 {
            rules3 = append(rules3, newRule)
        } else {
            log.Fatal("Erroneous input: " + ruleRaw[0] + " => " + ruleRaw[1])
        }
    }

    return rules2, rules3
}