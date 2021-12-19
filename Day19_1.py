
def main():
  print (f"Compare 0 - 1")
  compare_(sample_set_0, sample_set_1)
  print (f"Compare 1 - 4")
  compare_(sample_set_1, sample_set_4)


def compare_(set1, set2):
  max_match_set_0 = set()
  max_match_set_1 = set()

  for x in range(4):
    set1 = list(map(rX, set1))
    for y in range(4):
      set1 = list(map(rY, set1))
      for z in range(4):
        set1 = list(map(rZ, set1))
        matches_1 = set()
        matches_2 = set()

        for p1_b_i in range(len(set1)):
          for p2_b_i in range(len(set2)):
            for p1_i in range(p1_b_i+1, len(set1)):
              if p1_b_i == p1_i: continue
              r1 = relative(set1[p1_b_i], set1[p1_i])

              for p2_i in range(p2_b_i, len(set2)):
                if p2_b_i == p2_i: continue
                r2 = relative(set2[p2_b_i], set2[p2_i])
                if r1 == r2:
                  matches_1.add(p1_i)
                  matches_1.add(p1_b_i)
                  matches_2.add(p2_i)
                  matches_2.add(p2_b_i)

        if (len(matches_1) > len(max_match_set_1)):
          max_match_set_1 = matches_1
          max_match_set_2 = matches_2

  match_list = list(map(lambda _:set1[_], max_match_set_1))
  match_list.sort(key=lambda _:_[0], reverse=True)
  for m in match_list:
    print (m)
  print()

  match_list = list(map(lambda _:set2[_], max_match_set_2))
  match_list.sort(key=lambda _:_[0], reverse=True)
  for m in match_list:
    print (m)
  print()

def relative(p1, p2):
  return (p2[0] - p1[0], p2[1] - p1[1], p2[2] - p1[2])


def compare(_1, _2):
  sum = 0
  for y1 in range(len(_1)):
    for x1 in range(len(_1[y1])):
      for y2 in range(len(_2)):
        for x2 in range(len(_2[y2])):
          if _1[y1][x1] == _2[y2][x2]: sum += 1
  return sum


def rX(v):
  return rotate(mrX, v)

def rY(v):
  return rotate(mrY, v)

def rZ(v):
  return rotate(mrZ, v)


def rotate(m, v):
  x = 0
  y = 0
  z = 0
  for i in range(3):
    x += m[0][i]*v[i]
    y += m[1][i]*v[i]
    z += m[2][i]*v[i]

  return (x, y, z)


sin90 = 1
cos90 = 0

mrX = [
    [1, 0, 0],
    [0, cos90, -sin90],
    [0, sin90, cos90]
]

mrY = [
    [cos90, 0, sin90],
    [0, 1, 0],
    [-sin90, 0, cos90]
]

mrZ = [
    [cos90, -sin90, 0],
    [sin90, cos90, 0],
    [0, 0, 1]
]

sample_set_0 = [
(404,-588,-901),
(528,-643,409),
(-838,591,734),
(390,-675,-793),
(-537,-823,-458),
(-485,-357,347),
(-345,-311,381),
(-661,-816,-575),
(-876,649,763),
(-618,-824,-621),
(553,345,-567),
(474,580,667),
(-447,-329,318),
(-584,868,-557),
(544,-627,-890),
(564,392,-477),
(455,729,728),
(-892,524,684),
(-689,845,-530),
(423,-701,434),
(7,-33,-71),
(630,319,-379),
(443,580,662),
(-789,900,-551),
(459,-707,401)
]

sample_set_1 = [
(686,422,578),
(605,423,415),
(515,917,-361),
(-336,658,858),
(95,138,22),
(-476,619,847),
(-340,-569,-846),
(567,-361,727),
(-460,603,-452),
(669,-402,600),
(729,430,532),
(-500,-761,534),
(-322,571,750),
(-466,-666,-811),
(-429,-592,574),
(-355,545,-477),
(703,-491,-529),
(-328,-685,520),
(413,935,-424),
(-391,539,-444),
(586,-435,557),
(-364,-763,-893),
(807,-499,-711),
(755,-354,-619),
(553,889,-390)
]

sample_set_4 = [
(727,592,562),
(-293,-554,779),
(441,611,-461),
(-714,465,-776),
(-743,427,-804),
(-660,-479,-426),
(832,-632,460),
(927,-485,-438),
(408,393,-506),
(466,436,-512),
(110,16,151),
(-258,-428,682),
(-393,719,612),
(-211,-452,876),
(808,-476,-593),
(-575,615,604),
(-485,667,467),
(-680,325,-822),
(-627,-443,-432),
(872,-547,-609),
(833,512,582),
(807,604,487),
(839,-516,451),
(891,-625,532),
(-652,-548,-490),
(30,-46,-14)
]

main()
