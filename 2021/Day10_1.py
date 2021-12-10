file = open("Day10.txt", "r")

errorscore = 0

for line in file.readlines():
  expected = []
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
      errorscore += (3 if ch == ')'
                     else 57 if ch == ']'
                     else 1197 if ch == '}'
                     else 25137)
      break

print (errorscore)
