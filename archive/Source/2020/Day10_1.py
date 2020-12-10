file = open("Day10.txt")

data = []
for line in file:
    val = int(line[:-1])
    data.append(val)

data.sort()
data.append(max(data)+3)

last = 0
_1count = 0
_3count = 0
for d in data:
    if (d - last == 1): _1count += 1
    elif (d - last == 3): _3count += 1
    else: print ("!!!")
    last = d

print (f"1-diff: {_1count}")
print (f"3-diff: {_3count}")
print (_3count * _1count)
