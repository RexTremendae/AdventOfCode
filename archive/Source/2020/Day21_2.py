file = open("Day21.txt")
allIngredients = set()
candidates = {}
dishes = []

for line in [l[:-1] for l in file]:
    parts = line.split(' (contains ')
    ingredients = parts[0].split(" ")
    allergens = parts[1][:-1].split(", ")
    dishes.append((ingredients, allergens))
    #print(f"{ingredients}: {allergens}")
    for ing in ingredients:
        allIngredients.add(ing)
    for al in allergens:
        if al not in candidates:
            candidates[al] = []
        candidates[al].append(ingredients)

safeIngredients = set()
for ing in allIngredients:
    safeIngredients.add(ing)

for cand in candidates:
    candidatesLeft = set()
    for alList in candidates[cand]:
        if len(candidatesLeft) == 0:
            for al in alList:
                candidatesLeft.add(al)
        else:
            toIterate = []
            for cnl in candidatesLeft:
                toIterate.append(cnl)
            for al in toIterate:
                if al not in alList:
                    candidatesLeft.remove(al)
    candidates[cand] = candidatesLeft

for cand in candidates:
    for al in candidates[cand]:
        if al in safeIngredients: safeIngredients.remove(al)

#print (allIngredients)
#print (safeIngredients)

canonical = []

solved = False
while not solved:
    solved = True
    for cand in candidates:
        if len(candidates[cand]) < 2: continue
        solved = False
        for other in candidates:
            if other == cand: continue
            toRemove = ""
            if len(candidates[other]) == 1:
                toRemove = next(iter(candidates[other]))
            if toRemove not in candidates[cand]: continue
            candidates[cand].remove(toRemove)

for cand in candidates:
    print (f"{cand}: {candidates[cand]}")
print()

for ing in allIngredients:
    if ing not in safeIngredients:
        canonical.append(ing)

canonical.sort()
print (canonical)

canonicalIngredients = []
for ing in candidates:
    canonicalIngredients.append(ing)
canonicalIngredients.sort()

print (canonicalIngredients)
print()

print("============================================")
for caning in canonicalIngredients:
    print (next(iter(candidates[caning])),end=",")
print()
print("============================================")
