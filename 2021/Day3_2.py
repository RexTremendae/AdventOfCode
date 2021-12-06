from typing import BinaryIO


def find(binArray, mode):
  binArray = [b.copy() for b in binArray]
  next = binArray
  bIdx = 0

  while (len(binArray) > 1):
    next = []
    ocnt = 0
    zcnt = 0

    for line in binArray:
      if line[bIdx] == 1: ocnt += 1
      else: zcnt += 1

    for line in binArray:
      if mode == 'm':
        if ocnt >= zcnt and line[bIdx] == 1: next.append(line)
        if zcnt > ocnt and line[bIdx] == 0: next.append(line)
      if mode == 'l':
        if zcnt > ocnt and line[bIdx] == 1: next.append(line)
        if ocnt >= zcnt and line[bIdx] == 0: next.append(line)

    binArray = next
    bIdx += 1
  return binArray[0]

file = open("Day3.txt", "r")

data = []
for line in file:
  dataLine = []
  for b in line.rstrip():
    dataLine.append(1 if b == '1' else 0)
  data.append(dataLine)

ox = int(''.join([str(b) for b in find(data, 'm')]),2)
co = int(''.join([str(b) for b in find(data, 'l')]),2)

print(ox*co)
