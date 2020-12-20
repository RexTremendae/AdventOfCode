file = open("Day20.txt")

index = 0
tileId = 0
tileRows = []
tiles = {}

for line in map(lambda l: l[:-1], file):
    tileRow = index % 12
    if tileRow == 0:
        tileId = int(line[:-1].split(' ')[1])
    elif tileRow == 11:
        tiles[tileId] = tileRows
        tileRows = []
    else:
        tileRows.append(line)
    index += 1

if len(tileRows) > 0:
    tiles[tileId] = tileRows


def getEdges(tile):
    edges = []
    edgeL = ""
    edgeR = ""
    for r in tile:
        edgeL += r[0]
        edgeR += r[-1:][0]

    edges.append(tile[0])
    edges.append(edgeR)
    edges.append(tile[-1:][0][::-1])
    edges.append(edgeL[::-1])
    return edges

def rotateRight(tile):
    rotated = []
    for x in range(len(tile[0])):
        rotated.append("")
        for y in range(len(tile)):
            rotated[x] = rotated[x] + tile[len(tile)-y-1][x]
    return rotated

def rotateLeft(tile):
    rotated = []
    ySize = len(tile)
    xSize = len(tile[0])
    for y in range(xSize):
        rotated.append("")
        for x in range(ySize):
            #print(f"{x}, {y}")
            rotated[y] = rotated[y] + tile[x][xSize-y-1]
    return rotated

def flipVertical(tile):
    flipped = []
    for row in tile[::-1]:
        flipped.append(row)
    return flipped

def flipHorizontal(tile):
    flipped = []
    for row in tile:
        flipped.append(row[::-1])
    return flipped

def removeEdges(tile):
    noEdges = []
    for row in tile[1:-1]:
        noEdges.append(row[1:-1])
    return noEdges

#for tid in tiles:
#    print (f"{tid}: {getEdges(tiles[tid])}")
#print()

matches = {}

for t1id in tiles:
    tile1 = tiles[t1id]
    for e1i, e1 in enumerate(getEdges(tile1)):
        for t2id in tiles:
            tile2 = tiles[t2id]
            for e2i, e2 in enumerate(getEdges(tile2)):
                if t1id == t2id: continue
                if e1 == e2 or e1 == e2[::-1]:
                    if t1id not in matches:
                        matches[t1id] = {}
                    if t2id not in matches[t1id]:
                        matches[t1id][t2id] = (e1i, e2i)
                    if t2id not in matches:
                        matches[t2id] = {}
                    if t1id not in matches[t2id]:
                        matches[t2id][t1id] = (e2i, e1i)

matches2 = [m for m in matches if len(matches[m]) == 2]
matches3 = [m for m in matches if len(matches[m]) == 3]
matches4 = [m for m in matches if len(matches[m]) == 4]

keys = [m for m in matches]
keys.sort()

#for m in keys:
#    print (f"{m}: {matches[m]}")

#print()


fullMap = []

# Puzzle input starting values (should be easy enough to figure out, but I cheated a small bit
# and picked and oriented a starting corner tile manually)
##############################
rowStartTileId = 2897
rowStartMatchEdge = 1
flipRowEdge = True
##############################

# Small example staring values
##############################
#rowStartTileId = 1951
#rowStartMatchEdge = 2
#flipRowEdge = True
##############################

colTileId = 0
colMatchEdge = 0
rowIndex = 0
flipColEdge = False

