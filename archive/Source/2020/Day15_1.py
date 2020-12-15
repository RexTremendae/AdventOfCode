input = [1,0,18,10,19,6]
history = {}

turn = 1
for n in input:
    history[n] = [turn]
    turn += 1

spoken = input[-1]

while turn <= 2020:
    hist = history[spoken]
    #print (f"{spoken} was last spoken at turn {turn}, history: {hist}")
    if (len(hist) < 2):
        spoken = 0
    else:
        spoken = hist[-1] - hist[-2]

    if (not spoken in history):
        hist = []
    else:
        hist = history[spoken]

    hist.append(turn)
    history[spoken] = hist
    turn += 1

print (spoken)
