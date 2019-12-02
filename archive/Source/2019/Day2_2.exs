defmodule Loop do
    def execute(data, idx) do
        case Enum.at(data, idx) do
            1 -> execute(List.replace_at(data, Enum.at(data, idx+3), getValue(data, idx+1) + getValue(data, idx+2)), idx+4)
            2 -> execute(List.replace_at(data, Enum.at(data, idx+3), getValue(data, idx+1) * getValue(data, idx+2)), idx+4)
            99 -> hd(data)
        end
    end

    def find(data, goal) do
        find(data, goal, 0, 0)
    end

    def find(data, goal, noun, verb) do
        case execute(setup(data, noun, verb), 0) do
            ^goal -> {noun, verb}
            _ when noun == 99 -> find(data, goal, 0, verb+1)
            _ -> find(data, goal, noun+1, verb)
        end
    end

    def setup(data, noun, verb) do
        List.replace_at(List.replace_at(data, 1, noun), 2, verb)
    end

    def getValue(data, idx) do
        Enum.at(data, Enum.at(data, idx))
    end
end


data = [1,0,0,3,1,1,2,3,1,3,4,3,1,5,0,3,2,6,1,19,2,19,9,23,1,23,5,27,2,6,27,31,1,31,5,35,1,35,5,39,2,39,6,43,2,43,10,47,1,47,6,51,1,51,6,55,2,55,6,59,1,10,59,63,1,5,63,67,2,10,67,71,1,6,71,75,1,5,75,79,1,10,79,83,2,83,10,87,1,87,9,91,1,91,10,95,2,6,95,99,1,5,99,103,1,103,13,107,1,107,10,111,2,9,111,115,1,115,6,119,2,13,119,123,1,123,6,127,1,5,127,131,2,6,131,135,2,6,135,139,1,139,5,143,1,143,10,147,1,147,2,151,1,151,13,0,99,2,0,14,0]
goal = 19690720

nounVerb = Tuple.to_list(Loop.find(data, goal))
IO.puts(100 * Enum.at(nounVerb, 0) + Enum.at(nounVerb, 1))

