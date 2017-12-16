package main

import (
	"bufio"
	"fmt"
	"log"
	"os"
	"strconv"
	"strings"
)

type Program struct {
	Name string
	Next *Program
	Prev *Program
}

func main() {
	// Puzzle input
	len := 16
	input := GetInput()

	// Small example
	//len := 5
	//input := "s1,x3/4,pe/b"

	start := CreateDancers(len)
	start = Dance(start, input)

	PrintForward(start, len)

	fmt.Println()
}

func GetInput() string {
	file, err := os.Open("Day16.txt")
    if (err != nil) {
        fmt.Println("Could not open input file.")
    }
    defer file.Close()

	scanner := bufio.NewScanner(file)
	scanner.Scan()
    return scanner.Text()
}

func Dance(start *Program, input string) *Program {
	danceSteps := strings.Split(input, ",")

	for _, step := range danceSteps {
		switch step[0] {
		case 's':
			movement, err := strconv.Atoi(step[1:])
			if err != nil {
				log.Fatal("Invalid dance move: " + step)				
			}
			for ;movement > 0; movement-- { start = start.Prev }
		case 'x':
			positions := strings.Split(step[1:], "/")
			pos1, err := strconv.Atoi(positions[0])
			if err != nil {
				log.Fatal("Invalid dance move: " + step)				
			}
			pos2, err := strconv.Atoi(positions[1])
			if err != nil {
				log.Fatal("Invalid dance move: " + step)				
			}
			swap1 := start
			swap2 := start
			for ;pos1 > 0; pos1-- { swap1 = swap1.Next }
			for ;pos2 > 0; pos2-- { swap2 = swap2.Next }
			tmp := swap2.Name
			swap2.Name = swap1.Name
			swap1.Name = tmp
		case 'p':
			partners := strings.Split(step[1:], "/")
			swap1 := start
			swap2 := start
			for ;swap1.Name != partners[0]; { swap1 = swap1.Next }
			for ;swap2.Name != partners[1]; { swap2 = swap2.Next }
			tmp := swap2.Name
			swap2.Name = swap1.Name
			swap1.Name = tmp
		default:
			log.Fatal("Invalid dance move: " + step)
		}
	}

	return start
}

func PrintForward(start *Program, len int) {
	node := start
	for i := 0; i < len; i ++ {
		fmt.Printf("%s", node.Name)
		node = node.Next
	}
	fmt.Println()
}

func PrintBackwards(start *Program, len int) {
	node := start.Prev
	for i := 0; i < len; i ++ {
		fmt.Printf("%s", node.Name)
		node = node.Prev
	}
}

func CreateDancers(len int) *Program {
	var start *Program;
	var prev *Program
	var new *Program

	for i := 0; i < len; i++ {
		new = &Program { Name: string('a' + i), Prev: prev }
		if start == nil {
			start = new
		} else {
			prev.Next = new
		}
		prev = new
	}

	new.Next = start
	start.Prev = new

	return start
}