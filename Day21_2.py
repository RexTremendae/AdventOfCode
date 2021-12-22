from time import time

def main():
  #Puzzle input
  #p1_pos = 7
  #p2_pos = 8

  #Example
  p1_pos = 4
  p2_pos = 8

  start = time()
  (p1_total_wins, p2_total_wins) = run_brute_force2(21, p1_pos, p2_pos)

  print (f"!!! {p1_total_wins} - {p2_total_wins}  ({time() - start:.3f}s)")
  print()

  for w in range(1, 6):
    #start = time()
    #(p1_total_wins, p2_total_wins) = run_brute_force(w, p1_pos, p2_pos)
    #print (f"{w}: {p1_total_wins} - {p2_total_wins}  ({time() - start:.3f}s)")

    start = time()
    (p1_total_wins, p2_total_wins) = run_brute_force2(w, p1_pos, p2_pos)
    print (f"{w}: {p1_total_wins} - {p2_total_wins}  ({time() - start:.3f}s)")
    print()

def run_brute_force2(winning_score, p1_pos, p2_pos):
  # (p1_pos, p2_pos, p1_score, p2_score, turn, multiplier)
  universes = [(p1_pos, p2_pos, 0, 0, 'p1', 1)]

  p1_total_wins = 0
  p2_total_wins = 0

  while len(universes) > 0:
    (p1_pos, p2_pos, p1_score, p2_score, turn, multiplier) = universes[0]
    universes.pop(0)
    #print(f"{turn}  p1_pos: {p1_pos}  p2_pos: {p2_pos}  p1_score: {p1_score}  p2_score: {p2_score}")

    enqd = 0

    for roll in die_rolls2:
      n_p1_pos = p1_pos
      n_p2_pos = p2_pos
      n_p1_score = p1_score
      n_p2_score = p2_score

      if turn == 'p1':
        n_p1_pos = (p1_pos + roll - 1)%10 + 1
        n_p1_score = p1_score + n_p1_pos
        if (n_p1_score) >= winning_score:
          p1_total_wins += die_rolls2[roll]*multiplier
          #print (f"  {roll}: p1 wins! ({die_rolls2[roll]*multiplier})")
          continue
      else:
        n_p2_pos = (p2_pos + roll - 1)%10 + 1
        n_p2_score = p2_score + n_p2_pos
        if (n_p2_score) >= winning_score:
          p2_total_wins += die_rolls2[roll]*multiplier
          #print (f"  {roll}: p2 wins! ({die_rolls2[roll]*multiplier})")
          continue

      n_turn = 'p2' if turn == 'p1' else 'p1'
      universes.append((n_p1_pos, n_p2_pos, n_p1_score, n_p2_score, n_turn, die_rolls2[roll]*multiplier))
      enqd += 1

    #print (f"  {enqd} enqueued")

  return (p1_total_wins, p2_total_wins)


def run_brute_force(winning_score, p1_pos, p2_pos):
  # (p1_pos, p2_pos, p1_score, p2_score, turn)
  universes = [(p1_pos, p2_pos, 0, 0, 'p1')]

  p1_total_wins = 0
  p2_total_wins = 0

  while len(universes) > 0:
    (p1_pos, p2_pos, p1_score, p2_score, turn) = universes[0]
    universes.pop(0)
    #print(f"{turn}  p1_pos: {p1_pos}  p2_pos: {p2_pos}  p1_score: {p1_score}  p2_score: {p2_score}")

    enqd = 0

    for roll in die_rolls:
      n_p1_pos = p1_pos
      n_p2_pos = p2_pos
      n_p1_score = p1_score
      n_p2_score = p2_score

      if turn == 'p1':
        n_p1_pos = (p1_pos + roll - 1)%10 + 1
        n_p1_score = p1_score + n_p1_pos
        if (n_p1_score) >= winning_score:
          p1_total_wins += 1
          #print (f"  {roll}: p1 wins!")
          continue
      else:
        n_p2_pos = (p2_pos + roll - 1)%10 + 1
        n_p2_score = p2_score + n_p2_pos
        if (n_p2_score) >= winning_score:
          p2_total_wins += 1
          #print (f"  {roll}: p2 wins!")
          continue

      n_turn = 'p2' if turn == 'p1' else 'p1'
      universes.append((n_p1_pos, n_p2_pos, n_p1_score, n_p2_score, n_turn))
      enqd += 1

    #print (f"  {enqd} enqueued")

  return (p1_total_wins, p2_total_wins)


die_rolls = []

for r1 in range(1, 4):
  for r2 in range(1, 4):
    for r3 in range(1, 4):
      die_rolls.append(r1 + r2 + r3)

die_rolls.sort()

die_rolls2 = {}
for roll in die_rolls:
  if roll in die_rolls2:
    die_rolls2[roll] += 1
  else:
    die_rolls2[roll] = 1

main()
