import sys

file = open("Day7.txt", "r")
crabs = [int(f) for f in file.readlines()[0].rstrip().split(',')]

minSum = sys.maxsize

for i in range(len(crabs)):
  target = crabs[i]
  sum = 0
  for j in range(len(crabs)):
    if i == j: continue
    sum += abs(target - crabs[j])
  minSum = min(sum, minSum)

print (minSum)
