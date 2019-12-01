package main

import (
    "fmt"
)

type Node struct {
    Next *Node
    Prev *Node
    Value int
}

func main() {
    listLength := 256
    start := BuildList(listLength)
    currentNode := start
    skipSize := 0

    PrintList(start, listLength)
    fmt.Println("===============")

    //for _, currentInput := range []int { 3, 4, 1, 5 } {
    for _, currentInput := range []int { 34,88,2,222,254,93,150,0,199,255,39,32,137,136,1,167 } {
        currentNode, skipSize = DoMagic(start, currentNode, currentInput, skipSize)
        PrintList(start, listLength)
        fmt.Printf("Current: %d\n", currentNode.Value)
        fmt.Println("===============")
    }

    fmt.Printf("<ANSWER> %d", start.Value*start.Next.Value)
}

func DoMagic(start *Node, current *Node, input int, skipSize int) (*Node, int) {
    fmt.Printf("Input: %d\n", input)
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