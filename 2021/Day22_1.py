file = open('Day22.txt', 'r')

cubes = set()

for line in [l.rstrip() for l in file]:
  if line == '': continue

  turn_on = line.startswith('on')
  line = line.replace('on ', '').replace('off ', '')
  parts = line.split(',')
  if not parts[0].startswith('x=') or not parts[1].startswith('y=') or not parts[2].startswith('z='):
    print('ERROR in input!')
    exit()

  x_interval = [int(x) for x in parts[0][2:].split('..')]
  y_interval = [int(y) for y in parts[1][2:].split('..')]
  z_interval = [int(z) for z in parts[2][2:].split('..')]

  for x in range(max(x_interval[0], -50), min(x_interval[1], 50)+1):
    for y in range(max(y_interval[0], -50), min(y_interval[1], 50)+1):
      for z in range(max(z_interval[0], -50), min(z_interval[1], 50)+1):
        if turn_on:
          cubes.add((x, y, z))
        elif (x, y, z) in cubes:
          cubes.remove((x, y, z))

print (len(cubes))
