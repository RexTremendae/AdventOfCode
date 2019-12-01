package main

import (
	"bufio"
	"fmt"
	"log"
	"os"
	"strconv"
	"strings"
)

type Move struct {
	MoveType string
	Position1 int
	Position2 int
	Value1 int
	Value2 int
}

func main() {
	//Test()
	//return

	// Puzzle input
	len := 16
	input := GetInput()
	repeat := 1000000000

	// Small example
	//len := 5
	//input := "s1,x3/4,pe/b"
	//repeat := 2

	dancers := CreateDancers(len)
	moves := ParseInput(input)
	visited := make(map[string]bool)
	firstVisited := 0
	for ; firstVisited < repeat; firstVisited++ {
		Dance(dancers, moves)
		formation := FormatDancers(dancers)
		if _, ok := visited[formation]; ok {
			break;
		}
		visited[formation] = true
	}

	prev := ""
	for i := 0; i < repeat % firstVisited; i++ {
		prev = FormatDancers(dancers)
		Dance(dancers, moves)
	}

	current := FormatDancers(dancers)
	Dance(dancers, moves)
	next := FormatDancers(dancers)

	fmt.Printf("Prev: %s\n", prev) // Right answer!
	fmt.Printf("Current: %s\n", current)
	fmt.Printf("Next: %s\n", next)
}

func Test() {
	fmt.Println("Testing spin...")
	dcrs := CreateDancers(5)
	mvs := ParseInput("s0,s1,s2,s3,s4")
	for i := 0; i < len(*mvs); i++ {
		Dance(dcrs, &[]Move {(*mvs)[i]})
		PrintForward(dcrs)
	}

	fmt.Println("===============")

	fmt.Println("Testing exchange...")
	dcrs = CreateDancers(5)
	mvs = ParseInput("x0/1,x0/2,x0/3,x0/4,x1/2,x1/4,x3/4")
	for i := 0; i < len(*mvs); i++ {
		Dance(dcrs, &[]Move {(*mvs)[i]})
		PrintForward(dcrs)
	}

	fmt.Println("===============")
	
	fmt.Println("Testing partner...")
	dcrs = CreateDancers(5)
	mvs = ParseInput("pa/b,pa/c,pa/d,pb/e,pa/b")
	for i := 0; i < len(*mvs); i++ {
		Dance(dcrs, &[]Move {(*mvs)[i]})
		PrintForward(dcrs)
	}

}

func FormatThousandSeparated(i int) string {
	result := ""
	for ;i >= 1000;i/=1000{
		part := i % 1000
		strPart := strconv.Itoa(part)
		for ;len(strPart) < 3; {
			strPart = "0" + strPart
		}
		result = strPart + " " + result
	}

	result = strconv.Itoa(i) + " " + result
	return strings.TrimSpace(result)
}

func Dance(dancers *[]int, moves *([]Move)) {
	for _, step := range *moves {
		switch step.MoveType {
		case "s":
			split := step.Position1
			firstHalf := make([]int, len(*dancers)-split)
			fhlen := len(firstHalf)
			copy(firstHalf, (*dancers)[:fhlen])
			for i := 0; i < split; i++ {
				(*dancers)[i] = (*dancers)[fhlen+i]
			}
			for i := 0; i < fhlen; i++ {
				(*dancers)[split+i] = firstHalf[i]
			}
		case "x":
			pos1 := step.Position1
			pos2 := step.Position2
			tmp := (*dancers)[pos2]
			(*dancers)[pos2] = (*dancers)[pos1]
			(*dancers)[pos1] = tmp
		case "p":
			i, pos1, pos2 := 0, 0, 0
			for i = 0; (*dancers)[i] != step.Value1; i++ { }
			pos1 = i
			for i = 0; (*dancers)[i] != step.Value2; i++ { }
			pos2 = i

			tmp := (*dancers)[pos2]
			(*dancers)[pos2] = (*dancers)[pos1]
			(*dancers)[pos1] = tmp
		default:
			log.Fatal("Invalid dance move: " + step.MoveType)
		}
	}
}

func ParseInput(input string) *([]Move) {
	danceSteps := strings.Split(input, ",")
	moves := make([]Move, len(danceSteps))

	for idx, step := range danceSteps {
		switch step[0] {
		case 's':
			movement, err := strconv.Atoi(step[1:])
			if err != nil {
				log.Fatal("Invalid dance move: " + step)				
			}
			moves[idx] = Move { MoveType: "s", Position1: movement }
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
			moves[idx] = Move { MoveType: "x", Position1: pos1, Position2: pos2 }
		case 'p':
			partners := strings.Split(step[1:], "/")
			moves[idx] = Move { MoveType: "p", Value1: int(partners[0][0] - 'a'), Value2: int(partners[1][0] - 'a') }
		default:
			log.Fatal("Invalid dance move: " + step)
		}
	}

	return &moves
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

func FormatDancers(dancers *[]int) string {
	result := ""
	for i := 0; i < len(*dancers); i ++ {
		result += string((*dancers)[i]+'a')
	}

	return result
}

func PrintForward(dancers *[]int) {
	for i := 0; i < len(*dancers); i ++ {
		fmt.Printf("%s", string((*dancers)[i]+'a'))
	}
	fmt.Println()
}

func CreateDancers(len int) *[]int {
	result := make([]int, len)
	for i := 1; i < len; i++ {
		result[i] = i
	}
	return &result
}