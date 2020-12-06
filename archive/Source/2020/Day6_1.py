file = open("Day6.txt", "r")

sum = 0
groupAnswers = set()

for line in file:
    if (line == "\n"):
        sum += len(groupAnswers)
        groupAnswers = set()
        continue

    for ch in line[:-1]:
        groupAnswers.add(ch)

sum += len(groupAnswers)
print (sum)
