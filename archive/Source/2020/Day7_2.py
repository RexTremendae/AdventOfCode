import re

def Find(start):
    if start in visited: return visited[start]

    found = 1
    for (c, bag) in containments[start]:
        found += c*Find(bag)

    visited[start] = found
    return found

file = open("Day7.txt", "r")

regex = '\s?([0-9]*\s?[a-z]+ [a-z]+)'
containments = {}
containments["-"] = []

for line in file:
    matches = re.findall(regex, line[:-1])

    if (matches == []):
        print(f"ERROR! Could not parse '{line[:-1]}'")
        exit()

    rightSide = []
    for part in matches[2:]:
        idx = next((i for i, ch in enumerate(part) if ch == ' '), None)
        nstr = part[:idx]
        bag = part[idx+1:] if nstr != "no" else "-"
        n = int(nstr) if nstr != "no" else 0
        rightSide.append((n, bag))
    containments[matches[0]] = rightSide

for key in containments:
    print (f"{key}: {containments[key]}")
print()

visited = {}
print (Find("shiny gold")-1)
