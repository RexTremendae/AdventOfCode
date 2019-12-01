fuelRequired = fn 
    (x, _) when x < 9 -> 0
    (x, recurse) -> (div x, 3) - 2 + recurse.((div x, 3) - 2, recurse)
end

IO.puts(
File.stream!("Day1.txt", [], :line)
    |> Enum.map(&String.trim(&1))
    |> Enum.filter(&(&1 != ""))
    |> Enum.map(&fuelRequired.(String.to_integer(&1), fuelRequired))
    |> Enum.sum
)

