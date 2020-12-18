def evaluate(input):
    parts = input.split(' ')
    result = { 0: 0 }
    operator = { 0: '+' }
    level = 0

    pIdx = -1
    term = 0

    while True:
        pIdx += 1
        if (pIdx >= len(parts)): break
        p = parts[pIdx]
        endSubIdx = p.find(')')

        if p == '+':
            operator[level] = '+'
            continue
        elif p == '*':
            operator[level] = '*'
            continue
        elif p.startswith('('):
            level += 1
            parts[pIdx] = parts[pIdx][1:]
            result[level] = 0
            operator[level] = '+'
            pIdx -= 1
            continue
        elif endSubIdx >= 0:
            level -= 1
            decrease = 1
            if endSubIdx > 0:
                term = int(p[:endSubIdx])
                term = result[level+1] = result[level+1] + term if operator[level+1] == '+' else result[level+1] * term
                decrease += endSubIdx
            else:
                term = result[level+1]

            parts[pIdx] = parts[pIdx][decrease:]
            pIdx -= 1
        else:
            if len(p) > 0:
                term = int(p)
            else:
                continue            

        result[level] = result[level] + term if operator[level] == '+' else result[level] * term

    #print(result)
    return result[0]

def validate(input, expected):
    result = evaluate(input)

    if result == expected:
        print (f"OK! '{input}' => {result})")
    else:
        print (f"FAILURE! '{input}' => Expected {expected}, but got {result}")
    print()

file = open("Day18.txt", "r")

sum = 0
for line in file.readlines():
    sum += evaluate(line[:-1])
print (sum)


exit()

print()
print()
validate("2 * 3 + 4 * 5", 50)
validate("2 * 3 + (4 * 5)", 26)
validate("1 + 3 * (2 + (1 + 2))", 20)
validate("5 + (8 * 3 + 9 + 3 * 4 * 3)", 437)
validate("5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))", 12240)
validate("((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2", 13632)
validate("5 + 1 * (2 * (3 + 4))", 84)
validate("8 * 3 * 9 + (9 + 9 + 9) * 3 * 7", 5103)
validate("9 * ((6 * 9 * 5 * 6) * (3 + 5 * 4 + 3 * 3) * 8 * 3) + (9 * 7 + 6 + (2 + 8 + 6 * 2 + 5 * 8) + 9 * 3) + 2 * 5", 183_713_620)

print()
