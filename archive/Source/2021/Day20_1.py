
def main():
  (algorithm, image) = read_data()
  for i in range(2):
    image = enhance(image, algorithm)

  total = 0
  for line in image[4:-4]:
    total += sum([1 if ch == '#' else 0 for ch in line[4:-4]])
  print (total)


def enhance(image, algorithm):
  width = len(image[0])
  height = len(image)

  enhanced_image = []
  for y in range(-4, height+2):
    enhanced_image_row = []
    for x in range(-4, width+2):
      idx = 0
      b = 512
      for yy in range(3):
        for xx in range(3):
          b //= 2
          _x = x+xx
          _y = y+yy
          if _x < 0 or _y < 0 or _x >= width or _y >= height: continue
          if image[_y][_x] == '#': idx += b
      enhanced_image_row.append(algorithm[idx])
    enhanced_image.append(enhanced_image_row)
  return enhanced_image


def read_data():
  file = open('Day20.txt', 'r')

  algorithm = ''
  image = []

  for line in file:
    line = line.rstrip()
    if line == '': continue

    if algorithm == '': algorithm = line
    else: image.append([ch for ch in line])

  return (algorithm, image)


main()
