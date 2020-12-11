file = open("Day11.txt")

map = []
for line in file:
    map.append(line[:-1])

visited = set()

def printMap(map):
    for y in range(len(map)):
        for x in range(len(map[y])):
            print(map[y][x],end="")
        print()

def countOccupiedNeighbors(map, x, y):
    maxX = len(map[0])
    maxY = len(map)

    count = 0
    for xi in range(x-1, x+2):
        for yi in range(y-1, y+2):
            if (xi == x and yi == y): continue
            if (xi < 0 or yi < 0): continue
            if (xi >= maxX or yi >= maxY): continue
            if (map[yi][xi] == '#'): count += 1
    return count

def iterate(map):
    newMap = []
    for y in range(len(map)):
        newRow = ""
        for x in range(len(map[y])):
            lastState = map[y][x]
            if lastState == '.':
                newRow += '.'
                continue

            occupiedCount = countOccupiedNeighbors(map, x, y)
            if (lastState == 'L' and occupiedCount == 0):
                newRow += '#'
            elif (lastState == '#' and occupiedCount >= 4):
                newRow += 'L'
            else:
                newRow += lastState
        newMap.append(newRow)
    return newMap

visited = set()
count = 0

lastVisitedCount = -1
while (lastVisitedCount != len(visited)):
    map = iterate(map)
    lastVisitedCount = len(visited)
    visited.add(''.join(map))

    #printMap(map)
    #print()

print(''.join(map).count('#'))
