package main

import (
    "bufio"
    "fmt"
    "log"
    "os"
    "strconv"
    "strings"
)

func main() {
    file, err := os.Open("input.txt")
    if err != nil {
        log.Fatal(err)
    }

    defer file.Close()
    checksum := 0
    scanner := bufio.NewScanner(file)

    for scanner.Scan() {
        data := []int { }

        for _, curr := range strings.Split(scanner.Text(), "\t") {
            val, err := strconv.Atoi(curr)
            if err != nil { log.Fatal(err) }
            data = append(data, val)
        }

        diff := 0
        for _, curr := range data {
            for _, curr2 := range data {
                if curr != curr2 && curr % curr2 == 0 {
                    if curr < curr2 {
                        diff = curr2 / curr
                    } else {
                        diff = curr / curr2
                    }

                    break;
                }
            }
            if diff != 0 { break }
        }
        checksum += diff
    }

    defer fmt.Println(checksum)

    if err := scanner.Err(); err != nil {
        log.Fatal(err)
    }
}