use std::fmt;
use std::fmt::{Display, Formatter};
use std::fs;
use std::collections::HashMap;
use std::collections::HashSet;
use std::collections::hash_map::Entry::{Occupied, Vacant};

struct Step
{
    name: String,
    next: Vec<usize>,
    prev: Vec<usize>
}

impl Display for Step {
    fn fmt(&self, f: &mut Formatter) -> fmt::Result {
        let mut _prev = String::new();
        for _idx in &self.prev
        {
            if _prev != "" { _prev += ", "; }
            _prev += &_idx.to_string();
        }

        let mut _next = String::new();
        for _idx in &self.next
        {
            if _next != "" { _next += ", "; }
            _next += &_idx.to_string();
        }

        write!(f, "[{}] -> {} -> [{}]", _prev, self.name, _next)
    }
}

fn main()
{
    let _all_steps = parse_input();

    println!("Solution reached in {} s", solve(&_all_steps));
}

fn solve(_steps: &Vec<Step>) -> i32
{
    const NUM_WORKERS:i32 = 5;
    const OFFSET:i32 = 60;

    let mut _available_candidates:HashSet<usize> = HashSet::new();
    let mut _finished:HashSet<usize> = HashSet::new();
    let mut _in_progress:HashMap<usize, i32> = HashMap::new();
    let mut _goal:usize = 0;

    for (_idx, _step) in _steps.iter().enumerate()
    {
        if _step.next.len() == 0 { _goal = _idx; }
        if _step.prev.len() == 0 { _available_candidates.insert(_idx); }
    }

    let mut _solution = "".to_string();
    println!("Found goal: {}", _steps[_goal]);
    let mut _second = 0;

    loop
    {
        let mut _available_vec:Vec<(usize, String)> = Vec::new();
/*
        println!();
        print!("Unsorted: ");
        for _idx in _available.iter()
        {
            print!("{}", _steps[*_idx].name);
        }
        println!();
*/

        // Sort available steps
        for _idx in _available_candidates.iter()
        {
            let mut can_execute = true;
            //println!("Evaluating if step {} is executable", _steps[*_idx].name);
            for _prev in &_steps[*_idx].prev
            {
                if !_finished.contains(&_prev)
                {
                    //println!("Missing prev step {}", _steps[*_prev].name);
                    can_execute = false;
                    break;
                }
            }

            if !can_execute
            {
                continue;
            }

            let mut _pos:usize = 0;
            let _name = (&_steps[*_idx].name).to_string();
            for (_i, _step) in _available_vec.iter()
            {
                if _step >= &_name { break; }
                _pos += 1;
            }

            //println!("Inserting {} at {}", _name, _pos);
            //println!("Added {} to available steps", _steps[*_idx].name);
            _available_vec.insert(_pos, (*_idx, _name));
        }
/*
        print!("Sorted: ");
        for _idx in _available_vec.iter()
        {
            print!("{}", _idx.1);
        }
        println!();
*/

        // Find steps that can be executed
        let mut _to_execute:Vec<usize> = Vec::new();
        for _idx in _available_vec.iter()
        {
            let mut can_execute = true;
            for _prev in &_steps[_idx.0].prev
            {
                if !_finished.contains(&_prev) { can_execute = false; break; }
            }

            if can_execute
            {
                //println!("Added {} to can_execute", &_steps[_idx.0].name);
                _to_execute.push(_idx.0);
            }
        }

        print!("{}: ", _second);

        for (_step, _seconds) in _in_progress.clone().iter()
        {
            let _step_seconds = _steps[*_step].name.chars().next().unwrap() as i32 - 'A' as i32 + 1 + OFFSET;
            println!("{} {}/{}", _steps[*_step].name, *_in_progress.get_mut(_step).unwrap()+1, _step_seconds);
            //if (_seconds >= _all_steps[_step])
            *_in_progress.get_mut(_step).unwrap() += 1;
            if *_in_progress.get_mut(_step).unwrap() >= _step_seconds
            {
                println!("Finished step {}", _steps[*_step].name);
                _solution += &_steps[*_step].name.to_string();
                _finished.insert(*_step);
                for _next in &_steps[*_step].next
                {
                    //println!("New available step: {}", &_steps[*_next].name);
                    _available_candidates.insert(*_next);
                }
            }
        }

        for _exec in _to_execute.clone().iter()
        {
            if _in_progress.len() >= NUM_WORKERS as usize
            {
                break;
            }

            println!("Starting task {}", _steps[*_exec].name);
  
            let _step_seconds = _steps[*_exec].name.chars().next().unwrap() as i32 - 'A' as i32 + 1 + OFFSET;
            if _step_seconds == 1
            {
                println!("Finished step {}", _steps[*_exec].name);
                _solution += &_steps[*_exec].name.to_string();
                _finished.insert(*_exec);
                for _next in &_steps[*_exec].next
                {
                    //println!("New available step: {}", &_steps[*_next].name);
                    _available_candidates.insert(*_next);
                }
            }
            else
            {
                _in_progress.insert(*_exec, 1);
            }

            _available_candidates.remove(&_exec);
        }
        println!();
        //_solution += &_steps[_to_execute].name.to_string();
        //for _next in &_steps[_to_execute].next { _available.insert(*_next); }
        //if _to_execute == _goal { break; }

        let mut _reached_goal = false;
        for _fin in _finished.clone()
        {
            _in_progress.remove(&_fin);
            if _fin == _goal
            {
                _reached_goal = true;
                break;
            }
        }
        println!("{}", _solution);

        _second += 1;

        if _reached_goal
        {
            break;
        }
    }

    _second
}

fn parse_input() -> Vec<Step>
{
    let mut _all_steps:Vec<Step> = Vec::new();
    let mut _steps_by_index:HashMap<String, usize> = HashMap::new();

    let contents = fs::read_to_string("Day7.txt")
        .expect("Something went wrong reading the file");

    let all_lines = contents.split("\n").collect::<Vec<&str>>();
    for line in all_lines
    {
        if line == "" { continue; }

        let pos = line.split(" ").collect::<Vec<&str>>();
        let _name1 = pos[1].to_string();
        let _name2 = pos[7].to_string();

        let _name1_clone = _name1.to_string();
        match _steps_by_index.entry(_name1) {
            Occupied(_entry) => {},
            Vacant(_entry) =>
            {
                let idx = _all_steps.len();
                _entry.insert(idx);
                _all_steps.push(Step {name: _name1_clone, next: Vec::new(), prev: Vec::new()});
            }
        }

        let _name2_clone = _name2.to_string();
        match _steps_by_index.entry(_name2) {
            Occupied(_entry) => {},
            Vacant(_entry) =>
            {
                let idx = _all_steps.len();
                _entry.insert(idx);
                _all_steps.push(Step {name: _name2_clone, next: Vec::new(), prev: Vec::new()});
            }
        }

        let _name1 = pos[1].to_string();
        let _name2 = pos[7].to_string();

        let _name1_idx = _steps_by_index[&_name1];
        let _name2_idx = _steps_by_index[&_name2];
        _all_steps[_name1_idx].next.push(_name2_idx);
        _all_steps[_name2_idx].prev.push(_name1_idx);
    }

    _all_steps
}
