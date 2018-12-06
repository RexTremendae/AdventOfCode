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

    for _y in _min_y..=_max_y
    {
        for _x in _min_x..=_max_x
        {
            let mut _smallest_dist = 10000;
            let mut _smallest_dist_pos = -1;
            for (_count, (_px, _py)) in &_positions
            {
                let dist = (_x - _px).abs() + (_y - _py).abs();
                if dist < _smallest_dist
                {
                    _smallest_dist = dist;
                    _smallest_dist_pos = *_count;
                }
                else if dist == _smallest_dist
                {
                    _smallest_dist_pos = -1;
                }
            }

            if _smallest_dist_pos >= 0
            {
                *_area_sizes.entry(_smallest_dist_pos).or_insert(0) += 1;

                if _y == _min_y || _y == _max_y || _x == _min_x || _x == _max_x
                {
                    *_unlimited_areas.get_mut(&_smallest_dist_pos).unwrap() = true;
                }
            }
        }
    }

    let mut _largest_index = 0;
    let mut _largest_area = 0;
    for (_count, _size) in _area_sizes
    {
        if !_unlimited_areas.get(&_count).unwrap() && _size > _largest_area
        {
            _largest_area = _size;
            _largest_index = _count;
        }
    }

    println!("#{}: {}", _largest_index, _largest_area);
}
