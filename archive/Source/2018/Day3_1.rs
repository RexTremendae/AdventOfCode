use std::fs;

fn main() {
    let contents = fs::read_to_string("Day3.txt")
        .expect("Something went wrong reading the file");

    const SIZE:usize = 1000;
    let mut _matrix = vec![vec![0i32; SIZE]; SIZE];

    for _y in 0..SIZE
    {
        for _x in 0..SIZE
        {
            _matrix[_y][_x] = 0;
        }
    }

    for mut line in contents.split("\n")
    {
        if line == "" { continue; }

//        println!("Split ' '");
        let _parts:Vec<&str> = line.split(" ").collect::<Vec<&str>>();
        let _idsub = (&_parts[0][1..]).parse::<i32>().unwrap();

//        println!("Parse pos");
        let _pos_raw = &_parts[2];
        let _pos = &(_pos_raw)[.._pos_raw.len()-1];
        let _pos_split = _pos.split(",").collect::<Vec<&str>>();
        let _x = _pos_split[0].parse::<i32>().unwrap();
        let _y = _pos_split[1].parse::<i32>().unwrap();

//        println!("Parse size");
        let _size_raw = &_parts[3];
        let _size = &(_size_raw)[.._size_raw.len()-1];
        let _size_split = _size.split("x").collect::<Vec<&str>>();
        let _w = _size_split[0].parse::<i32>().unwrap();
        let _h = _size_split[1].parse::<i32>().unwrap();

 //       println!("#{} [{}, {}] {}x{}", _idsub, _x, _y, _w, _h);
        for _yy in 0.._h
        {
            for _xx in 0.._w
            {
                let _pos_y = (_yy + _y) as usize;
                let _pos_x = (_xx + _x) as usize;
                _matrix[_pos_y][_pos_x] = _matrix[_pos_y][_pos_x] + 1;
            }
        }
    }

    let mut total = 0;

    for _yy in 0..SIZE
    {
        for _xx in 0..SIZE
        {
            if _matrix[_yy as usize][_xx as usize] > 1 { total = total + 1; }
        }
    }

    println!("{}", total);
}
