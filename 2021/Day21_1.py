p1_pos = 7
p2_pos = 8

p1_score = 0
p2_score = 0

die_rolls = 0
next_roll = 1
turn = 'p1'

while p1_score < 1000 and p2_score < 1000:
  mvmnt = 0
  for i in range(3):
    mvmnt += next_roll
    next_roll += 1
    if (next_roll > 100): next_roll = 1
  die_rolls += 3

  if turn == 'p1':
    p1_pos += mvmnt
    p1_pos = (p1_pos-1)%10+1
    turn = 'p2'
    p1_score += p1_pos
  else:
    turn = 'p1'
    p2_pos += mvmnt
    p2_pos = (p2_pos-1)%10+1
    p2_score += p2_pos


print ((p1_score if p1_score < 1000 else p2_score) * die_rolls)
