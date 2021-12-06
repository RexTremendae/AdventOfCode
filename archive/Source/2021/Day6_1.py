file = open("Day6.txt", "r")
fish = [int(f) for f in file.readlines()[0].rstrip().split(',')]

for s in range(1, 81):
  newCount = 0
  for i in range(len(fish)):
    fish[i] -= 1
    if fish[i] == -1:
      fish[i] = 6
      newCount += 1
  for i in range(newCount):
    fish.append(8)

print (len(fish))
