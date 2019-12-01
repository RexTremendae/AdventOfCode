use std::fs;

fn main() {
    let contents = fs::read_to_string("Day2.txt")
        .expect("Something went wrong reading the file");

    let mut two_count = 0;
    let mut three_count = 0;

    for mut line in contents.split("\n")
    {
        let mut _lettercount:[i32;150] = [0;150];
        line = line.trim_matches('\r');
        if line == "" { continue; }

        for letter in line.as_bytes()
        {
            let idx = *letter as usize;
            _lettercount[idx] = _lettercount[idx] + 1;
        }

        let mut has_two = false;
        let mut has_three = false;

        for count in _lettercount.iter()
        {
            if *count == 2
            {
                has_two = true;
            }
            if *count == 3 
            {
                has_three = true;
            }
        }

        if has_two { two_count = two_count + 1 }
        if has_three { three_count = three_count + 1; }

        println!("{}: 2[{}] 3[{}]", line, has_two, has_three);
    }

    println!("{} * {} = {}", two_count, three_count, two_count * three_count);
}

