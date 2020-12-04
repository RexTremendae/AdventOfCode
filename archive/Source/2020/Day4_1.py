file = open("Day4.txt", "r")

passports = []
current = {}
passports.append(current)

for line in file:
    if (line == '\n'):
        current = {}
        passports.append(current)
        continue

    parts = line[:-1].split(' ')

    c = 0
    for p in parts:
        c += 1
        kvp = p.split(':')
        current[kvp[0]] = kvp[1]

validcount = 0
required = ["byr","iyr","eyr","hgt","hcl","ecl","pid"]

for passp in passports:
    isvalid = True
    for req in required:
        if(not req in passp):
            isvalid = False
            break
    if (isvalid): validcount += 1

print (f"Number of valid passports: {validcount}")
