fn main()
{
    //let input = "80871224585914546619083218645595";
    let input = "59764635797473718052486376718142408346357676818478503599633670059885748195966091103097769012608550645686932996546030476521264521211192035231303791868456877717957482002303790897587593845163033589025995509264282936119874431944634114034231860653524971772670684133884675724918425789232716494769777580613065860450960426147822968107966020797566015799032373298777368974345143861776639554900206816815180398947497976797052359051851907518938864559670396616664893641990595511306542705720282494028966984911349389079744726360038030937356245125498836945495984280140199805250151145858084911362487953389949062108285035318964376799823425466027816115616249496434133896";
    let result = calculate_phases(input.chars().collect::<Vec<char>>(), 100);

    for j in 0..8
    {
        print!("{}", result[j]);
    }
    println!("");
}

fn calculate_phases(input_signal: Vec<char>, phase_count: u32) -> Vec<char>
{
    let mut next_signal = input_signal;

    for _i in 0..phase_count
    {
        next_signal = generate_next(next_signal);
    }

    return next_signal;
}

fn get_pattern(i: usize, j: usize) -> i32
{
    let i = (i+1) as i32;
    let j = (j+1) as i32;

    let q = j / i;

    if q % 2 == 0
    {
        return 0 as i32;
    }

    if ((q-1) / 2) % 2 == 1
    {
        return -1 as i32;
    }

    return 1 as i32;
}

fn generate_next(input_signal: Vec<char>) -> Vec<char>
{
    let mut next_signal = Vec::<char>::new();

    for i in 0..input_signal.len()
    {
        let mut sum = 0;
        for j in 0..(input_signal.len())
        {
            let digit = input_signal[j].to_digit(10).unwrap() as i32;
            sum += digit * get_pattern(i, j);
        }
        let sum_fmt = format!("{}", sum).chars().collect::<Vec<char>>();
        next_signal.push(sum_fmt[sum_fmt.len()-1]);
    }

    return next_signal;
}
