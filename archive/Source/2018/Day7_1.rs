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

    let _solution = solve(&_all_steps);
    println!();
    println!("Solution: {}", _solution);

    println!();
    print_steps (_all_steps);
}

fn solve(_steps: &Vec<Step>) -> String
{
    let mut _available:HashSet<usize> = HashSet::new();
    let mut _finished:HashSet<usize> = HashSet::new();
    let mut _goal:usize = 0;

    for (_idx, _step) in _steps.iter().enumerate()
    {
        if _step.next.len() == 0 { _goal = _idx; }
        if _step.prev.len() == 0 { _available.insert(_idx); }
    }

    let mut _solution = "".to_string();
    println!("Found goal: {}", _steps[_goal]);

    loop
    {
        let mut _to_execute:usize = 0;
        let mut _available_vec:Vec<(usize, String)> = Vec::new();
        println!();
        print!("Unsorted: ");
        for _idx in _available.iter()
        {
            print!("{}", _steps[*_idx].name);
        }
        println!();
        for _idx in _available.iter()
        {
            let mut _pos:usize = 0;
            let _name = (&_steps[*_idx].name).to_string();
            for (_i, _step) in _available_vec.iter()
            {
                if _step >= &_name { break; }
                _pos += 1;
            }

            //println!("Inserting {} at {}", _name, _pos);
            _available_vec.insert(_pos, (*_idx, _name));
        }
        print!("Sorted: ");
        for _idx in _available_vec.iter()
        {
            print!("{}", _idx.1);
        }
        println!();
        for _idx in _available_vec.iter()
        {
            let mut can_execute = true;
            for _prev in &_steps[_idx.0].prev
            {
                if !_finished.contains(&_prev) { can_execute = false; break; }
            }

            if can_execute
            {
                _to_execute = _idx.0;
                break;
            }
        }
        _available.remove(&_to_execute);
        _finished.insert(_to_execute);
        _solution += &_steps[_to_execute].name.to_string();

        for _next in &_steps[_to_execute].next { _available.insert(*_next); }
        if _to_execute == _goal { break; }
    }

    _solution
}

fn print_steps(steps: Vec<Step>)
{
    for (idx, step) in steps.iter().enumerate()
    {
        println!("{}: {}", idx, step);
    }
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
