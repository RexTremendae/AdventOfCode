import sys

file = open("Day13.txt", "r")

earliest = int(file.readline())
departures = file.readline()[:-1].split(',')

minWait = sys.maxsize
minDepart = 0

for dep in departures:
    wait = -1
    if (dep != 'x'):
        dp = int(dep)
        wait = dp - (earliest % dp)
        if (wait < minWait):
            minWait = wait
            minDepart = dp

    #waitStr = "-" if wait < 0 else f"{wait}"
    #print(f"{dep}: {waitStr}")

print (f"{minDepart}: {minWait} => {minDepart*minWait}")