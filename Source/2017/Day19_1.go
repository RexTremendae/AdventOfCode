package main

import (
    "bufio"
    "fmt"
    "log"
    "os"
)

const (
    Down = iota
    Up
    Left
    Right
)

func main() {
    data := GetInput()

    x := 0
    for ; x < len((*data)[0]); x++ {
        if (*data)[0][x] == '|' { break }
    }

    y := 0
    dir := Down

    for {
        x, y = Move(x, y, dir)
        next := (*data)[y][x]
        if (IsLetter(next)) {
            fmt.Print(string(next))
        } else if next == '+' {
            if dir == Up || dir == Down {
                if (*data)[y][x+1] != ' ' {
                    dir = Right
                } else if (*data)[y][x-1] != ' ' {
                    dir = Left
                } else {
                    log.Fatal("\n!!!")
                }
            } else {
                if (*data)[y+1][x] != ' ' {
                    dir = Down
                } else if (*data)[y-1][x] != ' ' {
                    dir = Up
                } else {
                    log.Fatal("\n!!!")
                }
            }
        } else if next == '-' || next == '|' {
        } else {
            fmt.Println()
            fmt.Printf("(%d, %d) <END>\n", x, y)
            return
        }
    }
}

func Move(x int, y int, dir int) (int, int) {
    switch dir {
    case Down:
        y++
    case Up:
        y--
    case Right:
        x++
    case Left:
        x--
    }

    return x, y
}

func IsLetter(char byte) bool {
    if char >= 'a' && char <= 'z' { return true }
    if char >= 'A' && char <= 'Z' { return true }

    return false
}

func GetInput() *([]string) {
    file, err := os.Open("Day19.txt")
    if (err != nil) {
        log.Fatal("Could not open input file.")
    }
    defer file.Close()

    scanner := bufio.NewScanner(file)

    ret := []string{}
    for scanner.Scan() {
        ret = append(ret, scanner.Text())
    }

    return &ret
}
