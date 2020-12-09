data = []
file = open("Day9.txt", "r")
for line in file:
    if len(line[:-1]) == 0: break
    data.append(int(line[:-1]))
preambleLen = 25

#data = [35,20,15,25,47,40,62,55,65,95,102,117,150,182,127,219,299,277,309,576]
#preambleLen = 5

invalid = 0
for i in range(len(data)-preambleLen):
    valid = []

    for pi in range(preambleLen):
        for pj in range(pi+1, preambleLen):
            valid.append(data[pi+i] + data[pj+i])
    d = data[preambleLen+i]
    if (not d in valid):
        invalid = d
        break

for i in range(len(data)):
    d = data[i]
    smallest = d
    largest = d
    sum = d
    for j in range(i+1, len(data)):
        d = data[j]
        smallest = min(d, smallest)
        largest = max(d, largest)
        sum += d
        if (sum > invalid):
            break
        if (sum == invalid):
            print (f"{smallest} + {largest} = {smallest + largest}")
            exit()


