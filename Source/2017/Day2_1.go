package main

import (
	"bufio"
	"fmt"
	"log"
	"os"
	"strconv"
	"strings"
)

func main() {
	file, err := os.Open("input.txt")
    if err != nil {
        log.Fatal(err)
    }
    defer file.Close()
	checksum := 0
	scanner := bufio.NewScanner(file)
    for scanner.Scan() {
		max, min := 0, 1000000
		for _, curr := range strings.Split(scanner.Text(), "\t") {
			val, err := strconv.Atoi(curr)
			if err != nil {
				log.Fatal(err)
			}

			if (max < val) { max = val }
			if (min > val) { min = val }
		}

		checksum += max - min
    }

	defer fmt.Println(checksum)

	if err := scanner.Err(); err != nil {
        log.Fatal(err)
	}
}