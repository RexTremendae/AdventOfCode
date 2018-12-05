use std::fs;
use std::collections::HashMap;

fn main() {
    let contents = fs::read_to_string("Day4.txt")
        .expect("Something went wrong reading the file");

    let mut all_lines = contents.split("\n").collect::<Vec<&str>>();
    all_lines.sort();
    let mut _guard_id = -1;
    let mut _sleep_minutes:HashMap<(i32, i32), i32> = HashMap::new();
    let mut _sleep_start = 0;
    let mut _guard_ids:Vec<i32> = Vec::new();

    for line in all_lines
    {
        if line == "" { continue; }

        //    0         1      2    3    4      5
        //[1518-11-01 00:00] Guard #10 begins shift
        //[1518-11-01 00:05] falls asleep
        //[1518-11-01 00:25] wakes up

        let parts = line.split(" ").collect::<Vec<&str>>();
        let _time = parts[1][..5].split(":").collect::<Vec<&str>>();
        let _hour = _time[0].parse::<i32>().unwrap();
        let _min = _time[1].parse::<i32>().unwrap();
        let _date = parts[0][1..].split("-").collect::<Vec<&str>>();
        let mut _year = _date[0].parse::<i32>().unwrap();
        let mut _month = _date[1].parse::<i32>().unwrap();
        let mut _day = _date[2].parse::<i32>().unwrap();

        let (_year, _month, _day) = get_guard_date(_year, _month, _day, _hour);

        if parts[2] == "Guard"
        {
            _guard_id = parts[3][1..].parse::<i32>().unwrap();
            _guard_ids.push(_guard_id);
            //println!("[{}-{}-{}] Guard {}", _year, _month, _day, _guard_id);
            continue;
        }
        else if parts[2] == "falls"
        {
            _sleep_start = _min;
            //println!("[{}-{}-{}] Guard {} falls asleep at {}", _year, _month, _day, _guard_id, _min);
            continue;
        }
        else if parts[2] == "wakes"
        {
            //println!("[{}-{}-{}] Guard {} wakes up at {}", _year, _month, _day, _guard_id, _min);
            for mut i in _sleep_start.._min
            {
                *_sleep_minutes.entry((_guard_id, i)).or_insert(0) += 1;
            }

            continue;
        }
    }

    let mut max_id = 0;
    let mut max_minute = 0;
    let mut max_count = 0;

    for _id in _guard_ids
    {
        //println!("found #{}", _id);
        for _minute in 0..60
        {
            match _sleep_minutes.get(&(_id, _minute))
            {
                Some(count) =>
                {
                    //println!("#{} {} min: {}", max_id, max_minute, max_count);
                    if max_count < *count
                    {
                        max_count = *count;
                        max_id = _id;
                        max_minute = _minute;
                    }
                },
                None => {}
            }
        }
    }

    println!("{}", max_id * max_minute);
}

fn get_guard_date(mut year: i32, mut month: i32, mut day: i32, hour: i32) -> (i32, i32, i32)
{
    if hour == 0
    {
        return (year, month, day);
    }

    day = day + 1;

    // 31 day month
    if [1, 3, 5, 7, 8, 10, 12].contains(&month)
    {
        if day > 31
        {
            day = 1;
            month = month + 1;
        }
    }
    // 30 day month
    else if [4, 6, 9, 11].contains(&month)
    {
        if day > 30
        {
            day = 1;
            month = month + 1;
        }
    }
    // February
    else
    {
        if day > 28
        {
            day = 1;
            month = month + 1;
        }
    }

    if month > 12
    {
        year = year + 1;
        month = 1;
    }

    (year, month, day)
}
