package main

import (
    "bufio"
    "fmt"
    "log"
    "os"
)

type Garbage struct {
    NonCancelled int
}

type Group struct {
    Content[] interface {}
    Parent *Group 
    Score int
}

func main() {
    file, err := os.Open("Day9.txt")
    if (err != nil) {
        log.Fatalln("Could not open input file.")
    }
    defer file.Close()

    scanner := bufio.NewScanner(file)
    for scanner.Scan() {
        group := Parse(scanner.Text())
        score, nonCancelled := Evaluate(group, false)
        fmt.Printf("\nTotal score: %d", score)
        fmt.Printf("\nTotal non-cancelled characters: %d\n", nonCancelled)
        fmt.Println("===")
    }
}

func EvaluateOld(group *Group, print bool) (int, int) {
    return EvaluateInner(group, print, 0)
}

func Evaluate(node interface{}, print bool) (int, int) {
    return EvaluateInner(node, print, 0)
}

func EvaluateInner(node interface{}, print bool, indent int) (int, int) {
    if group, ok := node.(*Group); ok {
        return EvaluateGroup(group, print, indent)
    }
    if gbg, ok := node.(*Garbage); ok {
        return EvaluateGarbage(gbg, print, indent)
    }

    log.Fatal("Invalid type")
    return 0, 0
}

func EvaluateGroup(group *Group, print bool, indent int) (int, int) {
    if print {
        for i := 0; i < indent; i++ {
            fmt.Print(" ")
        }
        fmt.Println(group.Score)
    }

    score := group.Score
    nonCancelled := 0

    for _, content := range group.Content {
        currScore, currNonCancelled := EvaluateInner(content, print, indent+1)
        score += currScore
        nonCancelled += currNonCancelled
    }

    return score, nonCancelled
}

func EvaluateGarbage(gbg *Garbage, print bool, indent int) (int, int) {
    if print {
        for i := 0; i <= indent; i++ {
            fmt.Print(" ")
        }
        fmt.Printf("<%d>\n", gbg.NonCancelled)
    }

    return 0, gbg.NonCancelled
}

func EvaluateInnerOld(group *Group, print bool, indent int) (int, int) {
    score := group.Score
    nonCancelled := 0

    if print {
        for i := 0; i < indent; i++ {
            fmt.Print(" ")
        }
        fmt.Println(score)
    }

    for _, content := range group.Content {
        if group, ok := content.(*Group); ok {
            currScore, currNonCancelled := EvaluateInner(group, print, indent+1)
            score += currScore
            nonCancelled += currNonCancelled
        }
        if gbg, ok := content.(*Garbage); ok {
            if print {
                for i := 0; i <= indent; i++ {
                    fmt.Print(" ")
                }
                fmt.Printf("<%d>\n", gbg.NonCancelled)
                nonCancelled += gbg.NonCancelled
            }
        }
    }

    return score, nonCancelled
}

func Parse(input string) *Group {
    var root, group, parent *Group
    var gbg *Garbage
    skipNext := false

    for idx, char := range input {
        if skipNext {
            skipNext = false
            continue
        }

        if gbg != nil && char != '!' && char != '>' {
            gbg.NonCancelled++
            continue
        }

        switch char {
            case '!':
                if group == nil || gbg == nil {
                    log.Fatalf("Invalid input @%d\n", idx)
                }
                skipNext = true

            case '<':
                if group == nil {
                    log.Fatalf("Invalid input @%d\n", idx)
                }
                gbg = &Garbage {}
                group.Content = append(group.Content, gbg)

            case '>':
                if group == nil {
                    log.Fatalf("Invalid input @%d\n", idx)
                }
                gbg = nil

            case '{':
                if group == nil && root != nil {
                    log.Fatalf("Invalid input @%d\n", idx)
                }
                parent = group
                group = &Group { Parent: parent }
                if root == nil {
                    root = group
                }

                if parent == nil {
                    group.Score = 1
                } else {
                    parent.Content = append(parent.Content, group)
                    group.Score = parent.Score + 1
                }

            case ',':
                if group == nil {
                    log.Fatalf("Invalid input @%d\n", idx)
                }

            case '}':
                if group == nil {
                    log.Fatalf("Invalid input @%d\n", idx)
                }
                group = parent
                if group != nil {
                    parent = group.Parent
                }

            default:
                log.Fatalf("Invalid input @%d\n", idx)
        }
    }

    return root
}