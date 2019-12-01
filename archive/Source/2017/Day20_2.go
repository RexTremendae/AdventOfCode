package main

import (
	"strconv"
    "bufio"
    "fmt"
    "log"
    "os"
    "strings"
)

type Point3D struct {
    X int64
    Y int64
    Z int64
}

type Particle struct {
    Pos *Point3D
    Vel *Point3D
    Acc *Point3D
}

func main() {
    particlesList := GetInput()

    particlesMap := map[*Particle]bool {}
    for _, p := range particlesList {
        particlesMap[p] = true
    }

    lastCount := len(particlesMap)
    fmt.Println(lastCount)
    ResolveCollisions(&particlesMap)
    for i := 0; i < 500000; i++ {
        for part, _ := range particlesMap {
            part.Vel.X += part.Acc.X
            part.Vel.Y += part.Acc.Y
            part.Vel.Z += part.Acc.Z

            part.Pos.X += part.Vel.X
            part.Pos.Y += part.Vel.Y
            part.Pos.Z += part.Vel.Z
        }

        ResolveCollisions(&particlesMap)
        count := len(particlesMap)
        if count != lastCount {
            lastCount = count
            fmt.Println(count)
        }
    }
    fmt.Println(len(particlesMap))
}

func ResolveCollisions(particles *map[*Particle]bool) {
    samePlace := map[Point3D]([]*Particle) {}

    for part, _ := range *particles {
        if list, ok := samePlace[*part.Pos]; ok {
            samePlace[*part.Pos] = append(list, part)
        } else {
            samePlace[*part.Pos] = []*Particle{ part }
        }
    }

    for _, value := range samePlace {
        if len(value) > 1 {
            for _, toDelete := range value {
                delete(*particles, toDelete)
            }
        }
    }
}

func Abs(value int64) int64 {
    if value < 0 { return -value }
    return value
}

func PrintParticle(particle Particle) {
    fmt.Printf("P: (%d, %d, %d) ", particle.Pos.X, particle.Pos.Y, particle.Pos.Z)
    fmt.Printf("V: (%d, %d, %d) ", particle.Vel.X, particle.Vel.Y, particle.Vel.Z)
    fmt.Printf("A: (%d, %d, %d) ", particle.Acc.X, particle.Acc.Y, particle.Acc.Z)
    fmt.Println()
}

func PrintParticles(particles [](*Particle)) {
    for _, part := range particles {
        PrintParticle(*part)
    }
    fmt.Println()
}

func GetInput() []*Particle {
    file, err := os.Open("Day20.txt")
    if err != nil { log.Fatal("Input file not found") }
    defer file.Close()

    particleList := [](*Particle) { }

    scanner := bufio.NewScanner(file)
    for scanner.Scan() {
        particle := Particle {}
        split := strings.Split(scanner.Text(), ", ")

        for _, data := range split {
            split := strings.Split(data, "=")
            coords := strings.Split(split[1][1:len(split[1])-1], ",")
            x, err := strconv.Atoi(strings.TrimSpace(coords[0]))
            if err != nil { log.Fatal("Invalid input: " + scanner.Text() + ", specifically " + coords[0]) }
            y, err := strconv.Atoi(strings.TrimSpace(coords[1]))
            if err != nil { log.Fatal("Invalid input: " + scanner.Text() + ", specifically " + coords[1]) }
            z, err := strconv.Atoi(strings.TrimSpace(coords[2]))
            if err != nil { log.Fatal("Invalid input: " + scanner.Text() + ", specifically " + coords[2]) }

            p := Point3D { X: int64(x), Y: int64(y), Z: int64(z) }
            switch split[0] {
                case "p": particle.Pos = &p
                case "v": particle.Vel = &p
                case "a": particle.Acc = &p
            }
        }

        particleList = append(particleList, &particle)
    }

    return particleList
}
