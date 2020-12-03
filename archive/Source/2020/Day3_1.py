file = open("Day3.txt", "r")

data = []
for line in file:
    data.append(line[:-1])

x = 0
y = 0
treecount = 0

for line in data:
    newline = line
    if (y > 0):
        x += 3
        if (x >= len(line)): x -= len(line)
        newline = line[:x] + ('O' if line[x] == '.' else 'X') + line[(x+1):]
        if (line[x] == '#'): treecount += 1
    #print ("%d %s (%d)" % (y, newline, len(newline)))
    y += 1

print (treecount)