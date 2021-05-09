fn main()
{
    let mut posibilities = 0;

    // Puzzle input: 156218-652527
    for candidate in 156218..652528
    {
        if validate(candidate)
        {
            posibilities += 1;
        }
    }

    println!("{}", posibilities);
}

fn validate(password: i32) -> bool
{
    let pwd_string = password.to_string();

    let mut last:Option<char> = None;
    let mut has_consecutive_pair = false;

    for c in pwd_string.chars()
    {
        if last == None
        {
            last = Some(c);
            continue;
        }

        if last.unwrap() > c
        {
            return false;
        }

        if last.unwrap() == c
        {
            has_consecutive_pair = true;
        }

        last = Some(c);
    }
    return has_consecutive_pair;
}
