import sys

file = open("Day7.txt", "r")
crabs = {}
for c in [int(c) for c in file.readlines()[0].rstrip().split(',')]:
  if c in crabs: crabs[c] += 1
  else: crabs[c] = 1

minSum = sys.maxsize
maxKey = max(crabs.keys())

for i in range(maxKey):
  #print (f"{i} ({maxKey})")
  target = i
  sum = 0
  for k in set(crabs.keys()):
    cnt = crabs[k]
    if (i == k):
      cnt -= 1
      if cnt == 0: continue
    cost = 1
    costSum = 0
    for j in range(abs(target - k)):
      costSum += cost
      cost += 1
    sum += costSum*cnt
    #print (f"{k} -> {target}: {costSum}")
  #print (sum)
  if sum > minSum: break
  minSum = sum
  #print("")

print("")
print (minSum)
