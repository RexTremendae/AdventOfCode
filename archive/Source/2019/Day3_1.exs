defmodule Wires do
    def unpack(wire, map) do
        case wire do
            [] -> map
            wire2 -> map = unpackMove(hd(wire2), map)
                     unpack(tl(wire2), map)
        end
    end

    def unpackMove(wire, map) do
        chr = to_charlist(wire)
        unpackMove(hd(chr), String.to_integer(List.to_string(tl(chr))), map, map[:pos])
    end

    def unpackMove(dir, length, map, pos) do
        case dir do
            _ when length == 0 -> map
            # 'U'
            85 -> updateMap(dir, length, map, up(pos))

            # 'D'
            68 -> updateMap(dir, length, map, down(pos))

            # 'L'
            76 -> updateMap(dir, length, map, left(pos))

            # 'R'
            82 -> updateMap(dir, length, map, right(pos))
        end
    end

    def updateMap(dir, length, map, newpos) do
        map = Map.replace!(map, :pos, newpos)
        map = Map.replace!(map, :min, minManhattan(map[:min], Map.get(map, newpos)))
        map = Map.put_new(map, newpos, newpos)
        unpackMove(dir, length-1, map, newpos)
    end

    def newMin(pos1, pos2) do
        case pos1 do
            p1 when p1 != pos2 -> pos2
            _ -> nil
        end
    end

    def minManhattan(pos1, pos2) do
        case {pos1, pos2} do
            {nil, _} -> pos2
            {_, nil} -> pos1
            {p1, p2} when abs(elem(p1,0)) + abs(elem(p1,1)) < abs(elem(p2,0)) + abs(elem(p2,1)) -> pos1
            _ -> pos2
        end
    end

    def up(pos) do
        {elem(pos, 0), elem(pos, 1)-1}
    end

    def down(pos) do
        {elem(pos, 0), elem(pos, 1)+1}
    end

    def left(pos) do
        {elem(pos, 0)-1, elem(pos, 1)}
    end

    def right(pos) do
        {elem(pos, 0)+1, elem(pos, 1)}
    end
end


map = %{:pos => {0,0}, :min => nil}
data = File.stream!("Day3.txt", [], :line)
    |> Enum.map(&String.trim(&1))
    |> Enum.map(&String.split(&1, ","))

map = Wires.unpack(hd(data), map)
map = %{ map | :pos => {0, 0}, :min => nil}
map = Wires.unpack(hd(tl(data)), map)

min = map[:min]
IO.inspect(min)
IO.puts(abs(elem(min, 0)) + abs(elem(min, 1)))
