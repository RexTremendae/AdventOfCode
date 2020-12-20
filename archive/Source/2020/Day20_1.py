file = open("Day20.txt")

index = 0
edges = []
edge1 = ""
edge2 = ""
tileId = 0

tiles = []

for line in map(lambda l: l[:-1], file):
    tileRow = index % 12
    if tileRow == 0:
        tileId = int(line[:-1].split(' ')[1])
    elif tileRow == 11:
        edges.append(edge1)
        edges.append(edge2)
        tiles.append((tileId, edges))
        edges = []
        edge1 = ""
        edge2 = ""
    else:
        if tileRow == 1 or tileRow == 10:
            edges.append(line)
        edge1 += line[0]
        edge2 += line[9]
    index += 1

if len(edges) > 0:
    if len (edges) < 4:
        edges.append(edge1)
        edges.append(edge2)
    tiles.append((tileId, edges))

for tile in tiles:
    print (f"{tile[0]}: {tile[1]}")
print()

matches = {}

for t1 in tiles:
    for e1 in t1[1]:
        for t2 in tiles:
            for e2 in t2[1]:
                tile1 = t1[0]
                tile2 = t2[0]
                if tile1 == tile2: continue
                if e1 == e2 or e1 == e2[::-1]:
                    if tile1 not in matches:
                        matches[tile1] = []
                    if tile2 not in matches[tile1]:
                        matches[tile1].append(tile2)
                    if tile2 not in matches:
                        matches[tile2] = []
                    if tile1 not in matches[tile2]:
                        matches[tile2].append(tile1)

answer = 1
for m in matches:
    if len(matches[m]) != 2: continue
    print (f"{m}: {matches[m]}")
    answer *= m

print ()
print (answer)
