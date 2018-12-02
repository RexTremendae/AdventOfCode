use std::fs;

fn main() {
    let contents = fs::read_to_string("Day2.txt")
        .expect("Something went wrong reading the file");

    let mut _lines = Vec::new();
    for mut line in contents.split("\n")
    {
        line = line.trim_matches('\r');
        if line == "" { continue; }
        _lines.push(line);
    }

    let mut i = 0;
    let mut j = 0;
    let mut stop;

    loop
    {
        loop
        {
            j = j + 1;
            if j >= _lines.len()
            {
                i = i + 1;
                if i >= _lines.len() - 1 { stop = true; break; }

                j = i + 1;
            }

            let _line1 = _lines[i];
            let _line2 = _lines[j];

            //println!("{} {}", i, j);
            let mut line2iter = _line2.char_indices();
            let mut diffs = 0;
            for (_idx, chr1) in _line1.chars().enumerate()
            {
                let chr2 = line2iter.next().unwrap().1;
                //println!("  {} {} {}", _idx, chr1, chr2);

                if chr1 != chr2
                {
                    diffs = diffs + 1;
                    if diffs > 1
                    {
                        continue;
                    }
                }
            }

            if diffs == 1
            {
                let mut line2iter = _line2.char_indices();
                for (_idx, chr1) in _line1.chars().enumerate()
                {
                    let chr2 = line2iter.next().unwrap().1;
                    if chr1 == chr2
                    {
                        print!("{}", chr1);
                    }
                }

                println!();
            }
        }

        if stop { break; }
    }
}

