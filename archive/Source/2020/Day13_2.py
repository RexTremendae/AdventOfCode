import sys
import datetime as dt
import math

def solve(buses):
    startTime = dt.datetime.now()

    increment = 1
    base = 1

    for d, val in enumerate(buses):
        if val == 0: continue
        ok = False
        while not ok:
            #print (f"{base}")
            ok = True
            for i in range(d+1):
                if buses[i] == 0: continue
                #print (f"test ({base}+{i}+1) % buses[{i}]: {(base+i+1) % buses[i]}")
                if (base+i+1) % buses[i] != 0:
                    ok = False
                    break
            if not ok: base += increment
        increment *= val

        print (f"{d}: {val} [base: {base}]")

    duration = dt.datetime.now() - startTime
    solution = base+1
    print(f"Solution {solution} found after {duration.total_seconds():.2f}s")

    return solution

def validate(input, expected):
    result = solve(input)

    if result == expected:
        print (f"OK! ({input} => {result})")
    else:
        print (f"FAILURE! ({input}) Expected {expected}, but got {result}")
    print()

file = open("Day13.txt", "r")

buses = []
file.readline()
for i, depart in enumerate(file.readline()[:-1].split(',')):
    buses.append(0 if depart == 'x' else int(depart))

validate([7,13,0,0,59,0,31,19], 1068781)
validate([67,7,59,61], 754018)
validate([67,0,7,59,61], 779210)
validate([1789,37,47,1889], 1202161486)

solve(buses)
