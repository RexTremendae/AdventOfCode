def main():
  indata = read_data()

  print (f'BF: {brute_force(indata)}')
  print (f'Smarter: {solve_cubes(indata)}')



def solve_cubes(indata):
  cubes = []

  for (turn_on, x_interval, y_interval, z_interval, _) in indata:
    for cbe in cubes:
      overlap = get_overlap(cbe, (x_interval, y_interval, z_interval))
      print (overlap)

  return 0


def get_overlap(cube1, cube2):
  


def brute_force(indata):
  cubes = set()
  for (turn_on, x_interval, y_interval, z_interval, _) in indata:
    for x in range(x_interval[0], x_interval[1]+1):
      for y in range(y_interval[0], y_interval[1]+1):
        for z in range(z_interval[0], z_interval[1]+1):
          if turn_on:
            cubes.add((x, y, z))
          elif (x, y, z) in cubes:
            cubes.remove((x, y, z))
  return len(cubes)


def read_data():
  file = open('Day22.txt', 'r')

  line_data = []
  line_number = 0

  for line in [l.rstrip() for l in file]:
    line_number += 1
    if line == '' or line[0] == '#': continue

    turn_on = line.startswith('on')
    line = line.replace('on ', '').replace('off ', '')
    parts = line.split(',')
    if not parts[0].startswith('x=') or not parts[1].startswith('y=') or not parts[2].startswith('z='):
      print('ERROR in input!')
      exit()

    x_interval = [int(x) for x in parts[0][2:].split('..')]
    y_interval = [int(y) for y in parts[1][2:].split('..')]
    z_interval = [int(z) for z in parts[2][2:].split('..')]

    line_data.append((turn_on, x_interval, y_interval, z_interval, line_number))

  return line_data


main()
