from termcolor import colored


puzzle_input = \
"""
5251578181
6158452313
1818578571
3844615143
6857251244
2375817613
8883514435
2321265735
2857275182
4821156644
"""

example_input = \
"""
5483143223
2745854711
5264556173
6141336146
6357385478
4167524645
2176841721
6882881134
4846848554
5283751526
"""

def print_data(data):
  for y in range(10):
    for x in range(10):
      d = data[(x, y)]
      print(colored(0, 'white', attrs=['bold']) if d > 9 else colored(d, 'cyan'), end="")
    print("")

def increase(data, x, y):
  if (x,y) not in data: return
  data[(x,y)] += 1
  if (data[(x, y)] == 10):
    for (nx,ny) in [(x-1, y-1), (x, y-1), (x+1, y-1), (x-1, y), (x+1, y), (x-1, y+1), (x, y+1), (x+1, y+1)]:
      increase(data, nx,ny)


input = puzzle_input

data = {}

y = 0
for line in input.split():
  line = line.rstrip()
  if line == "": continue
  x = 0
  for d in [int(ch) for ch in line]:
    data[(x,y)] = d
    x += 1
  y += 1


#print_data(data)
#print("")

tsum = 0

for i in range(100):
  for x in range(10):
    for y in range(10):
      increase(data, x, y)

  #print_data(data)

  sum = 0
  for x in range(10):
    for y in range(10):
      if data[(x,y)] > 9:
        data[(x,y)] = 0
        sum += 1
  tsum += sum
  #print (sum)
  #print("")

print (tsum)
