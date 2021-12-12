def main():
  map = get_map(puzzle_input)

  path_count = 0

  q = []
  q.append(('start', None, ['start'], []))

  while len(q) > 0:
    (pos, twice_cave, visited, path) = q[0]
    q.pop(0)

    if pos == 'end':
      path.append(pos)
      #print (f"FOUND: {path}")
      path_count += 1
      continue

    if pos not in map: continue

    for dest in map[pos]:
      if dest == 'start': continue
      twice_cave_clone = twice_cave
      if dest in visited:
        if twice_cave == None: twice_cave_clone = dest
        elif dest != twice_cave: continue
        elif path.count(dest) > 1: continue

      visit_clone = visited.copy()
      path_clone = path.copy()

      if dest.islower():
        visit_clone.append(dest)

      path_clone.append(pos)

      q.append((dest, twice_cave_clone, visit_clone, path_clone))

  print (path_count)


def get_map(input):
  map = {}
  for path in filter(lambda p: p != "", input.split()):
    parts = path.split('-')
    _from = parts[0]
    _to = parts[1]

    if not _from in map:
      map[_from] = []
    map[_from].append(_to)

    if not _to in map:
      map[_to] = []
    map[_to].append(_from)

  return map

small_example = \
"""
start-A
start-b
A-c
A-b
b-d
A-end
b-end
"""

slightly_larger_example = \
"""
dc-end
HN-start
start-kj
dc-start
dc-HN
LN-dc
HN-end
kj-sa
kj-HN
kj-dc
"""

even_larger_example = \
"""
fs-end
he-DX
fs-he
start-DX
pj-DX
end-zg
zg-sl
zg-pj
pj-he
RW-he
fs-DX
pj-RW
zg-RW
start-pj
he-WI
zg-he
pj-fs
start-RW
"""

puzzle_input = \
"""
xq-XZ
zo-yr
CT-zo
yr-xq
yr-LD
xq-ra
np-zo
end-LD
np-LD
xq-kq
start-ra
np-kq
LO-end
start-xq
zo-ra
LO-np
XZ-start
zo-kq
LO-yr
kq-XZ
zo-LD
kq-ra
XZ-yr
LD-ws
np-end
kq-yr
"""

main()
