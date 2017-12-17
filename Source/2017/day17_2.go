package main

import (
	"fmt"
)

func main() {
	steps := 380
	maxLen := 50000000
	maxLen++
	data := make([]int, 2)
	debug := false
	idx := 0

	if (debug) { fmt.Println("(0)") }
	len := 1
	for i := 1; i < maxLen; i++ {
		idx = (idx + steps) % len
		idx++
		if (idx == 1) { data[idx] = i }
		if (debug) {
			for j := 0; j < 2; j++ {
				if j == idx {
					fmt.Printf("(%d) ", data[j])
				} else {
					fmt.Printf("%d ", data[j])
				}
			}
			fmt.Println()
		}
		len++
	}

	fmt.Println(data[1])
}
