use std::fs;

#[derive(Clone)]
struct Node
{
    metadata_count: Option<usize>,
    metadata: Vec<i32>,
    child_count: usize,
    children: Vec<usize>,
    parent: usize
}

fn main()
{
    let _nodes = parse_input();
}

fn parse_input() -> Vec<Node>
{
    let contents = fs::read_to_string("Day8.txt")
        .expect("Something went wrong reading the file");

    let mut _all_nodes:Vec<Node> = Vec::new();
    _all_nodes.push(Node { metadata_count: Some(0), metadata: Vec::new(), child_count: 1, children: Vec::new(), parent: 0 });
    let mut current_idx:usize = 0;
    println!("Added root node");
    let mut tot_meta = 0;

    let all_lines = contents.split(" ").collect::<Vec<&str>>();
    let mut idx = 0;

    loop
    {
        if idx >= all_lines.len() { break; }
        let line = all_lines[idx].trim_matches('\r').trim_matches('\n');
        if line == "" { continue; }
        let mut data:i32 = 0;

        match line.parse::<i32>()
        {
            Ok(l) => { data = l; }
            Err(e) => { println!("Error parsing '{}': {}", line, e); }
        }

        let mut _current_node = &_all_nodes.clone()[current_idx];

        // No metadata counter
        if _current_node.metadata_count == None
        {
            (&mut _all_nodes[current_idx]).metadata_count = Some(data as usize);
            println!("Added meta data count: {}", data);
        }
        // Missing a few children...
        else if _current_node.children.len() < _current_node.child_count
        {
            let parent_idx = current_idx;
            current_idx = _all_nodes.len();
            &_all_nodes[parent_idx].children.push(current_idx);
            _all_nodes.push(Node { child_count: data as usize, children: Vec::new(), metadata_count: None, metadata: Vec::new(), parent: parent_idx });
            println!("Added child with {} children", data);
        }
        // All children added, only metadata left
        else if _current_node.metadata.len() < _current_node.metadata_count.unwrap()
        {
            &_all_nodes[current_idx].metadata.push(data);
            tot_meta += data;
            println!("Added meta data: {}", data);
        }
        // Node full, backtrack to unfinished parent
        else
        {
            loop
            {
                current_idx = _current_node.parent;
                _current_node = &_all_nodes[current_idx];
                println!("Backtracking...");

                if _current_node.children.len() < _current_node.child_count || _current_node.metadata.len() < _current_node.metadata_count.unwrap()
                {
                    break;
                }
            }
            idx -= 1;
        }

        idx += 1;
    }

    println!("{}", tot_meta);
    _all_nodes
}
