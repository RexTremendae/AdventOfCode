package main

import (
	"bufio"
	"fmt"
	"log"
	"os"
)

type Point struct {
	X int
	Y int
}

const (
	Up = iota
	Down
	Left
	Right
)

const (
	Clean = iota
	Weakened
	Infected
	Flagged
)

func main() {
	grid := GetInput()
	PrintGrid(grid, Point{X:-4, Y:-4}, Point{X:4, Y:4})
	fmt.Println()
	dir := Up
	pos := Point {X:0, Y:0}
	infectionCount := 0

	for i := 0; i < 10000000; i++ {
		current := grid[pos]
		grid[pos]++
		if grid[pos] > Flagged { grid[pos] = Clean }
		if grid[pos] == Infected { infectionCount++ }

		switch dir {
		case Up:
			switch current {
			case Clean:
				dir = Left
			case Infected:
				dir = Right
			case Flagged:
				dir = Down
			}
		case Right:
			switch current {
			case Clean:
				dir = Up
			case Infected:
				dir = Down
			case Flagged:
				dir = Left
			}
		case Down:
			switch current {
			case Clean:
				dir = Right
			case Infected:
				dir = Left
			case Flagged:
				dir = Up
			}
		case Left:
			switch current {
			case Clean:
				dir = Down
			case Infected:
				dir = Up
			case Flagged:
				dir = Right
			}
		}

		switch dir {
		case Up:
			pos.Y--
		case Right:
			pos.X++
		case Down:
			pos.Y++
		case Left:
			pos.X--
		}
		//PrintGrid(grid, Point{X:-4, Y:-4}, Point{X:4, Y:4})
		//fmt.Println()
	}

	PrintGrid(grid, Point{X:-4, Y:-4}, Point{X:4, Y:4})
	fmt.Println(infectionCount)
}

func PrintGrid(grid map[Point]int8, topLeft Point, bottomRight Point) {
	for y := topLeft.Y; y <= bottomRight.Y; y++ {
		for x := topLeft.X; x <= bottomRight.X; x++ {
			switch grid[Point{X:x, Y:y}] {
			case Infected:
				fmt.Print("#")
			case Clean:
				fmt.Print(".")
			case Flagged:
				fmt.Print("F")
			case Weakened:
				fmt.Print("W")
			}

			if x == -1 && y == 0 {
				fmt.Print("(")
			} else if x == 0 && y == 0 {
				fmt.Print(")")
			} else {
				fmt.Print(" ")
			}
		}
		fmt.Println()
	}
}

func GetInput() map[Point]int8 {
	file, err := os.Open("Day22.txt")
	if err != nil {
		log.Fatal("Could not open input file.")
	}

	scanner := bufio.NewScanner(file)
	input := []string {}

	for scanner.Scan() {
		input = append(input, scanner.Text())
	}

	result := map[Point]int8 {}
	y := -len(input)/2
	for _, line := range input {
		x := -len(line)/2
		for _, data := range line {
			if data == '#' {
				result[Point {X:x, Y:y}] = Infected
			}
			x++
		}
		y++
	}

	return result
}