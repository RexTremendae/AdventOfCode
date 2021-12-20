from termcolor import colored

def main():
  (transformations, initial) = read_data()

  print('')
  print(colored('Naive:', 'green', attrs=['bold']))
  result = naive_and_slow(transformations, initial, 5)

  print('')
  print(colored('Smarter:', 'green', attrs=['bold']))
  result = smarter_and_faster(transformations, initial, 5)

  print('')
  print (result)

def smarter_and_faster(transformations, initial, steps):
  keys = []
  for c in range(len(initial)-1):
    keys.append(initial[c:c+2])

  tree_transformations = {}
  build_tree_transformations(tree_transformations, transformations, keys, 0, steps)

  return ''

def build_tree_transformations(tree_transformations, transformations, keys, level, max_depth):
  for key in keys:
    if key in tree_transformations:
      (line, height) = tree_transformations[key]
      if level + height < max_depth:
        for c in range(len(line)-1):
          build_tree_transformations(tree_transformations, transformations, [line[c:c+2]], height, max_depth)
    else:
      (transformations, key)

def naive_and_slow(transformations, initial, steps):
  print(f"0: {initial}")
  line = initial

  for i in range(steps):
    next = ''
    for c in range(len(line)-1):
      next += line[c]
      next += transformations[line[c:c+2]]

    line = next + line[-1]
    print(f"{i+1}: {line}")

  return line

def read_data():
  file = open('Day14.txt', 'r')
  initial = ''
  transformations = {}

  first = True
  for line in [line.rstrip() for line in file.readlines()]:
    if line == '': continue
    if first:
      initial = line
      first = False
      continue

    parts = line.split()
    transformations[parts[0]] = parts[2]

  return (transformations, initial)

main()
