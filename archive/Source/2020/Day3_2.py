file = open("Day3.txt", "r")

data = []
for line in file:
    data.append(line[:-1])

sum = 1
for (px, py) in ((1, 1), (3, 1), (5, 1), (7, 1), (1, 2)):
    x = 0
    y = 0
    treecount = 0

    print("%d, %d" % (px, py))
    while y < len(data):
        line = data[y]
        if (y > 0):
            x += px
            if (x >= len(line)): x -= len(line)
            line = line[:x] + ('O' if line[x] == '.' else 'X') + line[(x+1):]
            if (line[x] == 'X'): treecount += 1
        print ("%d %s (%d)" % (y, line, treecount))
        y += py

    print ()
    sum *= treecount

print ("=======")
print (sum)
#6 784 795 200 - too high
