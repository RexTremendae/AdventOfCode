
def main():
  #decode_input('D2FE28')                     # 2021
  #decode_input('C200B40A82')                 # finds the sum of 1 and 2, resulting in the value 3.
  #decode_input('04005AC33890')               # finds the product of 6 and 9, resulting in the value 54.
  #decode_input('880086C3E88112')             # finds the minimum of 7, 8, and 9, resulting in the value 7.
  #decode_input('CE00C43D881120')             # finds the maximum of 7, 8, and 9, resulting in the value 9.
  #decode_input('D8005AC2A8F0')               # produces 1, because 5 is less than 15.
  #decode_input('F600BC2D8F')                 # produces 0, because 5 is not greater than 15.
  #decode_input('9C005AC2F8F0')               # produces 0, because 5 is not equal to 15.
  #decode_input('9C0141080250320F1802104A08') # produces 1, because 1 + 3 = 2 * 2.
  decode_input('020D708041258C0B4C683E61F674A1401595CC3DE669AC4FB7BEFEE840182CDF033401296F44367F938371802D2CC9801A980021304609C431007239C2C860400F7C36B005E446A44662A2805925FF96CBCE0033C5736D13D9CFCDC001C89BF57505799C0D1802D2639801A900021105A3A43C1007A1EC368A72D86130057401782F25B9054B94B003013EDF34133218A00D4A6F1985624B331FE359C354F7EB64A8524027D4DEB785CA00D540010D8E9132270803F1CA1D416200FDAC01697DCEB43D9DC5F6B7239CCA7557200986C013912598FF0BE4DFCC012C0091E7EFFA6E44123CE74624FBA01001328C01C8FF06E0A9803D1FA3343E3007A1641684C600B47DE009024ED7DD9564ED7DD940C017A00AF26654F76B5C62C65295B1B4ED8C1804DD979E2B13A97029CFCB3F1F96F28CE43318560F8400E2CAA5D80270FA1C90099D3D41BE00DD00010B893132108002131662342D91AFCA6330001073EA2E0054BC098804B5C00CC667B79727FF646267FA9E3971C96E71E8C00D911A9C738EC401A6CBEA33BC09B8015697BB7CD746E4A9FD4BB5613004BC01598EEE96EF755149B9A049D80480230C0041E514A51467D226E692801F049F73287F7AC29CB453E4B1FDE1F624100203368B3670200C46E93D13CAD11A6673B63A42600C00021119E304271006A30C3B844200E45F8A306C8037C9CA6FF850B004A459672B5C4E66A80090CC4F31E1D80193E60068801EC056498012804C58011BEC0414A00EF46005880162006800A3460073007B620070801E801073002B2C0055CEE9BC801DC9F5B913587D2C90600E4D93CE1A4DB51007E7399B066802339EEC65F519CF7632FAB900A45398C4A45B401AB8803506A2E4300004262AC13866401434D984CA4490ACA81CC0FB008B93764F9A8AE4F7ABED6B293330D46B7969998021C9EEF67C97BAC122822017C1C9FA0745B930D9C480')

def decode_input(input):
  #print (f"Input '{input}'")
  #print (''.join(['-' for _ in range(len(input))]))
  (_, value) = decode(to_binary(input))
  print (value)
  print ('')


def decode(input):
  if len(input) < 6: return (0, 0)
  type = to_decimal(input[3:6])

  tcnt = 6
  input = input[6:]

  # literal
  if (type == 4):
    cnt = 0
    isLast = False

    bits = []
    for c in input:
      if cnt == 0:
        if c == '0': isLast = True
      else: bits.append(c)

      cnt += 1
      tcnt += 1
      if (cnt == 5):
        cnt = 0
        if isLast:
          return (tcnt, to_decimal(bits))

  # operator
  else:
    len_type = input[0]
    sub_packets_len_binary = 11 if len_type == '1' else 15

    tcnt += 1
    input = input[1:]

    sub_packets_len = to_decimal(input[:sub_packets_len_binary])

    tcnt += sub_packets_len_binary
    input = input[sub_packets_len_binary:]
    last_decode_len = -1

    args = []

    if len_type == '0':
      scnt = 0
      while scnt < sub_packets_len:
        (last_decode_len, value) = decode(input)
        scnt += last_decode_len
        tcnt += last_decode_len
        input = input[last_decode_len:]
        args.append(value)
    else:
      for _ in range(sub_packets_len):
        (last_decode_len, value) = decode(input)
        tcnt += last_decode_len
        input = input[last_decode_len:]
        args.append(value)

    value = 0

    if type == 0: # sum
      value = sum(args)
    elif type == 1: # product
      value = 1
      for a in args:
        value *= a
    elif type == 2: # min
      value = min(args)
    elif type == 3: # max
      value = max(args)
    elif type == 5: # >
      value = 1 if args[0] > args[1] else 0
    elif type == 6: # <
      value = 1 if args[0] < args[1] else 0
    elif type == 7: # =
      value = 1 if args[0] == args[1] else 0

    return (tcnt, value)


def to_binary(input):
  to_binary = {}
  to_binary['0'] = '0000'
  to_binary['1'] = '0001'
  to_binary['2'] = '0010'
  to_binary['3'] = '0011'
  to_binary['4'] = '0100'
  to_binary['5'] = '0101'
  to_binary['6'] = '0110'
  to_binary['7'] = '0111'
  to_binary['8'] = '1000'
  to_binary['9'] = '1001'
  to_binary['A'] = '1010'
  to_binary['B'] = '1011'
  to_binary['C'] = '1100'
  to_binary['D'] = '1101'
  to_binary['E'] = '1110'
  to_binary['F'] = '1111'

  return ''.join([to_binary[b] for b in input])


def to_decimal(input):
  v = 1
  dec = 0
  for d in input[::-1]:
    dec += v if d == '1' else 0
    v *= 2

  return dec


main()
