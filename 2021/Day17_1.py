# Puzzle input
x1=79
x2=137
y1=-176
y2=-117

def main():
  global_max_y = 0

  # Arbitrary limit 200 (not sure how to programatically find it)
  for yΔ in range(200):
    max_y = 0

    xΔ = 0
    steps = 0

    while steps == 0 and xΔ < x2:
      xΔ += 1
      (steps, y) = calculate(xΔ, yΔ)

    if xΔ >= x2:
      continue

    max_y = y

    while steps != 0:
      xΔ += 1
      (steps, y) = calculate(xΔ, yΔ)
      if y > max_y:
        max_y = y

    global_max_y = max(max_y, global_max_y)
  print (global_max_y)


def calculate(xΔ, yΔ):
  min_x = 0
  min_y = min(y1, y2)
  max_x = max(x1, x2)
  max_y = 0

  inside_position = (0, 0)

  x = 0
  y = 0

  steps = 0
  while True:
    x += xΔ
    y += yΔ
    steps += 1

    if x > max_x or y < min_y: break
    if (x >= x1 and x <= x2 and y >= y1 and y <= y2):
      inside_position = (x, y)
      break

    min_x = min(min_x, x)
    max_x = max(max_x, x)
    min_y = min(min_y, y)
    max_y = max(max_y, y)

    if xΔ > 0:
      xΔ -= 1
    yΔ -= 1

  if inside_position == (0, 0):
    return (0, 0)

  return (steps, max_y)

main()
