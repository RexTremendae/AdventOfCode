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
    // input := "flqrgnkx" // example
    input := "jzgqcdpd" // puzzle input
    debug := false
    count := 0

    for i := 0; i < 128; i++ {
        hash := KnotHash(fmt.Sprintf("%s-%d", input, i), false)
        if debug {
            fmt.Println(hash)
        }

        for _, chr := range hash {
            val := 0
            if chr >= '0' && chr <= '9' {
                val = int(chr - '0')
            } else {
                val = int(chr - 'a') + 10
            }

            seq := ""
            for i := 0; i < 4; i ++ {
                if val & 1 == 1 {
                    seq = "#" + seq
                    count++
                } else {
                    seq = "." + seq
                }
                val >>= 1
            }
            if debug {
                fmt.Print(seq)
            }
        }
        if debug {
            fmt.Println()
        }
    }

    fmt.Println(count)
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