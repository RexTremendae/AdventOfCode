use std::fs;

fn main() {
    let contents = fs::read_to_string("Day1.txt")
        .expect("Something went wrong reading the file");

    let mut total = 0;
    for mut line in contents.split("\n")
    {
        line = line.trim_matches('\r');
        if line == "" { continue; }
        let _parsed = line.parse::<i32>();
        total += _parsed.unwrap();
    }

    println!("{}", total);
}

