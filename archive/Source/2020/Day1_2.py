
file = open("Day1.txt", "r")

data = []
for line in file:
    data.append(int(line[:-1]))

for d1 in range(len(data)-1):
    for d2 in range(d1+1, len(data)):
        for d3 in range(d2+1, len(data)):
            if (data[d1] + data[d2] + data[d3] == 2020):
                print (data[d1] * data[d2] * data[d3])
