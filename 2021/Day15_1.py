import sys

def main():
  map = read_data()
  print (dijkstra(map))


def dijkstra(map):
  visited = []
  distances = []
  for row in map:
    visited.append([False] * len(row))
    distances.append([sys.maxsize] * len(row))

  visited[0][0] = True
  distances[0][0] = 0

  q = set()
  q.add((0, 0))

  connections = {}

  width = len(map[0])
  height = len(map)

  x = 0
  y = 0

  while len(q) > 0 and (x != width-1 or y != height-1):
    min_cx = sys.maxsize
    min_cy = sys.maxsize
    min_cw = sys.maxsize

    for (cx, cy) in q:
      cw = distances[cy][cx]
      if cw < min_cw:
        min_cx = cx
        min_cy = cy
        min_cw = cw

    x = min_cx
    y = min_cy
    d = min_cw

    if x == sys.maxsize or y == sys.maxsize:
      print (q)
    q.remove((x, y))

    visited[y][x] = True

    min_cx = sys.maxsize
    min_cy = sys.maxsize
    min_cw = sys.maxsize

    candidates = [(x-1, y), (x, y-1), (x+1, y), (x, y+1)]

    for (cx, cy) in candidates:
      if (cx < 0 or cy < 0 or cx >= width or cy >= height): continue
      if d + map[cy][cx] < distances[cy][cx]:
        distances[cy][cx] = d + map[cy][cx]

      if not visited[cy][cx]:
        q.add((cx, cy))
        if distances[cy][cx] < min_cw:
          min_cx = cx
          min_cy = cy
          min_cw = distances[cy][cx]

    connections[(x, y)] = (min_cx, min_cy)

  x = width-1
  y = height-1

  return distances[y][x]


def read_data():
  data = []
  file = open('Day15.txt', 'r')
  for line in [line.rstrip() for line in file.readlines()]:
    if line == '': break
    data.append([int(x) for x in line])
  return data


main()
