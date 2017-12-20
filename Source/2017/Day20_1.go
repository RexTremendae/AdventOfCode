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
    particles := GetInput()

    lastMinDistance := int64(0)
    lastMinDistanceParticle := 0

    for i := 0; i < 150000; i++ {
        minAccDistance := int64(-1)
        //minAccDistanceParticle := -1

        minDistance := int64(-1)
        minDistanceParticle := -1

        for pidx, part := range particles {
            part.Vel.X += part.Acc.X
            part.Vel.Y += part.Acc.Y
            part.Vel.Z += part.Acc.Z

            part.Pos.X += part.Vel.X
            part.Pos.Y += part.Vel.Y
            part.Pos.Z += part.Vel.Z

            accDistance := CalculateDistance(*(part.Acc))
            distance := CalculateDistance(*(part.Pos))

            if minDistance < 0 || distance < minDistance {
                minDistance = distance
                minDistanceParticle = pidx

                lastMinDistance = minDistance
                lastMinDistanceParticle = minDistanceParticle
            }

            if minAccDistance < 0 || accDistance < minAccDistance {
                minAccDistance = accDistance
                //minAccDistanceParticle = pidx
            }
        }

        //fmt.Printf("%d: %d\n", i, minDistance)
        //PrintParticles(particles)
        //fmt.Printf("DIST: Particle %d (dist %d)\n", minDistanceParticle, minDistance)
        //fmt.Printf("ACCDIST: Particle %d (dist %d)\n", minAccDistanceParticle, minAccDistance)
    }

    fmt.Printf("DIST: Particle %d (dist %d)\n", lastMinDistanceParticle, lastMinDistance)
    //fmt.Printf("ACCDIST: Particle %d (dist %d)\n", minAccDistanceParticle, minAccDistance)
    PrintParticle(*particles[4])
    fmt.Println(CalculateDistance(*particles[4].Pos))
}

func CalculateDistance(point Point3D) int64 {
    absX := Abs(point.X)
    absY := Abs(point.Y)
    absZ := Abs(point.Z)
    dist := absX + absY + absZ

    if dist < absX || dist < absY || dist < absZ { log.Fatal("OVERFLOW!!!") }
    return dist
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
