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

print (allIngredients)
print (safeIngredients)

count = 0
for ingList, alList in dishes:
    for safe in safeIngredients:
        if safe in ingList: count += 1
print (count)
