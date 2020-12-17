# Small example
input = [
'.#.',
'..#',
'###']

# Puzzle input
input = [
"##....#.",
"#.#..#..",
"...#....",
"...#.#..",
"###....#",
"#.#....#",
".#....##",
".#.###.#"]

state = {}

neighborCoords = []

for x in range(-1, 2):
    for y in range(-1, 2):
        for z in range(-1, 2):
            if (x == y == z == 0): continue
            neighborCoords.append((x,y,z))

def countNeighbors(pos):
    count = 0
    for neigh in neighborCoords:
        neighPos = (neigh[0]+pos[0], neigh[1]+pos[1], neigh[2]+pos[2])
        if (neighPos in state):
            count += 1 if state[neighPos] == '#' else 0
    return count

for y,line in enumerate(input):
    for x,activity in enumerate(line):
        state[(x,y,0)] = activity

nextCount = 0

minX = 0
minY = 0
minZ = 0
maxX = len(input[0])-1
maxY = len(input)-1
maxZ = 0

for cycle in range(6):
    nextCount = 0
    nextState = {}

    for x in range(minX - 1, maxX + 2):
        for y in range(minY - 1, maxY + 2):
            for z in range(minZ - 1, maxZ + 2):
                cube = '.'
                if (x,y,z) in state: cube = state[(x,y,z)]

                n = countNeighbors((x,y,z))

                if cube == '.' and n == 3:
                    nextState[(x,y,z)] = '#'
                    nextCount += 1
                elif cube == '#' and (n == 2 or n == 3):
                    nextCount += 1
                    nextState[(x,y,z)] = '#'

    print (nextCount)
    state = nextState

    maxX += 1
    maxY += 1
    maxZ += 1
    minX -= 1
    minY -= 1
    minZ -= 1

exit()

# Print last state
for z in range(minZ-1, maxZ+2):
    print (f"Z: {z}")
    for y in range(minY-1, maxY+2):
        for x in range(minX-1, maxX+2):
            cube = '.'
            if (x,y,z) in state:
                cube = state[(x,y,z)]
            print (cube, end=" ")
        print()
    print()

