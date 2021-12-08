file = open("Day8.txt", "r")
tsum = 0

for line in file.readlines():
  parts = line.rstrip().split('|')
  tsum += sum(1 for x in parts[1].split(' ') if len(x) in [2,3,4,7])

print (tsum)
