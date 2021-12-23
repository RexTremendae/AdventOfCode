from termcolor import colored

def main():
  (transformations, initial) = read_data()

  print('')
  print(colored('Naive:', 'green', attrs=['bold']))
  result = naive_and_slow(transformations, initial, 5)
  counter = {}
  for ch in result:
    if ch not in counter:
      counter[ch] = 1
    else:
      counter[ch] += 1
  print(counter)

  print('')
  print(colored('Smarter:', 'green', attrs=['bold']))
  result = smarter_and_faster(transformations, initial, 5)

  print('')
  print (result)

def smarter_and_faster(transformations, initial, steps):
  tree_transformations = {}

  for c in range(len(initial)-1):
    build_tree_transformations(tree_transformations, transformations, initial[c:c+2], 0, steps)

  return ''

def build_tree_transformations(tree_transformations, transformations, key, level, max_depth):
  if key not in tree_transformations:
    tree_transformations[key] = {}
  tree = tree_transformations[key]
  tree_level = level
  while tree_level >= 0 and tree_level not in tree: tree_level -= 1

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
