file = open("Day10.txt", "r")

errorscores = []

for line in file.readlines():
  expected = []
  incomplete_end = ""
  for ch in line.rstrip():

    # opening
    if ch in "<{[(":
      expected.append('>' if ch == '<'
                      else '}' if ch == '{'
                      else ']' if ch == '['
                      else ')')

    # correct closing
    elif ch == expected[-1]:
      expected.pop()

    # corrupted
    else:
      expected = []
      break

  if len(expected) > 0:
    score = 0
    for x in expected[::-1]:
      score *= 5
      score += (1 if x == ')'
                else 2 if x == ']'
                else 3 if x == '}'
                else 4)
    errorscores.append(score)

print (sorted(errorscores)[len(errorscores)//2])
