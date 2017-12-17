package main

import (
	"fmt"
)

func main() {
	steps := 380
	maxLen := 2018
	data := make([]int, 1, maxLen)
	debug := false
	idx := 0

	if (debug) { fmt.Println("(0)") }
	for i := 1; i < maxLen; i++ {
		before := len(data)
		after := before + 1
		data = append(data, 0)

		idx = (idx + steps) % before
		idx++
		for j := after-1; j > idx; j-- {
			data[j] = data[j-1]
		}

		data[idx] = i
		if (debug) {
			for j := 0; j < after; j++ {
				if j == idx {
					fmt.Printf("(%d) ", data[j])
				} else {
					fmt.Printf("%d ", data[j])
				}
			}
			fmt.Println()
		}
	}

	for i := 0; i < len(data); i++ {
		if data[i] == maxLen-1 {
			i++
			fmt.Println(data[i])
			break
		}
	}
}
