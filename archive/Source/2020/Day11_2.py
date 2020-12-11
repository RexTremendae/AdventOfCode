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
    count = 0
    for xi in range(x-1, x+2):
        for yi in range(y-1, y+2):
            if (xi == x and yi == y): continue
            nextSeat = getNextVisibleSeat(map, x, y, (xi - x), (yi - y))
            if (nextSeat == '#'): count += 1
    return count

def getNextVisibleSeat (map, x, y, xOff, yOff):
    xMax = len(map[0])
    yMax = len(map)

    while (True):
        x += xOff
        y += yOff
        if (x < 0 or y < 0 or x >= xMax or y >= yMax): return '.'
        if (map[y][x] != '.'): return map[y][x]

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
            elif (lastState == '#' and occupiedCount >= 5):
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
