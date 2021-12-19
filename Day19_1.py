from time import time

def main():
  print()

  #print (f"Compare 0 - 1...")
  #compare_and_print(sample_set_0, sample_set_1)

  #print (f"Compare 1 - 4...")
  #compare_and_print(sample_set_1, sample_set_4)

  #result = set(sample_set_0)
  #for m in result:
  #  print (m)

  result = compare_(sample_set_0, sample_set_1)
  r1 = result[2]
  print(f"Rotation: {result[2]}")
  s1 = result[3]
  print(f"Sensor: {s1}")
  print()

  for r in result[0]:
    print(r)

  print()
  for r in result[1]:
    print(r)
  print()

  result = compare_(sample_set_1, sample_set_4)
  print(f"Rotation: {result[2]}")
  print(f"Sensor: {result[3]}")
  print()

  r1 = result[2]
  s1 = result[3]
  for r in result[1]:
    rx = rX(r, r=r1[0], cc=True)
    ry = rY(rx, r=r1[1], cc=True)
    rz = rZ(ry, r=r1[2], cc=True)
    #print ((rz[0] + s1[0], rz[1] + s1[1], rz[2] + s1[2]))
    #print (relative(rz, s1))
    #print (relative(s1, rz))


def compare_and_print(set1, set2):
  start = time()
  (result1, result2, (rx, ry, rz)) = compare_(set1, set2)
  end = time()

  print()
  print(f"X: {rx}, Y: {ry}, Z: {ry}")
  print()

  #result1.sort(key=lambda _:_[0], reverse=True)
  for m in result1:
    print (m)
  print()

  #result2.sort(key=lambda _:_[0], reverse=True)
  for m in result2:
    print (m)
  print()

  for i in range(len(result1)):
    print (relative(result2[i], result1[i]))

  print()
  print(f"{end - start:.3f}s")
  print("---------------------")
  print()


def compare_(set1, set2):
  max_match_set_1 = set()
  max_match_set_2 = set()

  rotation = (0, 0, 0)
  max_sensor = (0, 0)

  for x in range(4):
    set2 = list(map(rX, set2))
    for y in range(4):
      set2 = list(map(rY, set2))
      for z in range(4):
        set2 = list(map(rZ, set2))
        matches_1 = set()
        matches_2 = set()
        sensor = (0, 0)

        for p1_b_i in range(len(set1)):
          for p2_b_i in range(len(set2)):
            for p1_i in range(p1_b_i+1, len(set1)):
              if p1_b_i == p1_i: continue
              r1 = relative(set1[p1_b_i], set1[p1_i])

              for p2_i in range(p2_b_i+1, len(set2)):
                if p2_b_i == p2_i: continue
                r2 = relative(set2[p2_b_i], set2[p2_i])
                if r1 == r2:
                  if p1_i not in matches_1:
                    matches_1.add(p1_i)
                    matches_2.add(p2_i)
                    sensor = relative(set2[p2_i], set1[p1_i])
                  if p1_b_i not in matches_1:
                    matches_1.add(p1_b_i)
                    matches_2.add(p2_b_i)
                    sensor = relative(set2[p2_b_i], set1[p1_b_i])

        if (len(matches_1) > len(max_match_set_1)):
          max_match_set_1 = matches_1
          max_match_set_2 = matches_2
          rotation = (x, y, z)
          max_sensor = sensor

  #result_1 = []
  #for r in max_match_set_1:
  #  result_1.append(set1[r])

  result_1 = list(map(lambda _: set1[_], max_match_set_1))
  result_2 = list(map(lambda _: set2[_], max_match_set_2))
  #for r in max_match_set_2:
  #  result_2.append(set2[r])

  return (result_1, result_2, rotation, max_sensor)


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


def rX(v, r = 1, cc = False):
  for t in range(r):
    v = rotate(mrXcc if cc else mrX, v)
  return v

def rY(v, r = 1, cc = False):
  for t in range(r):
    v = rotate(mrYcc if cc else mrY, v)
  return v

def rZ(v, r = 1, cc = False):
  for t in range(r):
    v = rotate(mrZcc if cc else mrZ, v)
  return v


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

mrXcc = [
    [1, 0, 0],
    [0, cos90, sin90],
    [0, -sin90, cos90]
]

mrYcc = [
    [cos90, 0, -sin90],
    [0, 1, 0],
    [sin90, 0, cos90]
]

mrZcc = [
    [cos90, sin90, 0],
    [-sin90, cos90, 0],
    [0, 0, 1]
]

#--- scanner 0 ---
sample_set_0 = \
[
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

#--- scanner 1 ---
sample_set_1 = \
[
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

#--- scanner 2 ---
sample_set_2 = \
[
(649,640,665),
(682,-795,504),
(-784,533,-524),
(-644,584,-595),
(-588,-843,648),
(-30,6,44),
(-674,560,763),
(500,723,-460),
(609,671,-379),
(-555,-800,653),
(-675,-892,-343),
(697,-426,-610),
(578,704,681),
(493,664,-388),
(-671,-858,530),
(-667,343,800),
(571,-461,-707),
(-138,-166,112),
(-889,563,-600),
(646,-828,498),
(640,759,510),
(-630,509,768),
(-681,-892,-333),
(673,-379,-804),
(-742,-814,-386),
(577,-820,562)
]

#--- scanner 3 ---
sample_set_3 = \
[
(-589,542,597),
(605,-692,669),
(-500,565,-823),
(-660,373,557),
(-458,-679,-417),
(-488,449,543),
(-626,468,-788),
(338,-750,-386),
(528,-832,-391),
(562,-778,733),
(-938,-730,414),
(543,643,-506),
(-524,371,-870),
(407,773,750),
(-104,29,83),
(378,-903,-323),
(-778,-728,485),
(426,699,580),
(-438,-605,-362),
(-469,-447,-387),
(509,732,623),
(647,635,-688),
(-868,-804,481),
(614,-800,639),
(595,780,-596)
]

#--- scanner 4 ---
sample_set_4 = \
[
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
(30,-46,-14),
]

main()
