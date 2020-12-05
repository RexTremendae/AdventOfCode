file = open("Day5.txt", "r")

maxId = 0

for line in file:
    upper = 127
    lower = 0
    for r in line[:7]:
        halflength = (upper - lower + 1) // 2
        if (r == "B"):
            lower += halflength
        else:
            upper -= halflength
    row = upper

    upper = 7
    lower = 0

    for r in line[7:10]:
        halflength = (upper - lower + 1) // 2
        if (r == "R"):
            lower += halflength
        else:
            upper -= halflength

    col = upper
    id = row*8 + col

    if (id > maxId): maxId = id
    print (f"[{line[:-1]}] ", end="")
    print (f"row: {row}, col: {col}, id: {id}")

print (maxId)
