file = open("Day3.txt", "r")

ones = []
zeroes = []

first = True

for line in file:
  line = line.rstrip()
  if first:
    first = False
    for b in line:
      if b == '1':
        ones.append(1)
        zeroes.append(0)
      else:
        zeroes.append(1)
        ones.append(0)
  else:
    for b in range(len(line)):
      if line[b] == '1':
        ones[b] += 1
      else:
        zeroes[b] += 1

gammaArray = []
epsilonArray = []
for b in range(len(ones)):
  g = 1 if ones[b] > zeroes[b] else 0
  gammaArray.append(g)
  epsilonArray.append(1 if g == 0 else 0)

gammaStr = ''.join(map(str, gammaArray))
epsilonStr = ''.join(map(str, epsilonArray))
gamma = int(gammaStr, 2)
epsilon = int(epsilonStr, 2)

print (gamma*epsilon)
