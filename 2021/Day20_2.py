
def main():
  (algorithm, image) = read_data()
  factor = 50
  for i in range(factor):
    image = enhance(image, algorithm, i)

  total = 0
  for line in image:
    total += sum([1 if ch == '#' else 0 for ch in line])
  print (total)


def enhance(image, algorithm, step):
  width = len(image[0])
  height = len(image)

  extra = 3

  enhanced_image = []
  for y in range(-2-extra, height+extra):
    enhanced_image_row = []
    for x in range(-2-extra, width+extra):
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
    enhanced_image.append(enhanced_image_row if step % 2 == 0 else enhanced_image_row[(extra*2):(-extra*2)])
  return enhanced_image if step % 2 == 0 else enhanced_image[(extra*2):(-extra*2)]


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
