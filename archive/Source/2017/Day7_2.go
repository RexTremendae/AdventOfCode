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
    Name string
    Parent* Node
    Children []*Node
    Weight int
    TotalWeight int
}

func main() {
    file, err := os.Open("Day7.txt")
    if (err != nil) {
        log.Fatal(err)
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

    root := &Node {}
    for _, node := range nodeMap {
        if node.Parent == nil {
            root = node
            break
        }
    }

    RecalculateWeights(root)

    PrintTreeNodes(nodeMap)
    fmt.Println("--------------------")

    CalculateAndPrintUnbalance(root)
}

func CalculateAndPrintUnbalance(root *Node) {
    unbalanced := FindUnbalanced(root)
    balanced := &Node {}
    balanced = nil
    fmt.Println("Unbalanced node: " + unbalanced.Name)

    for _, node := range unbalanced.Parent.Children {
        if node.Name != unbalanced.Name {
            balanced = node
            fmt.Println("Any balanced sibling: " + node.Name)
            break
        }
    }

    fmt.Printf("Weight needed for balance: %d", unbalanced.Weight - unbalanced.TotalWeight + balanced.TotalWeight)
}

func RecalculateWeights(node *Node) int {
    sum := node.Weight
    for _, child := range node.Children {
        sum += RecalculateWeights(child)
    }
    node.TotalWeight = sum
    return sum
}

func FindUnbalanced(root *Node) *Node {
    weight1, weight2 := 0, 0
    weight1Count, weight2Count := 0, 0
    weight1Node, weight2Node := &Node {}, &Node {}
    weight1Node, weight2Node = nil, nil

    for _, node := range root.Children {
        if weight1 == node.TotalWeight {
            weight1Count++
            continue
        } else if weight2 == node.TotalWeight {
            weight2Count++
            continue
        } else if weight1 == 0 {
            weight1 = node.TotalWeight
            weight1Node = node
            weight1Count++
        } else if weight2 == 0 {
            weight2 = node.TotalWeight
            weight2Node = node
            weight2Count++            
        } else {
            log.Fatal("More than two different weights for children of node " + root.Name)
        }
    }

    if weight1Count == weight2Count {
        log.Fatal("Cannot determine which child is unbalanced for node " + root.Name)        
    }

    retNode := &Node {}
    retNode = nil

    if weight2Count == 0 {
        retNode = root
    } else if weight1Count == 1 {
        retNode = FindUnbalanced(weight1Node)
    } else if weight2Count == 1 {
        retNode = FindUnbalanced(weight2Node)        
    } 

    return retNode
}

func DecodeRow(text string, nodeMap map[string]*Node, childrenMap map[string][]string) {
    split := strings.Split(text, " -> ")
    nameSplit := strings.Split(split[0], " ")
    name := nameSplit[0]
    weight, err := strconv.Atoi(strings.TrimRight(strings.TrimLeft(nameSplit[1], "("), ")"))

    if (err != nil) {
        log.Fatal(err)
    }

    if (len(split)) > 1 {
        childSplit := strings.Split(split[1], ", ")
        children := []string { }

        for _, childName := range childSplit {
            children = append(children, childName)
        }

        childrenMap[name] = children
    }
    nodeMap[name] = &Node { Name: name, Weight: weight }
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
        fmt.Printf("%s (%d) <", node.Name, node.Weight)
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
