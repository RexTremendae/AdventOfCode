package main

import (
    "bufio"
    "fmt"
    "log"
    "os"
    "sort"
    "strings"
)

func main() {
	file, err := os.Open("input.txt")
    if err != nil {
        log.Fatal(err)
    }
    defer file.Close()
    scanner := bufio.NewScanner(file)
    validPhrasesCount := 0

    for scanner.Scan() {
        passphrase := scanner.Text()
        words := map[string]int{}

        split := strings.Split(passphrase, " ")
        phraseOk := true

        for _, currWord := range split {
            currWord = Sort(currWord)
            if _, ok := words[currWord]; ok {
                phraseOk = false
                break
            }
            words[currWord] = 1
        }
        if (phraseOk == false) { continue }

        validPhrasesCount ++
    }

    fmt.Println(validPhrasesCount)
}

func Sort(input string) string {
    charArray := strings.Split(input, "")
    sort.Strings(charArray)
    return strings.Join(charArray, "")
}