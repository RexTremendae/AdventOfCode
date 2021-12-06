file = open("Day2.txt", "r")

x = 0
y = 0

for line in file:
  parts = line.split(' ')
  mvmnt = int(parts[1])

  x += mvmnt if parts[0] == 'forward' else 0
  y += mvmnt if parts[0] == 'down' else (-mvmnt if parts[0] == 'up' else 0)

print (x*y)
