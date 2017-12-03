package main

import (
    "bufio"
    "fmt"
    "math"
    "os"
    "strconv"
)

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
        x, y := 0, 0

        for goal > 1 {
            x += xΔ
            y += yΔ

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

            goal --
        }

        dist := math.Abs(float64(x)) + math.Abs(float64(y))
        fmt.Printf("Dist: %d\n", int64(dist))
    }
}