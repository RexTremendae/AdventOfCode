file = open("Day14.txt", "r")

memory = {}
maskString = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"
data = 0
address = 0

for line in file:
    if line.startswith("mask"):
        maskString = line[7:-1]
        if len(maskString) != 36:
            print("Indata error!")
            exit()
        continue
        #print(mask)
    else:
        parts = line[:-1].split(" = ")
        address = int(parts[0][4:-1])
        data = int(parts[1])
        #print(f"mem @ {address}: {data}")

    bitValue = 2**35
    #print(data)
    for bit in maskString:
        if bit != 'X':
            data = (data | bitValue) if bit == '1' else (data & ~bitValue)
            #print (data)
        bitValue >>= 1
    #print(f"{address}: {data}")
    memory[address] = data
    #print()

#print (memory)
sum = 0
for address in memory: sum += memory[address]

print (sum)
