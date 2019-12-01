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

    let mut max_power = 0;

    let mut power_row1:Vec<i32> = Vec::new();
    let mut power_row2:Vec<i32> = Vec::new();
    let mut power_row3:Vec<i32> = Vec::new();

    for _y in 0..SIZE
    {
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

            power_row1.push(hundred-5);
            //print!("{} ", hundred - 5);

            if _x > 1 && _y > 1
            {
                let mut tot_power = 0;
                for _mx in (_x-2) as usize ..= _x as usize
                {
                    tot_power += power_row1[_mx] + power_row2[_mx] + power_row3[_mx];
                }

                if tot_power > max_power
                {
                    max_power = tot_power;

                    println!("New max found: {} [{}, {}]", max_power, _x-1, _y-1);
                }
            }
        }

        power_row3 = power_row2;
        power_row2 = power_row1;
        power_row1 = Vec::new();
        //println!();
    }

    println!();
    println!("Global max: {}", max_power);
}
