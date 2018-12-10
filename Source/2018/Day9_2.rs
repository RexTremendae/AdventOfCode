use std::env;
use std::time::Instant;

fn main()
{
    let _print_round = false;
    let _print_extra = false;

    let mut _num_players:usize = 9;
    let mut _max_marble:i64 = 25;

    let _args = env::args().collect::<Vec<String>>();
    if _args.len() > 1
    {
        _num_players = _args[1].parse::<usize>().unwrap();
    }
    if _args.len() > 2
    {
        _max_marble = _args[2].parse::<i64>().unwrap();
    }

    println!("Playing with {} players and max marble value of {}.", _num_players, _max_marble);
    println!();

    let mut _current_player:usize = 0;
    let mut _current_marble_number:i64 = 2;
    let mut _current_marble_position:i64 = 1;

    let mut _marbles:Vec<i64> = Vec::new();
    let mut _scores:Vec<i64> = vec![0;_num_players];

    _marbles.push(0);
    _marbles.push(1);
    let mut _marbles_len = 2;

    if _print_round
    {
        println!("[-]  (0)");
    }

    let mut _next_checkpoint = 23;
    let mut _start = Instant::now();

    loop
    {
        if _current_marble_number % 100_000 == 0
        {
            let elapsed = _start.elapsed();
            let _million:i64 = _current_marble_number / 1_000_000;
            let _thousand:i64 = (_current_marble_number - _million*1_000_000) / 100_000;
            if _million < 1
            {
                println!("  {}00 000 [{} s]", _thousand, elapsed.as_secs());
            }
            else
            {
                println!("{} {}00 000 [{} s]", _million, _thousand, elapsed.as_secs());
            }
            _start = Instant::now();
        }

        if _current_marble_number == _next_checkpoint
        {
            if _current_marble_position < 7 { _current_marble_position += _marbles_len; }
            _current_marble_position -= 7;
            _scores[_current_player] += _marbles[_current_marble_position as usize] + _current_marble_number;

            if _print_extra
            {
                println!("removed at position {}: player {} scored {} + {} = {}",
                    _current_marble_position,
                    _current_player+1,
                    _marbles[_current_marble_position as usize],
                    _current_marble_number,
                    _marbles[_current_marble_position as usize] + _current_marble_number);
            }

            _marbles.remove(_current_marble_position as usize);
            _marbles_len -= 1;
            _next_checkpoint += 23;
        }
        // Normal case
        else
        {
            _current_marble_position += 2;
            if _current_marble_position > _marbles_len { _current_marble_position -= _marbles_len; }
            _marbles.insert(_current_marble_position as usize, _current_marble_number);
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

fn print_round(_current_player: usize, _current_marble_number: i64, _marbles: Vec<i64>)
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