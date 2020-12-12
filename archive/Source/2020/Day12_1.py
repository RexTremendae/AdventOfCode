file = open("Day12.txt")

x = 0
y = 0
d = 0

directions = ['E', 'S', 'W', 'N']

vectors = {
    'E': (1, 0),
    'W': (-1, 0),
    'N': (0, -1),
    'S': (0, 1)
}

for line in file:
    op = line[0]
    val = int(line[1:-1])

    xmod = 0
    ymod = 0
    speed = 0

    if (op == 'L' or op == 'R'):
        val = val // 90
        if (op == 'L'): val = -val
        d = (4 + d + val) % 4

    elif (op in directions):
        (xmod, ymod) = vectors[op]
        speed = val

    elif (op == 'F'):
        (xmod, ymod) = vectors[directions[d]]
        speed = val

    x += xmod*speed
    y += ymod*speed

    print(f"{line[:-1]}: {op} {val}: ({x}, {y}) {directions[d]}")

print (f"({x}, {y}) => {abs(x)+abs(y)}")
