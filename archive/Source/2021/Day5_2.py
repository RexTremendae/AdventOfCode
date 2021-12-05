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

  x = x1
  y = y1

  while x != x2 or y != y2:
    if (x, y) in pts:
      pts[(x, y)] += 1
    else:
      pts[(x, y)] = 1
    x += 1 if x < x2 else -1 if x > x2 else 0
    y += 1 if y < y2 else -1 if y > y2 else 0

  if (x, y) in pts:
    pts[(x, y)] += 1
  else:
    pts[(x, y)] = 1

print (len([cnt for (_,_),cnt in pts.items() if cnt > 1]))
