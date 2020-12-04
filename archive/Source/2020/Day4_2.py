import re

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
    checks = {}
    for r in required:
        checks[r] = False
        if (not r in passp): continue
        val = passp[r]

        if (r == "byr"):
            if (len(val) == 4 and 1920 <= int(val) <= 2002):
                checks[r] = True
                continue
        elif (r == "iyr"):
            if (len(val) == 4 and 2010 <= int(val) <= 2020):
                checks[r] = True
                continue
        elif (r == "eyr"):
            if (len(val) == 4 and 2020 <= int(val) <= 2030):
                checks[r] = True
                continue
        elif (r == "ecl"):
            if (val in ["amb","blu","brn","gry","grn","hzl","oth"]):
                checks[r] = True
        elif (r == "hcl"):
            if (re.search("^#[0-9a-f]{6}$", val)):
                checks[r] = True
        elif (r == "pid"):
            if (re.search("^[0-9]{9}$", val)):
                checks[r] = True
        elif (r == "hgt"):
            if (val.endswith("cm")):
                if (150 <= int(val[:-2]) <= 193):
                    checks[r] = True
            elif (val.endswith("in")):
                if (59 <= int(val[:-2]) <= 76):
                    checks[r] = True

    #for c in checks:
    #    print(f"{c}: {checks[c]}")

    isvalid = True
    for r in required:
        if(not checks[r]):
            isvalid = False
            break

    if (isvalid): validcount += 1
    #print()

print (f"Number of valid passports: {validcount}")
