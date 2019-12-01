package main

import (
    "fmt"
)

type Rule struct {
    NewValue bool
    Movement int
    NextState string
}

type TapeCell struct {
    Next *TapeCell
    Prev *TapeCell
    Value bool
}

func main() {
    checksum := 0
    steps := 12302209
    state := "A"

    rules := CreateRules()
    cell := &TapeCell {}

    //PrintTape(cell)

    for i := 0; i < steps; i++ {
        var rule Rule
        if cell.Value {
            rule = rules[state][1] 
        } else {
            rule = rules[state][0]
        }

        state = rule.NextState
        if !cell.Value && rule.NewValue {
            checksum++
        } else if cell.Value && !rule.NewValue {
            checksum--
        }
        cell.Value = rule.NewValue

        if rule.Movement == 1 {
            if cell.Next == nil {
                cell.Next = &TapeCell { Prev: cell }
            }

            cell = cell.Next
        } else if rule.Movement == -1 {
            if cell.Prev == nil {
                cell.Prev = &TapeCell { Next: cell }
            }

            cell = cell.Prev
        }

        //PrintTape(cell)
    }
    fmt.Println(checksum)
}

func PrintTape(cursor *TapeCell) {
    cell := cursor
    for cell.Prev != nil {
        cell = cell.Prev
    }

    PrintCellValue(cell, cursor == cell)

    for cell.Next != nil {
        cell = cell.Next
        PrintCellValue(cell, cursor == cell)
    }

    fmt.Println()
}

func PrintCellValue(cell *TapeCell, isCursor bool) {
    val := "0"
    if cell.Value { val = "1" }
    if isCursor {
        fmt.Printf("[%s] ", val)
    } else {
        fmt.Printf("%s ", val)
    }
}

func CreateRules() map[string]([]Rule) {
    rules := map[string]([]Rule) {}

    rules["A"] = []Rule {
        Rule { NewValue: true, Movement: 1, NextState: "B" },
        Rule { NewValue: false, Movement: -1, NextState: "D" } }

    rules["B"] = []Rule {
        Rule { NewValue: true, Movement: 1, NextState: "C" },
        Rule { NewValue: false, Movement: 1, NextState: "F" } }

    rules["C"] = []Rule {
        Rule { NewValue: true, Movement: -1, NextState: "C" },
        Rule { NewValue: true, Movement: -1, NextState: "A" } }

    rules["D"] = []Rule {
        Rule { NewValue: false, Movement: -1, NextState: "E" },
        Rule { NewValue: true, Movement: 1, NextState: "A" } }

    rules["E"] = []Rule {
        Rule { NewValue: true, Movement: -1, NextState: "A" },
        Rule { NewValue: false, Movement: 1, NextState: "B" } }

    rules["F"] = []Rule {
        Rule { NewValue: false, Movement: 1, NextState: "C" },
        Rule { NewValue: false, Movement: 1, NextState: "E" } }

    return rules
}

func CreateSampleRules() map[string]([]Rule) {
    rules := map[string]([]Rule) {}

    rules["A"] = []Rule {
        Rule { NewValue: true, Movement: 1, NextState: "B" },
        Rule { NewValue: false, Movement: -1, NextState: "B" } }

    rules["B"] = []Rule {
        Rule { NewValue: true, Movement: -1, NextState: "A" },
        Rule { NewValue: true, Movement: 1, NextState: "A" } }

    return rules
}