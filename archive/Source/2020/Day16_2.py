file = open("Day16.txt")

section = 0

intervals = {}
tickets = []
yourTicket = []

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
        currTicket = [int(d) for d in data.split(',')]
        tickets.append(currTicket)
        if section == 1:
            yourTicket = currTicket[:]

validTickets = []

def isTicketValid(ticket):
    isTicketValid = True
    for data in tckt:
        isDataValid = False
        for tpe in intervals:
            for intr in intervals[tpe]:
                if (data >= intr[0] and data <= intr[1]):
                    isDataValid = True
                    break
        if not isDataValid:
            isTicketValid = False
            break
    return isTicketValid

def getColumnPlausibilities(column):
    plausibleTypes = []
    for tpe in intervals:
        plausibleType = True
        for tckt in validTickets:
            plausibleForTicket = False
            for intr in intervals[tpe]:
                if (tckt[column] >= intr[0] and tckt[column] <= intr[1]):
                    plausibleForTicket = True
                    break
            if not plausibleForTicket:
                plausibleType = False
                break
        if plausibleType:
            plausibleTypes.append(tpe)
    return plausibleTypes

for tckt in tickets:
    if isTicketValid(tckt):
        validTickets.append(tckt)

plausibilities = {}
for col in range(len(validTickets[0])):
    columnPlausibilities = getColumnPlausibilities(col)
    plausibilities[col] = columnPlausibilities

singlePlausibilities = {}

for col in range(len(validTickets[0])):
    for idx in plausibilities:
        if len(plausibilities[idx]) == 1:
            break
    plaus = plausibilities[idx][0]
    print (f"{idx}: {plaus}")
    singlePlausibilities[idx] = plaus
    for plausIdx in plausibilities:
        plauslist = plausibilities[plausIdx]
        if plaus in plauslist:
            plauslist.remove(plaus)

print (yourTicket)

answer = 1
for col in range(len(yourTicket)):
    tpe = singlePlausibilities[col]
    if tpe.startswith('departure'):
        answer *= yourTicket[col]
        print (f"{tpe}: {yourTicket[col]}")

print ()
print ("=============")
print (answer)
print ("=============")
print()
