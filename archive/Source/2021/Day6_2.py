file = open("Day6.txt", "r")
fish = {}
for f in [int(x) for x in file.readlines()[0].split(',')]:
  if f in fish:
    fish[f] += 1
  else:
    fish[f] = 1

step = 256
sum = 0
while len(fish) > 0:
  d = min(fish.keys())
  cnt = fish[d]
  fish.pop(d)

  sIdx = d + 1
  while sIdx <= step:
    idx = 8+sIdx
    if idx in fish:
      fish[idx] += cnt
    else:
      fish[idx] = cnt
    sIdx += 7
  sum += cnt

print(sum)
