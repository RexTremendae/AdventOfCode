import re

file = open("Day8.txt", "r")

visited = set()

code = []

for line in file:
    parts = line[:-1].split(' ')
    op = parts[0]
    arg = int(parts[1])
    code.append((op, arg))

acc = 0
ptr = 0
while True:
    if ptr in visited: break
    (op, arg) = code[ptr]
    #print (f"{ptr}: {op} {arg}")
    visited.add(ptr)
    if (op == "acc"): acc += arg
    if (op == "jmp"): ptr += arg-1
    ptr += 1

print(f"Acc is {acc} at instruction {ptr}")
