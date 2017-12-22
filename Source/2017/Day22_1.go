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

func main() {
	grid := GetInput()
	PrintGrid(grid, Point{X:-4, Y:-4}, Point{X:4, Y:4})
	fmt.Println()
	dir := Up
	pos := Point {X:0, Y:0}
	infectionCount := 0

	for i := 0; i < 10000; i++ {
		current := grid[pos]
		if current {
			grid[pos] = false
		} else {
			grid[pos] = true
			infectionCount++
		}

		switch dir {
		case Up:
			if current {
				dir = Right
				pos.X++
			} else {
				dir = Left
				pos.X--
			}
		case Right:
			if current {
				dir = Down
				pos.Y++
			} else {
				dir = Up
				pos.Y--
			}
		case Down:
			if current {
				dir = Left
				pos.X--
			} else {
				dir = Right
				pos.X++
			}
		case Left:
			if current {
				dir = Up
				pos.Y--
			} else {
				dir = Down
				pos.Y++
			}
		}
	}

	PrintGrid(grid, Point{X:-4, Y:-4}, Point{X:4, Y:4})
	fmt.Println(infectionCount)
}

func PrintGrid(grid map[Point]bool, topLeft Point, bottomRight Point) {
	for y := topLeft.Y; y <= bottomRight.Y; y++ {
		for x := topLeft.X; x <= bottomRight.X; x++ {
			if grid[Point{X:x, Y:y}] {
				fmt.Print("#")
			} else {
				fmt.Print(".")
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

func GetInput() map[Point]bool {
	file, err := os.Open("Day22.txt")
	if err != nil {
		log.Fatal("Could not open input file.")
	}

	scanner := bufio.NewScanner(file)
	input := []string {}

	for scanner.Scan() {
		input = append(input, scanner.Text())
	}

	result := map[Point]bool {}
	y := -len(input)/2
	for _, line := range input {
		x := -len(line)/2
		for _, data := range line {
			if data == '#' {
				result[Point {X:x, Y:y}] = true
			}
			x++
		}
		y++
	}

	return result
}