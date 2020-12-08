import re

file = open("Day8.txt", "r")

orig_code = []

for line in file:
    parts = line[:-1].split(' ')
    op = parts[0]
    arg = int(parts[1])
    orig_code.append((op, arg))

acc = 0
l = 0

for l in range(len(orig_code)):
    code = orig_code[:]
    (op, arg) = code[l]
    if (op == "nop"): code[l] = ("jmp", arg)
    elif (op == "jmp"): code[l] = ("nop", arg)
    else: continue

    acc = 0
    ptr = 0
    visited = set()
    while True:
        if (ptr >= len(code)): break
        if ptr in visited: break
        (op, arg) = code[ptr]
        visited.add(ptr)
        if (op == "acc"): acc += arg
        if (op == "jmp"): ptr += arg-1
        ptr += 1
    if ptr >= len(code): break

print(f"Acc is {acc} when changing line {l}")
