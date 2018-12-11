use std::env;

fn main()
{
    const SIZE:i32 = 300;
    let mut _serial_no = 8;
    let _args = env::args().collect::<Vec<String>>();
    if _args.len() > 1
    {
        _serial_no= _args[1].parse::<i32>().unwrap();
    }

    println!("Grid serial no: {}", _serial_no);
    println!();

    let mut power:Vec<Vec<i32>> = Vec::new();

    for _y in 0..SIZE
    {
        let mut power_row:Vec<i32> = Vec::new();
        for _x in 0..SIZE
        {
            let rack_id = _x+1 + 10;
            let mut power_level = rack_id * (_y+1) + _serial_no;
            power_level = power_level * rack_id;
            let mut hundred = 0;
            if power_level > 99
            {
                hundred = power_level / 100;
                hundred = hundred - (hundred / 10) * 10;
            }

            power_row.push(hundred-5);
            //print!("{} ", hundred - 5);
        }
        power.push(power_row);
        //println!();
    }

    let mut global_max = 0;
    for size in 1..=SIZE
    {
        println!("Trying size {}x{}", size, size);
        for ys in 0..=SIZE-size
        {
            for xs in 0..=SIZE-size
            {
                let mut local_max = 0;
                for y in ys..ys+size
                {
                    for x in xs..xs+size
                    {
                        local_max += power[y as usize][x as usize];
                    }
                }

                if local_max > global_max
                {
                    println!("New max found: {} [{}, {}] {}x{}", local_max, xs+1, ys+1, size, size);
                    global_max = local_max;
                }
            }
        }
    }

    println!();
    //println!("Global max: {}", max_power);
}
