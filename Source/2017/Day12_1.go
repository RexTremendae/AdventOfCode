package main

import (
    "bufio"
    "fmt"
    "log"
    "os"
    "strconv"
    "strings"
)

type Node struct {
    Id int
    Neighbours map[int]*Node
}

func main() {
    file, ok := os.Open("Day12.txt")
    if ok != nil {
        log.Fatal("Could not open input file")
    }
    defer file.Close()

    nodes := map[int]*Node {}
    var zeroNode *Node

    scanner := bufio.NewScanner(file)
    for scanner.Scan() {
        split := strings.Split(scanner.Text(), " <-> ")
        nodeId, err := strconv.Atoi(split[0])
        if err != nil {
            log.Fatal(err)
        }

        var node *Node
        if existingNode, ok := nodes[nodeId]; ok {
            node = existingNode
        } else {
            node = &Node { Id: nodeId, Neighbours: map[int]*Node {} }
            nodes[nodeId] = node
        }

        if nodeId == 0 { zeroNode = node }

        for _, neighbour := range strings.Split(split[1], ", ") {
            neighId, err := strconv.Atoi(neighbour)
			if err != nil {
				log.Fatal(err)
            }

            var neighbour *Node
            if existing, ok := nodes[neighId]; ok {
                neighbour = existing
            } else {
                neighbour = &Node { Id: neighId, Neighbours: map[int]*Node {} }
                nodes[neighId] = neighbour
            }

            if _, ok := neighbour.Neighbours[nodeId]; ok { continue }
            if _, ok := node.Neighbours[neighId]; ok { continue }

            neighbour.Neighbours[nodeId] = node
            node.Neighbours[neighId] = neighbour
        }
    }

    members := map[int]*Node {}

    queue := make([]*Node, 0)

    queue = append(queue, zeroNode)

    for len(queue) > 0 {
        dequeued := queue[0]
        queue = queue[1:]

        for _, neigh := range dequeued.Neighbours {
            if _, ok := members[neigh.Id]; ok { continue }
            members[neigh.Id] = neigh
            queue = append(queue, neigh)
        }
    }

    fmt.Println(len(members))

    // Push
    // queue = append(queue, 1)

    // Top (just get next element, don't remove it)
    // x := queue[0]

    // Discard top element
    // queue = queue[1:]

    // Is empty ?
    //if len(queue) == 0 { ... }

    //PrintAllNodes(nodes)
}

func PrintAllNodes(nodes map[int]*Node) {
    for _, node := range nodes {
        fmt.Printf("[%d] ", node.Id)
        for _, neigh := range node.Neighbours {
            fmt.Printf ("%d ", neigh.Id)
        }
        fmt.Println()
    }
}