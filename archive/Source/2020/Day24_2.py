validDirections = ['e', 'w', 'ne', 'nw', 'se', 'sw']

def move(pos, direction):
    if direction not in validDirections: return None

    x = pos[0]
    y = pos[1]

    if direction == 'e': return (x+1, y)
    elif direction == 'w': return (x-1, y)

    newX = x
    newY = y

    if direction.startswith('s'):
        newY += 1
        direction = direction[1]
    elif direction.startswith('n'):
        newY -= 1
        direction = direction[1]

    if direction == 'e' and y % 2 != 0:
        newX += 1
    elif direction == 'w' and y % 2 == 0:
        newX -= 1

    return (newX, newY)

tiles = {}

file = open("Day24.txt")
minX = 100
minY = 100
maxX = 0
maxY = 0

for movements in [l[:-1] for l in file]:
    pos = (0, 0)
    idx = 0

    while idx < len(movements):
        mv = movements[idx]
        if (mv == 's' or mv == 'n'):
            idx += 1
            mv += movements[idx]

        pos = move(pos, mv)
        idx += 1

    if pos not in tiles: tiles[pos] = False
    tiles[pos] = not tiles[pos]

    if pos[0] < minX: minX = pos[0]
    if pos[1] < minY: minY = pos[1]
    if pos[0] > maxX: maxX = pos[0]
    if pos[1] > maxY: maxY = pos[1]

minX -= 1
minY -= 1
maxX += 1
maxY += 1

count = 0
for t in tiles:
    if tiles[t]: count += 1

for i in range(100):
    nextTiles = {}
    for y in range(minY, maxY+1):
        for x in range(minX, maxX+1):
            pos = (x,y)
            neigh = 0
            for mv in validDirections:
                newPos = move(pos, mv)
                if newPos in tiles and tiles[newPos]: neigh += 1

            tileValue = tiles[pos] if pos in tiles else False
            if tileValue and (neigh == 0 or neigh > 2):
                nextTiles[pos] = False
            elif not tileValue and neigh == 2:
                nextTiles[pos] = True
            elif tileValue: nextTiles[pos] = True

    tiles = nextTiles

    maxX += 1
    maxY += 1
    minX -= 1
    minY -= 1

count = 0
for t in tiles:
    if tiles[t]: count += 1
print(count)
