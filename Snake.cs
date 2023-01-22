using System;
using System.Threading;

namespace Snake
{
    class Program
    {
        //unitype for coordinate and size
        struct PointSize
        {
            public int x;
            public int y;

            public PointSize(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public static bool operator==(PointSize p1, PointSize p2)
            {
                return (p1.x == p2.x) && (p1.y == p2.y);
            }

            public static bool operator!=(PointSize p1, PointSize p2)
            {
                return (p1.x != p2.x) || (p1.y != p2.y);;
            }
        }

        //snake direction and for other some
        enum Direction
        {
            Left,
            Up,
            Right,
            Down
        }

        public static void Main()
        {
            //variables and init:
            Random rand = new Random();
            PointSize fieldSize = new PointSize(15, 15);
            PointSize[] snakeBody = new PointSize[1];
            snakeBody[0].x = fieldSize.x / 2;
            snakeBody[0].y = fieldSize.y / 2;
            PointSize pickupPos = new PointSize(0, 0);
            int score = 0;
            Direction snakeDir = Direction.Left;
            int gameLoopDelay = 200;
            bool isGameOver = false;
            
            //style variables:
            ConsoleColor statBarBg = ConsoleColor.DarkBlue;
            ConsoleColor statBarFg = ConsoleColor.White;
            ConsoleColor fieldColor = ConsoleColor.DarkGreen;
            ConsoleColor pickupColor = ConsoleColor.Red;
            ConsoleColor snakeColor = ConsoleColor.Yellow;

            //game loop:
            while (!isGameOver)
            {
                //read key and change snake dir:
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey();
                    if (keyInfo.Key == ConsoleKey.LeftArrow)
                        snakeDir = Direction.Left;
                    else if (keyInfo.Key == ConsoleKey.UpArrow)
                        snakeDir = Direction.Up;
                    else if (keyInfo.Key == ConsoleKey.RightArrow)
                        snakeDir = Direction.Right;
                    else if (keyInfo.Key == ConsoleKey.DownArrow)
                        snakeDir = Direction.Down;
                }

                //move snake:
                snakeMove:
                PointSize newSnakePos = snakeBody[0];
                if (snakeDir == Direction.Left)
                    newSnakePos = new PointSize(snakeBody[0].x - 1, snakeBody[0].y);
                else if (snakeDir == Direction.Up)
                    newSnakePos = new PointSize(snakeBody[0].x, snakeBody[0].y - 1);
                else if (snakeDir == Direction.Right)
                    newSnakePos = new PointSize(snakeBody[0].x + 1, snakeBody[0].y);
                else if (snakeDir == Direction.Down)
                    newSnakePos = new PointSize(snakeBody[0].x, snakeBody[0].y + 1);
                
                //process snake new pos:
                //1. check for self through:
                if (snakeBody.Length > 1)
                {
                    PointSize nextBodyPart = snakeBody[1];
                    if (nextBodyPart == newSnakePos)
                    {
                        //change snake direction to opposite dirercion...
                        if (snakeDir == Direction.Left)
                            snakeDir = Direction.Right;
                        else if (snakeDir == Direction.Right)
                            snakeDir = Direction.Left;
                        else if (snakeDir == Direction.Up)
                            snakeDir = Direction.Down;
                        else if (snakeDir == Direction.Down)
                            snakeDir = Direction.Up;

                        //... and go to snake move processing again:
                        goto snakeMove;
                    }
                }
                //2. check for field wrap:
                {
                    if (newSnakePos.x < 0)
                        newSnakePos.x = fieldSize.x - 1;
                    else if (newSnakePos.x >= fieldSize.x)
                        newSnakePos.x = 0;

                    if (newSnakePos.y < 0)
                        newSnakePos.y = fieldSize.y - 1;
                    else if (newSnakePos.y >= fieldSize.y)
                        newSnakePos.y = 0;
                }
                //3. check for pickup:
                if (pickupPos == newSnakePos)
                {
                    score++;
                    PointSize[] newSnakeBody = new PointSize[snakeBody.Length + 1];
                    for (int i = 0; i < snakeBody.Length; ++i)
                        newSnakeBody[i] = snakeBody[i];
                    snakeBody = newSnakeBody;
                    newSnakeBody[^1].x = int.MinValue;
                    newSnakeBody[^1].y = int.MinValue;

                    while (pickupPos == newSnakePos || snakeBody.Any(coord => coord == pickupPos))
                    {
                        pickupPos.x = rand.Next(0, fieldSize.x);
                        pickupPos.y = rand.Next(0, fieldSize.y);
                    }
                }
                //4. check for game over:
                isGameOver = snakeBody.Any(coord => coord == newSnakePos);

                //after any checks, fill snakeBody:
                {
                    PointSize[] newSnakeBody = new PointSize[snakeBody.Length];
                    newSnakeBody[0] = newSnakePos;
                    for (int i = 0; i < snakeBody.Length - 1; ++i)
                        newSnakeBody[i+1] = snakeBody[i];
                    snakeBody = newSnakeBody;
                }

                //draw:
                Console.Clear();

                Console.BackgroundColor = statBarBg;
                Console.ForegroundColor = statBarFg;
                Console.WriteLine("Score: {0}", score);

                for (int row = 0; row < fieldSize.y; ++row)
                {
                    for (int col = 0; col < fieldSize.x; ++col)
                    {
                        if (snakeBody.Any(coord => (coord.x == col) && (coord.y == row)))
                        {
                            Console.BackgroundColor = snakeColor;
                        }
                        else if ((col == pickupPos.x) && (row == pickupPos.y))
                        {
                            Console.BackgroundColor = pickupColor;
                        }
                        else
                        {
                            Console.BackgroundColor = fieldColor;
                        }

                        Console.Write("  ");
                    }

                    Console.WriteLine();    
                }

                Console.ResetColor();
                Thread.Sleep(gameLoopDelay);
            }

            Console.WriteLine("Game over. Your score is: {0} point(s)", score);
        }
    }
}