while (True):
    if colTileId > 0:
        flipIndication = '(flipped)' if flipColEdge else ''
        #print (f"         #{colTileId}, edge {colMatchEdge} {flipIndication}")
        tile = tiles[colTileId]
        for r in range((colMatchEdge+1) % 4):
            tile = rotateLeft(tile)

        if flipColEdge:
            tile = flipVertical(tile)

        edgeToMatch = (colMatchEdge + 2) % 4
        edgeToMatchData = getEdges(tile)[1]

        tile = removeEdges(tile)
        #colStartRow = rowIndex - 1 - len(tile)
        colStartRow = rowIndex - len(tile)
        for y in range(len(tile)):
            fullMap[colStartRow + y] = fullMap[colStartRow + y] + tile[y]
            #fullMap[colStartRow + y] = fullMap[colStartRow + y] + " " + tile[y]

        nextColTileId = 0
        nextColMatchEdge = 0
        for match in matches[colTileId]:
            edgeMatch = matches[colTileId][match]
            if edgeMatch[0] == edgeToMatch:
                nextColMatchEdge = edgeMatch[1]
                nextColTileId = match
        colTileId = nextColTileId
        colMatchEdge = nextColMatchEdge
        if colTileId > 0:
            flipColEdge = edgeToMatchData == getEdges(tiles[colTileId])[colMatchEdge]

    elif rowStartTileId > 0:
        #print()
        flipIndication = '(flipped)' if flipRowEdge else ''
        #print (f"New row: #{rowStartTileId}, edge {rowStartMatchEdge} {flipIndication}")
        tile = tiles[rowStartTileId]
        for r in range(rowStartMatchEdge):
            tile = rotateLeft(tile)

        if flipRowEdge:
            tile = flipHorizontal(tile)

        for row in removeEdges(tile):
            fullMap.append(row)
            rowIndex += 1
        #fullMap.append("")
        #rowIndex += 1

        nextRowStartTileId = 0
        nextRowStartMatchEdge = 0

        colEdgeToMatch = ((rowStartMatchEdge + (3 if flipRowEdge else 1))) % 4
        rowEdgeToMatch = (rowStartMatchEdge + 2) % 4
        for match in matches[rowStartTileId]:
            edgeMatch = matches[rowStartTileId][match]
            if edgeMatch[0] == rowEdgeToMatch:
                nextRowStartMatchEdge = edgeMatch[1]
                nextRowStartTileId = match
                if nextRowStartTileId > 0:
                    flipRowEdge = getEdges(tile)[2] == getEdges(tiles[nextRowStartTileId])[nextRowStartMatchEdge]
            elif edgeMatch[0] == colEdgeToMatch:
                colMatchEdge = edgeMatch[1]
                colTileId = match
                if colTileId > 0:
                    flipColEdge = getEdges(tile)[1] == getEdges(tiles[colTileId])[colMatchEdge]

        rowStartTileId = nextRowStartTileId
        rowStartMatchEdge = nextRowStartMatchEdge

    else: break


#print()
#for row in fullMap:
#    print(row)


originalSeaMonster = [
    "                  # ",
    "#    ##    ##    ###",
    " #  #  #  #  #  #   "
]

monsterSize = len(originalSeaMonster[0])

allSeaMonsters = [originalSeaMonster, flipHorizontal(originalSeaMonster)]
for monster in allSeaMonsters[:]:
    allSeaMonsters.append(flipVertical(monster))
for monster in allSeaMonsters[:]:
    allSeaMonsters.append(rotateRight(monster))


monsters = []

for y in range(len(fullMap[0])-2):
    for x in range(len(fullMap)-2):
        monstersLeft = {}
        idx = 0
        for monster in allSeaMonsters:
            if len(monster[0])+x < len(fullMap[0]) and len(monster)+y < len(fullMap):
                monstersLeft[idx] = monster
            idx += 1

        for yy in range(monsterSize):
            for xx in range(monsterSize):
                keys = []
                for idx in monstersLeft.keys():
                    keys.append(idx)
                if len(keys) == 0: break
                for idx in keys:
                    monster = monstersLeft[idx]
                    if xx >= len(monster[0]) or yy >= len(monster):
                        continue
                    if xx+x >= len(fullMap[0]) or yy+y >= len(fullMap):
                        continue
                    if monster[yy][xx] != '#':
                        continue
                    #print(f"(x,y): ({x}, {y}) (xx, yy): ({xx}, {yy}): {fullMap[yy+y][xx+x]}")
                    if fullMap[yy+y][xx+x] != '#':
                        del monstersLeft[idx]
        if idx in monstersLeft:
            monsters.append((x, y, monstersLeft[idx]))
        if len(monstersLeft) > 0:
            print (f"{x},{y}: {len(monstersLeft)}")
            #for m in monstersLeft:
                #monster = monstersLeft[m]
                #for yy in range(len(monster)):
                    #print (fullMap[y+yy][x:x+len(monster[0])], end="  ")
                    #print (monster[yy])

            #print()
            #for yy in range(monsterSize):
                #for xx in range(monsterSize):
                #print (fullMap[y+yy][x:x+monsterSize])
            #exit()

for m in monsters:
    print (m)

nonMonsterCount = 0
for y in range(len(fullMap)):
    for x in range(len(fullMap[0])):
        if fullMap[y][x] == '#':
            isMonster = False
            for mx, my, m in monsters:
                if y >= my and x >= mx and y-my < len(m) and x-mx < len(m[0]) and m[y-my][x-mx] == '#':
                    isMonster = True
                    break
            nonMonsterCount += 0 if isMonster else 1

print (nonMonsterCount)
