
def main():
  file = open('Day18.txt', 'r')
  lines = [line.rstrip() for line in file]

  max_magnitude = 0

  for l1 in range(len(lines)):
    for l2 in range(l1+1, len(lines)):
      r1 = str(reduce(parse(f"[{lines[l1]},{lines[l2]}]")))
      r2 = str(reduce(parse(f"[{lines[l2]},{lines[l1]}]")))
      max_magnitude = max(get_magnitude(r1), max_magnitude)
      max_magnitude = max(get_magnitude(r2), max_magnitude)

  print (max_magnitude)


def get_magnitude(input):
  any_change = True
  while any_change:
    any_change = False
    idx = 0
    while idx < len(input):
      if  input[idx] == ',' and \
          idx > 0 and input[idx-1].isdigit() and \
          idx < len(input)-1 and input[idx+1].isdigit():
        idx_left = idx-2
        while input[idx_left].isdigit(): idx_left -= 1
        idx_right = idx+2
        while input[idx_right].isdigit(): idx_right += 1

        left_remainder = input[:idx_left]
        right_remainder = input[idx_right+1:]

        parts = input[idx_left+1:idx_right].split(',')
        left_value = int(parts[0])
        right_value = int(parts[1])

        input = left_remainder + str(3*left_value + 2*right_value) + right_remainder
        any_change = True
        break
      idx += 1
  return int(input)


def reduce(input):
  any_action = True
  while any_action:
    any_action = False
    to_explode = find_explode_candidate(input)
    if to_explode != None:
      any_action = True
      explode_left(to_explode, to_explode.children[0])
      explode_right(to_explode, to_explode.children[1])
      if to_explode.parent.children[0] == to_explode:
        to_explode.parent.children[0] = 0
      else:
        to_explode.parent.children[1] = 0
      continue
    if split(input): any_action = True
  return input


def split(input):
  last = input
  dir = 'D1'

  while True:
    if dir == 'U':
      last = input
      input = input.parent
      if input == None: return False

      if input.children[0] == last:
        if child_is_primitive(input, 1):
          if split_child(input, 1):
            return True

          dir = 'U'
          continue

        dir = 'D2'
        continue

    elif dir == 'D1':
      if child_is_primitive(input, 0):
        if split_child(input, 0):
          return True

        dir = 'D2'
        continue
      else:
        input = input.children[0]

    else: # dir == 'D2'
      if child_is_primitive(input, 1):
        if split_child(input, 1):
          return True
        dir = 'U'
        continue
      else:
        input = input.children[1]
        dir = 'D1'


def split_child(input, child):
  value = input.children[child]
  if value < 10: return False
  left_val = value // 2
  right_val = value - left_val
  input.children[child] = s_number([left_val, right_val], input, input.level+1)
  return True


def explode_left(input, value):
  last = input
  input = input.parent
  dir = 'U'

  while input != None:
    if dir == 'U':
      if input.children[0] == last:
        last = input
        input = input.parent
      else:
        if child_is_primitive(input, 0):
          input.children[0] += value
          return
        dir = 'D'
        input = input.children[0]
        continue
    else: # dir == 'D'
      if child_is_primitive(input, 1):
        input.children[1] += value
        return
      else:
        input = input.children[1]


def explode_right(input, value):
  last = input
  input = input.parent
  dir = 'U'

  while input != None:
    if dir == 'U':
      if input.children[1] == last:
        last = input
        input = input.parent
      else:
        if child_is_primitive(input, 1):
          input.children[1] += value
          return
        dir = 'D'
        input = input.children[1]
        continue
    else: # dir == 'D'
      if child_is_primitive(input, 0):
        input.children[0] += value
        return
      else:
        input = input.children[0]


def find_explode_candidate(input):
  while input != None:
    if child_is_primitive(input, 0):
      if child_is_primitive(input, 1):
        if input.level >= 4:
          return input
        last = input
        input = input.parent
        while True:
          if input == None: return None
          if input.children[0] == last and not child_is_primitive(input, 1):
            input = input.children[1]
            break
          last = input
          input = input.parent
      else:
        input = input.children[1]
    else:
      input = input.children[0]
  return input


def parse(input):
  current = None

  for c in input:
    if c == '[':
      if current == None:
        current = s_number([None, None], None, 0)
        continue
      else:
        new_current = s_number([None, None], current, current.level+1)
        if current.children[0] == None:
          current.children[0] = new_current
        else:
          current.children[1] = new_current
        current = new_current
    elif c == ']':
      if current.parent != None:
        current = current.parent
    elif c == ',':
      continue
    else:
      if current.children[0] == None:
        current.children[0] = int(c)
      else:
        current.children[1] = int(c)
  return current


def child_is_primitive(number, idx):
  return type(number.children[idx]) is int


class s_number:
  def __init__(self, children, parent, level):
    self.children = children
    self.parent = parent
    self.level = level

  def __str__(self):
    return '[' + \
           (str(self.children[0]) if child_is_primitive(self, 0) else \
           self.children[0].__str__()) + \
           ',' + \
           (str(self.children[1]) if child_is_primitive(self, 1) else \
           self.children[1].__str__()) + \
           ']'

  def get_magnitude(self):
    l = self.children[0] if child_is_primitive(self, 0) else self.children[0].get_magnitude()
    r = self.children[1] if child_is_primitive(self, 1) else self.children[1].get_magnitude()
    return 3*l + 2*r

main()
