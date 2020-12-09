#data = [35,20,15,25,47,40,62,55,65,95,102,117,150,182,127,219,299,277,309,576]
data = []
file = open("Day9.txt", "r")
for line in file:
    if len(line[:-1]) == 0: break
    data.append(int(line[:-1]))
preambleLen = 25

for i in range(len(data)-preambleLen):
    valid = []

    for pi in range(preambleLen):
        for pj in range(pi+1, preambleLen):
            valid.append(data[pi+i] + data[pj+i])
    d = data[preambleLen+i]
    if (not d in valid):
        print (d)
        break
