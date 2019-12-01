use std::fs;

fn main() {
    const SIZE:usize = 100000;
    let mut positives:[bool; SIZE] = [false; SIZE];
    let mut negatives:[bool; SIZE] = [false; SIZE];

    let contents = fs::read_to_string("Day1.txt")
        .expect("Something went wrong reading the file");

    let mut total = 0;
    let mut do_break = false;

    loop
    {
        let all_lines = contents.split("\n");
        for mut line in all_lines
        {
            line = line.trim_matches('\r');
            if line == "" { continue; }
            let _parsed = line.parse::<i32>().unwrap();
            total += _parsed;

            if total >= 0
            {
                if positives[total as usize]
                {
                    println!("{}", total);
                    do_break = true;
                    break;
                }

                positives[total as usize] = true;
            }
            else
            {
                if negatives[-total as usize]
                {
                    println!("{}", total);
                    do_break = true;
                    break;
                }

                negatives[-total as usize] = true;
            }
        }

        if do_break { break; }
    }
}

