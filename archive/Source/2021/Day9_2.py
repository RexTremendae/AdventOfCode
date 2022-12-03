file = open("Day9.txt", "r")

data = {}
y = 0
maxX = 0
maxY = 0

for line in file.readlines():
  x = 0
  for h in [int(h) for h in line.rstrip()]:
    data[(x, y)] = h
    maxX = max(x, maxX)
    maxY = max(y, maxY)
    x += 1
  y += 1

low_points = []

for y in range(maxY+1):
  for x in range(maxX+1):
    d = data[(x, y)]
    low = True
    for n in [(x-1, y), (x, y-1), (x+1, y), (x, y+1)]:
      if n in data and data[n] <= d:
        low = False
        break
    if not low: continue
    low_points.append((x, y))

visited = set()
basins = []

for (x, y) in low_points:
  q = []
  q.append((x,y))
  sze = 0
  while len(q) > 0:
    (x, y) = q[0]
    q.pop(0)
    if (x, y) in visited: continue
    visited.add((x, y))
    sze += 1
    for (nx, ny) in [(x-1, y), (x, y-1), (x+1, y), (x, y+1)]:
      if (nx, ny) in visited: continue
      if nx < 0 or ny < 0 or nx > maxX or ny > maxY: continue
      if data[(nx, ny)] == 9: continue
      q.append((nx, ny))
  basins.append(sze)

total = 1
for b in (sorted(basins)[-3:]): total *= b

print (total)
