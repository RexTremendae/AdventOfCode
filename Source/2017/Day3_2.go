package main

import (
    "bufio"
    "fmt"
    "os"
    "strconv"
)

type Point struct {
    x int
    y int
}

func main() {
    scanner := bufio.NewScanner(os.Stdin)

    for scanner.Scan() {
        goal, err := strconv.Atoi(scanner.Text())
        if err != nil {
            fmt.Println("Erroneous input")
            continue
        }

        stepsReach, stepsTrigger, stepsChange := 1, 0, 0
        xΔ, yΔ := 1, 0
        p := Point {0, 0}
        value := 0
        data := map[Point]int{{0, 0}: 1}

        for value <= goal {
            p.x += xΔ
            p.y += yΔ

            stepsTrigger ++
            if (stepsTrigger >= stepsReach) {
                switch {
                case xΔ > 0:
                    xΔ = 0
                    yΔ = -1
                case xΔ < 0:
                    xΔ = 0
                    yΔ = 1
                case yΔ > 0:
                    xΔ = 1
                    yΔ = 0
                case yΔ < 0:
                    xΔ = -1
                    yΔ = 0
                }

                stepsTrigger = 0
                stepsChange ++
                if stepsChange >= 2 {
                    stepsChange = 0
                    stepsReach ++
                }
            }

            value = 0
            value += data[Point{p.x-1, p.y-1}]
            value += data[Point{p.x+1, p.y-1}]
            value += data[Point{p.x-1, p.y}]
            value += data[Point{p.x+1, p.y}]
            value += data[Point{p.x-1, p.y+1}]
            value += data[Point{p.x+1, p.y+1}]
            value += data[Point{p.x, p.y-1}]
            value += data[Point{p.x, p.y+1}]
    
            data[p] = value
        }

        fmt.Println(value)
    }
}