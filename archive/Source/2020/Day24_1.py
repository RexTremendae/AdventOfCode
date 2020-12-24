#  //-\\ //-\\ //-\\
# | 0,0 | 1,0 | 2,0 |
#  \\-//-\\-//-\\-//-\\
#    | 0,1 | 1,1 | 2,1 |
#  //-\\-//-\\-//-\\-//
# | 0,2 | 1,2 | 2,2 |
#  \\-//-\\-//-\\-//-\\
#    | 0,3 | 1,3 | 2,3 |
#     \\-//-\\-//-\\-//

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

count = 0
for t in tiles:
    if tiles[t]: count += 1

print(count)
