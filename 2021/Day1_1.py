file = open("Day1.txt", "r")

last = -1
count = 0

for line in file:
  current = int(line)
  if current > last: count += 1
  last = current

print (count-1)
