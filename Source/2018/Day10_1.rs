use std::fs;

fn main()
{
    let mut data = parse_input();
    let mut _sec = 0;

    let mut _min_x = 100_000;
    let mut _min_y = 100_000;
    let mut _max_x = 0;
    let mut _max_y = 0;

    loop
    {
        _min_x = 100_000;
        _min_y = 100_000;
        _max_x = 0;
        _max_y = 0;

        for (_idx, (_, _vel)) in data.clone().iter().enumerate()
        {
            (data[_idx].0).0 += _vel.0;
            (data[_idx].0).1 += _vel.1;

            let x = (data[_idx].0).0;
            let y = (data[_idx].0).1;

            if x < _min_x { _min_x = x; }
            if y < _min_y { _min_y = y; }
            if x > _max_x { _max_x = x; }
            if y > _max_y { _max_y = y; }
        }

        _sec += 1;

        if _max_x - _min_x < 80 && _max_y - _min_y < 15
        {
            println!("{}: ", _sec);
            println!("{},{} - {},{}", _min_x, _min_y, _max_x, _max_y);
            visualize(data.clone(), (_min_x, _min_y), (_max_x, _max_y));
            println!();
        }

        if _sec > 11000 { break; }
    }
}

fn visualize(_data: Vec<((i64, i64), (i64, i64))>, _min_pos: (i64, i64), _max_pos: (i64, i64))
{
    // Create BBOX filled with '.'
    let mut rows:Vec<Vec<char>> = Vec::new();
    for _y in _min_pos.1..=_max_pos.1
    {
        let mut row:Vec<char> = Vec::new();
        for _x in _min_pos.0..=_max_pos.0
        {
            row.push('.');
        }

        rows.push(row);
    }

    // Add '#':s
    for (_pos, _) in _data
    {
        let x = _pos.0;
        let y = _pos.1;

        //if x >= 0 && x < *BBOX_WIDTH && y >= 0 && y < *BBOX_HEIGHT
        {
            rows[(y - _min_pos.1) as usize][(x - _min_pos.0) as usize] = '#';
        }
    }

    // Print!
    for _y in _min_pos.1..=_max_pos.1
    {
        for _x in _min_pos.0..=_max_pos.0
        {
            print!("{}", rows[(_y - _min_pos.1) as usize][(_x - _min_pos.0) as usize]);
        }
        println!();
    }
}

fn parse_input() -> Vec<((i64, i64), (i64, i64))>
{
    let contents = fs::read_to_string("Day10.txt")
        .expect("Something went wrong reading the file");

    let mut result:Vec<((i64, i64), (i64, i64))> = Vec::new();
    for mut line in contents.split("\n").collect::<Vec<&str>>()
    {
        line = line.trim_matches('\r');
        let mut _pos = "";
        let mut _vel = "";

        for (idx, chr) in line.chars().enumerate()
        {
            if chr == 'v'
            {
                _pos = &line[0..idx-1];
                _vel = &line[idx..];
                break;
            }
        }

        if _pos == "" { break; }

        let _pos = extract_int_pair(_pos);
        let _vel = extract_int_pair(_vel);

        //println!("pos: ({}, {})  vel: ({}, {})", _pos.0, _pos.1, _vel.0, _vel.1);

        result.push((_pos, _vel));
    }
    result
}

fn extract_int_pair(_input: &str) -> (i64, i64)
{
    let mut _coord_raw = _input.split("=").collect::<Vec<&str>>()[1];
    _coord_raw = &_coord_raw[1.._coord_raw.len()-1];
    let _coord_raw = _coord_raw.split(",").collect::<Vec<&str>>();
    (_coord_raw[0].trim().parse::<i64>().unwrap(), _coord_raw[1].trim().parse::<i64>().unwrap())
}
