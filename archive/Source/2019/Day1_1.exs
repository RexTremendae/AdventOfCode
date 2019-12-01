fuelRequired = fn x -> (div x, 3) - 2 end

IO.puts(
File.stream!("Day1.txt", [], :line)
    |> Enum.map(&String.trim(&1))
    |> Enum.filter(&(&1 != ""))
    |> Enum.map(&fuelRequired.(String.to_integer(&1)))
    |> Enum.sum
)

