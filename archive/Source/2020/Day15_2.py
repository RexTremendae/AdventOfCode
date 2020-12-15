import datetime as dt

input = [1,0,18,10,19,6]
history = {}

turn = 1
for n in input:
    history[n] = [turn]
    turn += 1

spoken = input[-1]

start = dt.datetime.now()

while turn <= 30000000:
    hist = history[spoken]
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
    if turn % 100000 == 0:
        print (f"{turn:_}...")
print (spoken)

duration = dt.datetime.now() - start
print (f"Finished in {duration.total_seconds():.2f}s")
