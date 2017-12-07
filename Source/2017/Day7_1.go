package main

import (
    "bufio"
    "fmt"
    "os"
    "strings"
)

type Node struct {
    Name string
    Parent* Node
    Children []*Node
}

func main() {
    file, err := os.Open("Day7.txt")
    if (err != nil) {
        fmt.Println("Could not open input file.")
    }
    defer file.Close()

    nodeMap := map[string]*Node {}
    childrenMap := map[string][]string {}

    scanner := bufio.NewScanner(file)
    for scanner.Scan() {
        DecodeRow(scanner.Text(), nodeMap, childrenMap)
    }

    MapChildren(nodeMap, childrenMap)

    fmt.Println("--------------------")

    PrintTreeNodes(nodeMap)

    fmt.Println("--------------------")

    fmt.Println("Orphans: ")
    for name, node := range nodeMap {
        if node.Parent == nil {
            fmt.Println(name)
        }
    }
}

func DecodeRow(text string, nodeMap map[string]*Node, childrenMap map[string][]string) {
    split := strings.Split(text, " -> ")
    nameSplit := strings.Split(split[0], " ")
    name := nameSplit[0]

    if (len(split)) > 1 {
        childSplit := strings.Split(split[1], ", ")
        children := []string { }

        for _, childName := range childSplit {
            children = append(children, childName)
        }

        childrenMap[name] = children
    }
    nodeMap[name] = &Node { Name: name }
}

func MapChildren(nodeMap map[string]*Node, childrenMap map[string][]string) {
    for parent, children := range childrenMap {
        node := nodeMap[parent]
        fmt.Println(node.Name)
        for _, childName := range children {
            child := nodeMap[childName]
            fmt.Println("  " + childName)
            node.Children = append(node.Children, child)
            child.Parent = node
        }
    }
}

func PrintTreeNodes(nodeMap map[string]*Node) {
    for _, node := range nodeMap {
        fmt.Print(node.Name + " <")
        if node.Parent == nil {
            fmt.Print("*orphan*")
        } else {
            fmt.Print(node.Parent.Name)
        }
        fmt.Print("> ")

        for _, child := range node.Children {
            fmt.Print(child.Name + " ")
        }
        fmt.Println()
    }
}
