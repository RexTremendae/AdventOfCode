file = open("Day12.txt")

x = 0
y = 0
wx = 10
wy = -1

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

    if (op == 'F'):
        x += wx*val
        y += wy*val

    elif (op in directions):
        (xmod, ymod) = vectors[op]
        wx += val*xmod
        wy += val*ymod

    if (op == 'L' or op == 'R'):
        val = val // 90
        if (op == 'L'): val = -val
        val = (4 + val) % 4
        for i in range(val):
            t = wx
            wx = -wy
            wy = t

    print(f"{line[:-1]}: ({x}, {y}) [{wx}, {wy}]")

print (f"({x}, {y}) => {abs(x)+abs(y)}")

