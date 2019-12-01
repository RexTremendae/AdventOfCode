using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Console;

namespace Day13
{
    public enum Direction { Up, Right, Down, Left }

    public enum Turn { Left, Straight, Right }

    public class Cart
    {
        public (int X, int Y) Position { get; set; }
        public Direction Direction { get; set; }
        public Turn NextTurn { get; set; }
    }

    public class Program
    {
        public static void Main()
        {
            new Program().Run();
        }

        private void Run()
        {
            var data = ParseInput("Day13.txt");

            var orderedCarts = data.carts.OrderBy(x => x.Position.Y).ThenBy(x => x.Position.X);
            while (true)
            {
                foreach (var cart in orderedCarts)
                {
                    MoveCart(cart);
                    if (DetectCollision(cart, data.carts))
                    {
                        WriteLine($"Collision at ({cart.Position.X}, {cart.Position.Y})");
                        return;
                    }
                    TurnCart(cart, data.map[cart.Position.Y][cart.Position.X]);
                }
            }
        }

        private void TurnCart(Cart cart, char trackData)
        {
            switch (trackData)
            {
                case '+':
                    TurnCartInCrossRoad(cart);
                    break;
                case '\\':
                    switch (cart.Direction)
                    {
                        case Direction.Up:
                            cart.Direction = Direction.Left;
                            break;
                        case Direction.Down:
                            cart.Direction = Direction.Right;
                            break;
                        case Direction.Left:
                            cart.Direction = Direction.Up;
                            break;
                        case Direction.Right:
                            cart.Direction = Direction.Down;
                            break;
                    }
                    break;
                case '/':
                    switch (cart.Direction)
                    {
                        case Direction.Up:
                            cart.Direction = Direction.Right;
                            break;
                        case Direction.Down:
                            cart.Direction = Direction.Left;
                            break;
                        case Direction.Left:
                            cart.Direction = Direction.Down;
                            break;
                        case Direction.Right:
                            cart.Direction = Direction.Up;
                            break;
                    }
                    break;
                case '-': case '|': break;
                default: throw new InvalidOperationException();
            }
        }

        private void TurnCartInCrossRoad(Cart cart)
        {
            switch (cart.NextTurn)
            {
                case Turn.Left:
                    cart.Direction--;
                    if (cart.Direction < 0) cart.Direction = Direction.Left;
                    break;
                case Turn.Right:
                    cart.Direction++;
                    if (cart.Direction > Direction.Left) cart.Direction = 0;
                    break;
                case Turn.Straight: break;
            }

            cart.NextTurn++;
            if (cart.NextTurn > Turn.Right) cart.NextTurn = (Turn)0;
        }

        private bool DetectCollision(Cart cart, IEnumerable<Cart> allCarts)
        {
            foreach (var otherCart in allCarts)
            {
                if (cart != otherCart && cart.Position.X == otherCart.Position.X && cart.Position.Y == otherCart.Position.Y)
                {
                    return true;
                }
            }

            return false;
        }

        private void MoveCart(Cart cart)
        {
            switch (cart.Direction)
            {
                case Direction.Up:
                    cart.Position = (cart.Position.X, cart.Position.Y - 1);
                    break;

                case Direction.Right:
                    cart.Position = (cart.Position.X + 1, cart.Position.Y);
                    break;

                case Direction.Down:
                    cart.Position = (cart.Position.X, cart.Position.Y + 1);
                    break;

                case Direction.Left:
                    cart.Position = (cart.Position.X - 1, cart.Position.Y);
                    break;
            }
        }

        private (List<List<char>> map, List<Cart> carts) ParseInput(string filepath)
        {
            var carts = new List<Cart>();
            var map = new List<List<char>>();
            int y = 0;
            foreach (var line in File.ReadAllLines(filepath))
            {
                int x = 0;
                if (string.IsNullOrEmpty(line)) continue;

                var row = new List<char>();

                foreach (var c in line)
                {
                    switch (c)
                    {
                        case '>': case '<':
                            row.Add('-');
                            carts.Add(new Cart
                                    {
                                        Position = (x, y),
                                        Direction = c == '<' ? Direction.Left : Direction.Right,
                                        NextTurn = Turn.Left
                                    });
                            break;

                        case '^': case 'v':
                            row.Add('|');
                            carts.Add(new Cart
                                    {
                                        Position = (x, y),
                                        Direction = c == '^' ? Direction.Up : Direction.Down,
                                        NextTurn = Turn.Left
                                    });
                            break;

                        case ' ': case '-': case '|': case '+': case '\\': case '/':
                            row.Add(c);
                            break;

                        default:
                            throw new InvalidOperationException($"Unexpected character '{c}'!");
                    }

                    x++;
                }

                y++;
                map.Add(row);
            }

            return (map, carts);
        }
    }
}
