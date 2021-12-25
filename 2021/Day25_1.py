from time import time
from typing import Any


def get_indata():
  file = open('Day25.txt', 'r')
  data = []
  for line in [line.rstrip() for line in file]:
    if line == '': break
    data.append([ch for ch in line])
  return data


def get_example_indata():
  data = []
  for line in [
    'v...>>.vv>',
    '.vv>>.vv..',
    '>>.>v>...v',
    '>>v>>.>.v.',
    'v>v.vv.v..',
    '>.>>..v...',
    '.vv..>.>v.',
    'v.v..>>v.v',
    '....v..v.>'
  ]:
    data.append([ch for ch in line])

  return data


def to_int_data(in_data):
  out_data = []
  for line in in_data:
    out_data.append([1 if ch == '>' else (2 if ch == 'v' else 0) for ch in line])
  return out_data


def main():
  data = to_int_data(get_indata())

  n = 0
  while True:
    any_change = False

    for y in range(len(data)):
      to_move_x = []
      for x in range(len(data[0])):
        nx = x+1 if x < len(data[0])-1 else 0
        if data[y][x] == 1 and data[y][nx] == 0:
          to_move_x.append(x)
          any_change = True
      for x in to_move_x:
        nx = x+1 if x < len(data[0])-1 else 0
        data[y][x] = 0
        data[y][nx] = 1

    for x in range(len(data[0])):
      to_move_y = []
      for y in range(len(data)):
        ny = y+1 if y < len(data)-1 else 0
        if data[y][x] == 2 and data[ny][x] == 0:
          to_move_y.append(y)
          any_change = True
      for y in to_move_y:
        ny = y+1 if y < len(data)-1 else 0
        data[y][x] = 0
        data[ny][x] = 2

    n += 1
    if not any_change: break

  print (n)


main()
