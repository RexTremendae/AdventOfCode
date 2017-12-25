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
    mappedInput := CreateMap(GetInput())

    fmt.Println(GetLongest(mappedInput))
}

func GetLongest(input map[int] ([]Pair)) int {
    longest := 0
    strongest := 0
    for _, itm := range input[0] {
        tmpLong, tmpStr := GetLongestIt(input, map[int]bool { itm.Id: true }, itm.Item1 + itm.Item2, itm, 1)
        if tmpLong > longest {
            longest = tmpLong
            strongest = tmpStr
        } else if tmpLong == longest && tmpStr > strongest {
            strongest = tmpStr
        }
    }

    return strongest
}

func GetLongestIt(input map[int] ([]Pair), visited map[int]bool, strongestTot int, curr Pair, length int) (int, int) {
    longest := length
    strongest := strongestTot
    for _, itm := range input[curr.Item2] {
        if _, ok := visited[itm.Id]; ok {
            continue
        }

        copy := Copy(visited)
        copy[itm.Id] = true
        tmpLong, tmpStr := GetLongestIt(input, copy, strongestTot + itm.Item1 + itm.Item2, itm, length+1)
        if tmpLong > longest {
            longest = tmpLong
            strongest = tmpStr
        } else if tmpLong == longest && tmpStr > strongest {
            longest = tmpLong
            strongest = tmpStr
        }
    }

    return longest, strongest
}

func Copy(in map[int]bool) map[int]bool {
    out := make(map[int]bool, len(in))

    for key, val := range in {
        out[key] = val
    }

    return out
}

type Pair struct {
    Item1 int
    Item2 int
    Id int
}

func CreateMap(pairs []Pair) map[int] ([]Pair) {
    pairMap := map[int] ([]Pair) {}
    for _, itm := range pairs {
        pairMap[itm.Item1] = append(pairMap[itm.Item1], Pair { Id: itm.Id, Item1: itm.Item1, Item2: itm.Item2 })
        if itm.Item1 != itm.Item2 {
            pairMap[itm.Item2] = append(pairMap[itm.Item2], Pair { Id: itm.Id, Item1: itm.Item2, Item2: itm.Item1 })
        }
    }

    return pairMap
}

func GetInput() []Pair {
    file, err := os.Open("Day24.txt")
    if err != nil {
        log.Fatal("Could not open file")
    }
    defer file.Close()

    result := []Pair {}
    scanner := bufio.NewScanner(file)
    idx := 0
    for scanner.Scan() {
        split := strings.Split(scanner.Text(), "/")
        i1, err := strconv.Atoi(split[0])
        if err != nil { log.Fatal("Input file error") }
        i2, err := strconv.Atoi(split[1])
        if err != nil { log.Fatal("Input file error") }

        result = append(result, Pair { Id: idx, Item1: i1, Item2: i2 })
        idx++
    }

    return result
}
