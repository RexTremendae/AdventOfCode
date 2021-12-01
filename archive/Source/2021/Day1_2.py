file = open("Day1.txt", "r")

increase_count = 0
data = []

for line in file:
  data.append(int(line))

last = 100000
for i in range(len(data)-2):
  sum = 0
  for j in range(3):
    sum += data[i+j]

  if sum > last: increase_count += 1
  last = sum

print (increase_count)
