file = open("Day22.txt")

player1 = []
player2 = []

player = 1
for line in [l[:-1] for l in file]:
    if line == '':
        player += 1
        continue
    if line.startswith('Player'):
        continue

    if player == 1:
        player1.append(int(line))
    else:
        player2.append(int(line))

while len(player1) > 0 and len(player2) > 0:
    p1 = player1[0]
    p2 = player2[0]
    del (player1[0])
    del (player2[0])

    if p1 > p2:
        player1.append(p1)
        player1.append(p2)
    else:
        player2.append(p2)
        player2.append(p1)

def calculateScore(cards):
    score = 0
    for i, c in enumerate(cards[::-1]):
        score += c*(i+1)
    return score

p1Score = calculateScore(player1)
p2Score = calculateScore(player2)

print (f"{p1Score} : {player1}")
print (f"{p2Score} : {player2}")
