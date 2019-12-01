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

type Point struct {
    X int
    Y int
}

type Queue struct {
    Data []Point
}

func Enqueue(queue *Queue, point Point) {
    queue.Data = append(queue.Data, point)
}

func Dequeue(queue *Queue) Point {
    dequeued := queue.Data[0]
    queue.Data = queue.Data[1:]
    return dequeued
}

func IsEmpty(queue *Queue) bool {
    return len(queue.Data) <= 0
}

func main() {
    //input := "flqrgnkx" // example
    input := "jzgqcdpd" // puzzle input

    grid := BuildGrid(input)

    regions := 0
    for y := 0; y < 128; y++ {
        for x := 0; x < 128; x++ {
            if ExploreRegion(&grid, Point { X: x, Y: y }) {
                regions ++
            }
        }
    }

    fmt.Println(regions)
}

func ExploreRegion(grid *[]([]bool), p Point) bool {
    if !(*grid)[p.Y][p.X] { return false }
    queue := &Queue { Data: []Point {} }
    Enqueue(queue, p)

    for !IsEmpty(queue) {
        dqp := Dequeue(queue)

        (*grid)[dqp.Y][dqp.X] = false
        TryEnqueueNeighbours(grid, queue, dqp.X, dqp.Y)
    }

    return true
}

func TryEnqueueNeighbours(grid *[]([]bool), queue *Queue, x int, y int) {
    if x > 0 && (*grid)[y][x-1] { Enqueue(queue, Point {x-1, y}) }
    if y > 0 && (*grid)[y-1][x] { Enqueue(queue, Point {x, y-1}) }
    if x < 127 && (*grid)[y][x+1] { Enqueue(queue, Point {x+1, y}) }
    if y < 127 && (*grid)[y+1][x] { Enqueue(queue, Point {x, y+1}) }
}

func BuildGrid(input string) []([]bool) {
    grid := []([]bool){}

    for i := 0; i < 128; i++ {
        gridRow := make([]bool, 128)

        hash := KnotHash(fmt.Sprintf("%s-%d", input, i), false)

        idx := 0
        for _, chr := range hash {
            val := 0
            if chr >= '0' && chr <= '9' {
                val = int(chr - '0')
            } else {
                val = int(chr - 'a') + 10
            }

            for i := 0; i < 4; i ++ {
                curr := idx + 3 - i
                if val & 1 == 1 {
                    gridRow[curr] = true
                } else {
                    gridRow[curr] = false
                }
                val >>= 1
            }
            idx += 4
        }

        grid = append(grid, gridRow)
    }

    return grid
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