
def validate(policyChar, policyMin, policyMax, password):
    return ((password[policyMin-1] == policyChar) ^ (password[policyMax-1] == policyChar))



####  MAIN  ####

file = open("Day2.txt", "r")

validCount = 0
for line in file:
    s1 = line[:-1].split(': ')
    policy = s1[0]
    password = s1[1]

    s2 = policy.split()
    interval = s2[0]
    policyChar = s2[1]

    s3 = interval.split('-')
    policyMin = int(s3[0])
    policyMax = int(s3[1])

    result = validate(policyChar, policyMin, policyMax, password)
    if (result): validCount += 1
    print("%s %s %d %d %s" % ("[True] " if result else "[False]", policyChar, policyMin, policyMax, password))

print (validCount)
