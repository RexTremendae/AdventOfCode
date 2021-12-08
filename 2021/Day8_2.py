def tostring(arr):
  return "".join(sorted(arr))

def get_output(line):
  parts = line.rstrip().split('|')

  inputs = [tostring(s) for s in parts[0].split(' ')]
  outputs = [tostring(s) for s in parts[1].split(' ')]
  all = inputs + outputs

  digit_by_segments = {}

  oneseg = ""
  threeseg = ""
  sixseg = ""

  # 4
  segments = [s for s in all if len(s) == 4][0]
  digit_by_segments[segments] = 4

  # 8
  digit_by_segments["abcdefg"] = 8

  # 1
  segments = [s for s in all if len(s) == 2][0]
  digit_by_segments[segments] = 1
  oneseg = segments

  # 7
  segments = [s for s in all if len(s) == 3][0]
  digit_by_segments[segments] = 7

  # 6
  for segments in [s for s in all if len(s) == 6]:
    if tostring(set(segments) & set(oneseg)) != oneseg:
      digit_by_segments[segments] = 6
      sixseg = segments

  # 2, 3, 5
  for segments in [s for s in all if len(s) == 5]:
    if tostring(set(segments) & set(oneseg)) == oneseg:
      digit_by_segments[segments] = 3
      threeseg = segments
    elif len(set(segments) & set(sixseg)) == 5:
      digit_by_segments[segments] = 5
    else:
      digit_by_segments[segments] = 2

  # 0, 9
  for segments in [s for s in all if len(s) == 6]:
    if tostring(set(segments) & set(oneseg)) == oneseg:
      if tostring(set(segments) & set(threeseg)) == threeseg:
        digit_by_segments[segments] = 9
      else:
        digit_by_segments[segments] = 0

  numbers = []
  for seg in [o for o in outputs if o != '']:
    numbers.append(digit_by_segments[seg])

  sum = 0

  d = 1
  for n in numbers[::-1]:
    sum += d*n
    d*=10

  return sum


file = open("Day8.txt", "r")
print (sum([get_output(line.rstrip()) for line in file.readlines()]))
