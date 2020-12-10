file = open("Day10.txt")

data = []
data.append(0)
for line in file:
    val = int(line[:-1])
    data.append(val)

data.sort()
data.append(max(data)+3)

reachable = {}
reachable[0] = 1

for d in range(1,len(data)):
    #print (data[d], end=": ")
    c = 0
    dd = d-1
    dreach = 0
    while dd >= 0 and data[d] - data[dd] <= 3:
        #print (data[dd], end=" ")
        dreach += reachable[data[dd]]
        dd -= 1
    reachable[data[d]] = dreach
    #print(f" ({dreach})")

print (reachable[max(reachable)])
