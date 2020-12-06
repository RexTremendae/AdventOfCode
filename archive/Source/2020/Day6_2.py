file = open("Day6.txt", "r")

sum = 0
groupAnswers = set()
firstline = True

for line in file:
    if (line == "\n"):
        sum += len(groupAnswers)
        groupAnswers = set()
        firstline = True
        continue

    if (firstline):
        for ch in line[:-1]:
            groupAnswers.add(ch)
    else:
        keys = dict.fromkeys(groupAnswers,[])
        for ch in keys:
            if (not ch in line[:-1]):
                groupAnswers.remove(ch)

    firstline = False

sum += len(groupAnswers)
print (sum)
