file = open('Day13.txt', 'r')

isFoldInstruction = False
dots = set()
foldInstructions = []

for line in file.readlines():
  line = line.rstrip()
  if line == '':
    if isFoldInstruction: break
    isFoldInstruction = True
    continue
  if (isFoldInstruction):
    parts = line.split(' ')[2].split('=')
    foldInstructions.append((parts[0], int(parts[1])))
  else:
    parts = line.split(',')
    dots.add((int(parts[0]), int(parts[1])))

(foldAxis, foldLine) = foldInstructions[0]
dotsToRemove = set()
dotsToAdd = set()

for (x, y) in dots:
  if foldAxis == 'x' and x < foldLine: continue
  if foldAxis == 'y' and y < foldLine: continue
  dotsToRemove.add((x, y))
  if foldAxis == 'x':
    dotsToAdd.add((foldLine*2 - x, y))
  else:
    dotsToAdd.add((x, foldLine*2 - y))

for (x, y) in dotsToRemove:
  dots.remove((x, y))

for (x, y) in dotsToAdd:
  dots.add((x, y))

print (len(dots))
