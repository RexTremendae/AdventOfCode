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
            for w in range(-1, 2):
                if (x == y == z == w == 0): continue
                neighborCoords.append((x,y,z,w))

def countNeighbors(pos):
    count = 0
    for neigh in neighborCoords:
        neighPos = (neigh[0]+pos[0], neigh[1]+pos[1], neigh[2]+pos[2], neigh[3]+pos[3])
        if (neighPos in state):
            count += 1 if state[neighPos] == '#' else 0
    return count

for y,line in enumerate(input):
    for x,activity in enumerate(line):
        state[(x,y,0,0)] = activity

nextCount = 0

minX = 0
minY = 0
minZ = 0
minW = 0
maxX = len(input[0])-1
maxY = len(input)-1
maxZ = 0
maxW = 0

for cycle in range(6):
    nextCount = 0
    nextState = {}

    for x in range(minX - 1, maxX + 2):
        for y in range(minY - 1, maxY + 2):
            for z in range(minZ - 1, maxZ + 2):
                for w in range(minW - 1, maxW + 2):
                    cube = '.'
                    if (x,y,z,w) in state: cube = state[(x,y,z,w)]

                    n = countNeighbors((x,y,z,w))

                    if cube == '.' and n == 3:
                        nextState[(x,y,z,w)] = '#'
                        nextCount += 1
                    elif cube == '#' and (n == 2 or n == 3):
                        nextCount += 1
                        nextState[(x,y,z,w)] = '#'

    print (nextCount)
    state = nextState

    maxX += 1
    maxY += 1
    maxZ += 1
    maxW += 1
    minX -= 1
    minY -= 1
    minZ -= 1
    minW -= 1
