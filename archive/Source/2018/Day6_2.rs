use std::fs;
use std::collections::HashMap;

fn main() {
    let contents = fs::read_to_string("Day6.txt")
        .expect("Something went wrong reading the file");

    let all_lines = contents.split("\n").collect::<Vec<&str>>();

    let mut _min_x = 10000;
    let mut _min_y = 10000;
    let mut _max_x = 0;
    let mut _max_y = 0;

    let mut count = 0;
    let mut _positions:HashMap<i32, (i32, i32)> = HashMap::new();
    let mut _area_sizes:HashMap<i32, i32> = HashMap::new();
    let mut _unlimited_areas:HashMap<i32, bool> = HashMap::new();

    for line in all_lines
    {
        if line == "" { continue; }

        let pos = line.split(",").collect::<Vec<&str>>();
        let x = pos[0].trim().parse::<i32>().unwrap();
        let y = pos[1].trim().parse::<i32>().unwrap();

        if _min_x > x { _min_x = x; }
        if _min_y > y { _min_y = y; }
        if _max_x < x { _max_x = x; }
        if _max_y < y { _max_y = y; }

        _positions.insert(count, (x, y));
        _unlimited_areas.insert(count, false);

        count += 1;
    }

    let mut _area_size = 0;
    for _y in _min_y..=_max_y
    {
        for _x in _min_x..=_max_x
        {
            let mut _tot_dist = 0;
            for (_count, (_px, _py)) in &_positions
            {
                let dist = (_x - _px).abs() + (_y - _py).abs();
                _tot_dist += dist;
            }

            if _tot_dist < 10000 {
                _area_size += 1;
            }
        }
    }

    println!("{}", _area_size);
}
