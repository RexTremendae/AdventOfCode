package main

import (
    "fmt"
    "strconv"
    "strings"
)

func main() {
    blocks := []int {4,1,15,12,0,9,9,5,5,8,7,3,14,5,12,3}
    blocksLen := len(blocks)
    key := CreateKey(blocks)
    visited := map[string]int { key: 1 }

    steps := 0
    firstVisitedFound := false

    for {
        maxIdx := GetMaxIdx(blocks)
        bucket := blocks[maxIdx]
        blocks[maxIdx] = 0
        idx := maxIdx

        for {
            idx++
            if idx >= blocksLen {
                idx = 0
            }

            blocks[idx] ++
            bucket--

            if bucket < 1 { break }
        }
        steps++

        key = CreateKey(blocks)
        if _, ok := visited[key]; ok {
            if (firstVisitedFound) {
                fmt.Println(steps)
                return;
            }

            key = CreateKey(blocks)
            visited = map[string]int { key: 1 }
            steps = 0

            firstVisitedFound = true
        }
        visited[key] = 1
    }
}

func GetMaxIdx(data []int) int {
    maxVal := 0
    maxIdx := -1

    for idx, value := range data {
        if value > maxVal {
            maxVal = value
            maxIdx = idx
        }
    }

    return maxIdx
}

func CreateKey(data []int) string {
    stringArray := []string {}

    for _, value := range data {
        stringArray = append(stringArray, strconv.Itoa(value))
    }

    return strings.Join(stringArray, ",")
}