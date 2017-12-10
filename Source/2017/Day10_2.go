package main

import (
    "bytes"
    "fmt"
)

type Node struct {
    Next *Node
    Prev *Node
    Value int
}

func main() {
    Hash("")
    Hash("AoC 2017")
    Hash("1,2,3")
    Hash("1,2,4")
    Hash("34,88,2,222,254,93,150,0,199,255,39,32,137,136,1,167")
}

func Hash(input string) {
    fmt.Printf("Input: \"%s\" -> %s\n", input, KnotHash(input, false))
}

func KnotHash(inputSequence string, print bool) string {
    inputNumbers := []int {}
    skipSize := 0
    listLength := 256
    start := BuildList(listLength)

    for _, char := range inputSequence {
        inputNumbers = append(inputNumbers, int(char))
    }
    inputNumbers = append(inputNumbers, 17, 31, 73, 47, 23)

    currentNode := start

    for i := 0; i < 64; i++ {
        for _, currentInput := range inputNumbers {
            currentNode, skipSize = DoMagic(start, currentNode, currentInput, skipSize, print)
            if print {
                PrintList(start, listLength)
                fmt.Printf("Current: %d\n", currentNode.Value)
                fmt.Println("===============")
            }
        }
    }

    denseHash := []int {}
    it := start

    for i := 0; i < 16; i++ {
        xor := it.Value
        it = it.Next

        for j := 1; j < 16; j++ {
            xor = xor ^ it.Value
            it = it.Next
        }

        denseHash = append(denseHash, xor)
    }

    var output bytes.Buffer
    for _, d := range denseHash {
        h := fmt.Sprintf("%02x", d)
        output.WriteString(h)
    }

    return output.String()
}

func DoMagic(start *Node, current *Node, input int, skipSize int, print bool) (*Node, int) {
    if print {
        fmt.Printf("Input: %d\n", input)
    }

    first := current
    last := current
    for i := 0; i < input-1; i ++ {
        last = last.Next
    }

    for {
        if (first == last) { break }

        temp := first.Value
        first.Value = last.Value
        last.Value = temp

        last = last.Prev
        if (first == last) { break }

        first = first.Next
    }

    for i := 0; i < input+skipSize; i++ {
        current = current.Next
    }

    return current, skipSize+1
}

func PrintList(start *Node, length int) {
    node := start
    fmt.Println("Forward:")
    for i := 0; i < length+1; i++ {
        fmt.Printf("%d ", node.Value)
        node = node.Next
    }
    fmt.Println()

    node = start
    fmt.Println("Backwards:")
    for i := 0; i < length+1; i++ {
        fmt.Printf("%d ", node.Value)
        node = node.Prev
    }
    fmt.Println()
}

func BuildList(length int) *Node {
    start := &Node { Value: 0 }
    last := start

    for i := 1; i < length; i++ {
        node := &Node { Value: i }
        node.Prev = last
        last.Next = node
        last = node
    }

    start.Prev = last
    last.Next = start

    return start
}