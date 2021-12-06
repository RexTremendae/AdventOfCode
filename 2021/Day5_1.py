file = open("Day5.txt", "r")

pts = {}

for line in file:
  line = line.rstrip()
  parts = line.split('->')
  start = parts[0].split(',')
  end = parts[1].split(',')
  x1 = int(start[0])
  x2 = int(end[0])
  y1 = int(start[1])
  y2 = int(end[1])

  if x1 == x2:
    for y in range(min(y1, y2), max(y1, y2)+1):
      if (x1, y) in pts:
        pts[(x1, y)] += 1
      else:
        pts[(x1, y)] = 1
  elif y1 == y2:
    for x in range(min(x1, x2), max(x1, x2)+1):
      if (x, y1) in pts:
        pts[(x, y1)] += 1
      else:
        pts[(x, y1)] = 1

print (len([cnt for (_,_),cnt in pts.items() if cnt > 1]))
