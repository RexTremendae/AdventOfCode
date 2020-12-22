import datetime as dt

file = open("Day22.txt")

player1input = []
player2input = []

def markAsVisited(visited, input1, input2, result1, result2):
    idx = input1[0]
    if idx not in visited:
        visited[idx] = []
    for vis1, vis2, _, _ in visited[idx]:
        if vis1 == input1 and vis2 == input2: return
    visited[idx].append((input1[:], input2[:], result1[:], result2[:]))

def checkVisited(visited, input1, input2):
    idx = input1[0]
    if idx not in visited: return ([], [])
    for vis1, vis2, res1, res2 in visited[idx]:
        if input1 == vis1 and input2 == vis2: return (res1, res2)
    return ([],[])

player = 1
for line in [l[:-1] for l in file]:
    if line == '':
        player += 1
        continue
    if line.startswith('Player'):
        continue

    if player == 1:
        player1input.append(int(line))
    else:
        player2input.append(int(line))

globallyVisited = {}

def playGame(player1start, player2start, indent = ""):
    (visitedp1, visitedp2) = checkVisited(globallyVisited, player1start, player2start)
    if len(visitedp1) > 0: return (visitedp1, [])
    if len(visitedp2) > 0: return ([], visitedp2)

    player1 = player1start[:]
    player2 = player2start[:]
    locallyVisited = {}
    while len(player1) > 0 and len(player2) > 0:
        (p1visited, p2visited) = checkVisited(locallyVisited, player1, player2)
        if len(p1visited) > 0 or len(p2visited) > 0:
            return ([1], [])
        markAsVisited(locallyVisited, player1, player2, [1], [])

        p1 = player1[0]
        p2 = player2[0]
        del (player1[0])
        del (player2[0])

        if p1 > len(player1) or p2 > len(player2):
            if p1 > p2:
                player1.append(p1)
                player1.append(p2)
            else:
                player2.append(p2)
                player2.append(p1)
        else:
            (sub1, _) = playGame(player1[:p1], player2[:p2], indent + " ")
            if len(sub1) > 0:
                player1.append(p1)
                player1.append(p2)
            else:
                player2.append(p2)
                player2.append(p1)
    markAsVisited(globallyVisited, player1start, player2start, player1, player2)
    return (player1, player2)

def calculateScore(cards):
    score = 0
    for i, c in enumerate(cards[::-1]):
        score += c*(i+1)
    return score

startTime = dt.datetime.now()
(player1input,player2input) = playGame(player1input,player2input)
duration = dt.datetime.now() - startTime
print(f"Solution found after {duration.total_seconds():.2f}s")

p1Score = calculateScore(player1input)
p2Score = calculateScore(player2input)

print (f"{p1Score} : {player1input}")
print (f"{p2Score} : {player2input}")
