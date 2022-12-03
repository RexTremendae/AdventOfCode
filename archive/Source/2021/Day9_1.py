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

sum = 0

for y in range(maxY+1):
  for x in range(maxX+1):
    d = data[(x, y)]
    low = True
    for n in [(x-1, y), (x, y-1), (x+1, y), (x, y+1)]:
      if n in data and data[n] <= d:
        low = False
        break
    if not low: continue
    sum += d + 1

print (sum)
