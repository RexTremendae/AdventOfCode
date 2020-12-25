key = [1717001,523731]
example = [5764801,17807724]

subject = 7
value = 1
i = 0
while value != key[1]:
    value = (value * subject) % 20201227
    i += 1
print (f"loop size: {i}")

loopsize = i
subject = key[0]
value = 1

for i in range(loopsize):
    value = (value * subject) % 20201227

print (f"{value}")
