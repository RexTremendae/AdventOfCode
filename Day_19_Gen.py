
def main():
  common = [
    [4,8,9],
    [2,6,4],
    [3,5,3],
    [0,6,3],
    [5,2,4]
  ]

  set1 = list(map(lambda _: rX(_), common))
  set1.insert(0, [2, 3, 1])
  set1.append([4, 3, 4])

  set2 = common.copy()
  set2.insert(0, [3, 4, 1])
  set2.append([5, 3, 5])

  print (set1)
  print (set2)


def rX(v):
  return rotate(mrX, v)

def rY(v):
  return rotate(mrY, v)

def rZ(v):
  return rotate(mrZ, v)


def rotate(m, v):
  x = 0
  y = 0
  z = 0
  for i in range(3):
    x += m[0][i]*v[i]
    y += m[1][i]*v[i]
    z += m[2][i]*v[i]

  return [x, y, z]


sin90 = 1
cos90 = 0

mrX = [
    [1, 0, 0],
    [0, cos90, -sin90],
    [0, sin90, cos90]
]

mrY = [
    [cos90, 0, sin90],
    [0, 1, 0],
    [-sin90, 0, cos90]
]

mrZ = [
    [cos90, -sin90, 0],
    [sin90, cos90, 0],
    [0, 0, 1]
]

main()
