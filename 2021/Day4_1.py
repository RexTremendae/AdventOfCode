file = open("Day4.txt", "r")

numbers = []
first = True
boards = []
currentBoard = ({}, [], []) # rowidx, data, marks
rowIdx = 0

for line in file:
  line = line.rstrip()
  if first:
    numbers = line.split(',')
    first = False
  elif line == '':
    if len(currentBoard[0]) > 0:
      boards.append(currentBoard)
      currentBoard = ({}, [], [])
      currentBoardMarks = []
      rowIdx = 0
  else:
    row = line.split()
    colIdx = 0
    for r in row:
      currentBoard[0][r] = (rowIdx, colIdx)
      colIdx += 1
    currentBoard[1].append(row)
    currentBoard[2].append([False for r in row])
    rowIdx += 1
boards.append(currentBoard)

win = -1
for n in numbers:
  for b in range(len(boards)):
    curr = boards[b]
    if n in curr[0]:
      idx = curr[0][n]
      curr[2][idx[0]][idx[1]] = True
      if len(set(curr[2][idx[0]])) == 1 or len(set([d[idx[1]] for d in curr[2]])) == 1:
        win = b
  if win >= 0: break

winb = boards[win]
print (f"Board {win} wins")
sum = 0
for row in range(len(winb[2])):
  for col in range(len(winb[2][row])):
    mrk = winb[2][row][col]
    data = winb[1][row][col]
    if not mrk:
      sum += int(winb[1][row][col])

print (sum*int(n))
