file = open('Day14.txt', 'r')

first = True
initial = ''
transformations = {}

for line in [line.rstrip() for line in file.readlines()]:
  if line == '': continue
  if first:
    initial = line
    first = False
    continue

  parts = line.split()
  transformations[parts[0]] = parts[2]

line = initial
for i in range(10):
  next = ''
  for c in range(len(line)-1):
    next += line[c]
    next += transformations[line[c:c+2]]
  line = next + line[-1]

elements = {}
for c in line:
  if c not in elements: elements[c] = 1
  else: elements[c] += 1

val = elements.values()
print (max(val) - min(val))
