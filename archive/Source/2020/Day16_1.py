file = open("Day16.txt")

section = 0

intervals = {}
tickets = []

for line in file:
    data = line[:-1]
    if len(data) == 0: continue
    elif data == 'your ticket:': section = 1
    elif data == 'nearby tickets:': section = 2
    elif section == 0:
        parts = data.split(': ')
        intervalParts = parts[1].split(' or ')
        intervalList = []
        for intr in intervalParts:
            startstop = intr.split('-')
            intervalList.append((int(startstop[0]), int(startstop[1])))
        intervals[parts[0]] = intervalList
    else:
        tickets.append([int(d) for d in data.split(',')])

invalidSum = 0

for tckt in tickets[1:]:
    for data in tckt:
        isValid = False
        for tpe in intervals:
            for intr in intervals[tpe]:
                if (data >= intr[0] and data <= intr[1]):
                    #print (f"{tckt}: {data}")
                    isValid = True
                    break
            if isValid: break
        if not isValid: invalidSum += data

print (invalidSum)
