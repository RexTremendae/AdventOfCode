use std::env;

fn main()
{
    let _print_round = false;
    let _print_extra = false;

    let mut _num_players:usize = 9;
    let mut _max_marble:i32 = 25;

    let _args = env::args().collect::<Vec<String>>();
    if _args.len() > 1
    {
        _num_players = _args[1].parse::<usize>().unwrap();
    }
    if _args.len() > 2
    {
        _max_marble = _args[2].parse::<i32>().unwrap();
    }

    println!("Playing with {} players and max marble value of {}.", _num_players, _max_marble);
    println!();

    let mut _current_player:usize = 0;
    let mut _current_marble_number = 1;
    let mut _current_marble_position = 1;

    let mut _marbles:Vec<i32> = Vec::new();
    let mut _scores:Vec<i32> = vec![0;_num_players];

    _marbles.push(0);
    let mut _marbles_len = 1;

    if _print_round
    {
        println!("[-]  (0)");
    }

    loop
    {
        // Special case for starting condition
        if _marbles_len == 1
        {
            _marbles.insert(_current_marble_position, _current_marble_number);
            _marbles_len += 1;
        }
        // Special case, % 23
        else if _current_marble_number % 23 == 0
        {
            if _current_marble_position < 7 { _current_marble_position += _marbles_len; }
            _current_marble_position -= 7;
            _scores[_current_player] += _marbles[_current_marble_position] + _current_marble_number;

            if _print_extra
            {
                println!("removed at position {}: player {} scored {} + {} = {}",
                    _current_marble_position,
                    _current_player+1,
                    _marbles[_current_marble_position],
                    _current_marble_number,
                    _marbles[_current_marble_position] + _current_marble_number);
            }

            _marbles.remove(_current_marble_position);
            _marbles_len -= 1;
        }
        // Normal case
        else
        {
            _current_marble_position += 2;
            if _current_marble_position > _marbles_len { _current_marble_position -= _marbles_len; }
            _marbles.insert(_current_marble_position, _current_marble_number);
            _marbles_len += 1;
        }

        if _print_round
        {
            print_round(_current_player, _current_marble_number, _marbles.clone());
        }

        _current_marble_number += 1;
        _current_player += 1;

        if _current_player >= _num_players { _current_player = 0; }
        if _current_marble_number > _max_marble { break; }
    }

    println!();
    let mut _max_score = 0;
    for (_p, _s) in _scores.iter().enumerate()
    {
        if _print_extra
        {
            println!("Player {} got score {}", _p+1, _s);
        }
        if _max_score < *_s { _max_score = *_s; }
    }

    println!();
    println!("Max score was {}", _max_score);
    println!();
}

fn print_round(_current_player: usize, _current_marble_number: i32, _marbles: Vec<i32>)
{
    print!("[{}] ", _current_player+1);
    for _m in _marbles.iter()
    {
        if _m == &_current_marble_number
        {
            print!(" ({}) ", _m);
        }
        else
        {
            print!(" {} ", _m);
        }
    }
    println!();
}