exampleInput = [3,8,9,1,2,5,4,6,7]
puzzleInput = [6,4,3,7,1,9,2,5,8]
inputData = puzzleInput

class Node:
    def __init__(self, label):
        self.label = label
        self.next = None
    def assignNext(self, nextNode):
        self.next = nextNode

def printList(node, separator = " "):
    first = node
    it = first

    while True:
        print(f"{it.label}", end=separator)
        it = it.next
        if (it == None or it.label == first.label): break
    print()

def findInList(node, value):
    first = node
    it = first

    while True:
        if it.label == value: return it
        it = it.next
        if (it == None or it.label == first.label): break

    return None

n = Node(inputData[0])
nn = n
for d in inputData[1:]:
    nn.assignNext(Node(d))
    nn = nn.next
nn.next = n

for i in range(100):
    label = n.label
    n1 = n.next
    n2 = n1.next
    n3 = n2.next
    nn = n3.next
    nextN = nn
    n.assignNext(nn)
    n3.assignNext(None)

    labelToFind = label-1
    if labelToFind < 1: labelToFind = 9

    while findInList(n1, labelToFind) != None:
        labelToFind -= 1
        if labelToFind < 1: labelToFind = 9

    n = findInList(n, labelToFind)
    n3.assignNext(n.next)
    n.assignNext(n1)
    n = nn

while (n.next.label != 1):
    n = n.next

n.assignNext(n.next.next)
n = n.next

print()
printList(n, "")
print()
