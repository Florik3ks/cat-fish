using System;
using System.Threading;
using System.Collections.Generic;

namespace cat_fish
{
    class Program
    {
        private static DateTime startTime;
        private static bool running;
        private static int gridHeight = 45;
        private static int gridWidth = 75;
        private static int speed = 1;
        private static int catchedFishCount;
        private static int droppedFishCount;
        private static int catPosition = gridWidth / 2;
        private static int spawnTimer;
        private static int spawnTimerCooldown = 250;
        private static int leftMargin = 1;
        private static int rightMargin = leftMargin + 7;
        private static (int, int) catPickupRange = (-1, 7);
        private static string fishArt = " __\n/o \\/\n\\__/\\";
        private static string catArt = "\\_____/\n /\\_/\\\n( o.o )\n > ^ <";
        // (x position, y position, speed)
        private static List<(int, float, float)> fishes = new List<(int, float, float)>();
        private static Cell[,] grid = new Cell[gridHeight, gridWidth];

        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.SetWindowSize(gridWidth, gridHeight + 5);
            running = true;
            catchedFishCount = 0;
            droppedFishCount = 0;
            spawnTimer = 0;
            startTime = DateTime.Now;
            createGrid();
            while (running)
            {
                Game();
            }
        }

        static void Game()
        {
            getInput();
            updateScreen();
            Move();
            spawnTimer--;
            if (spawnTimer <= 0)
            {
                spawnTimer = spawnTimerCooldown;
                spawnFish();
            }
            Thread.Sleep(speed * 1);
        }

        static void spawnFish()
        {
            int x = new Random().Next(leftMargin, gridWidth - rightMargin);
            float y = 1f;
            float speed = new Random().Next(1, 7);
            speed /= 100;
            fishes.Add((x, y, speed));
        }

        static void updateScreen()
        {
            Console.SetCursorPosition(0, 0);
            printGrid();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("catched fish: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(catchedFishCount);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("dropped fish: ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(droppedFishCount);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("elapsed time since start: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(DateTime.Now - startTime);
            
            Console.ForegroundColor = ConsoleColor.White;
        }

        static void getInput()
        {
            ConsoleKeyInfo input;
            if (!Console.KeyAvailable)
            {
                return;
            }
            input = Console.ReadKey();
            if (input.Key == ConsoleKey.LeftArrow)
            {
                if (catPosition - leftMargin > 0)
                {
                    catPosition--;
                }
            }
            else if (input.Key == ConsoleKey.RightArrow)
            {
                if (catPosition + rightMargin < gridWidth)
                {
                    catPosition++;
                }
            }
            else if (input.Key == ConsoleKey.C)
            {
                running = false;
            }
        }

        static void Move()
        {
            updateFishPositions();
            updateCatPosition();
        }

        static void updateCatPosition()
        {
            string[] art = catArt.Split("\n");
            for (int height = gridHeight - art.Length; height < gridHeight; height++)
            {
                int charPos = catPosition - 1;
                foreach (char c in art[art.Length - (gridHeight - height)])
                {
                    charPos++;
                    grid[height - 1, charPos].Set(c.ToString());
                }
            }
        }

        static void updateFishPositions()
        {
            string[] art = fishArt.Split("\n");
            for (int i = 0; i < fishes.Count; i++)
            {
                // despawn at bottom
                if (fishes[i].Item2 > gridHeight - 4)
                {
                    fishes.RemoveAt(i);
                    droppedFishCount++;
                    continue;
                }
                // picked up
                int fishX = fishes[i].Item1 + 2;
                if (fishes[i].Item2 > gridHeight - 10 && fishX >= catPosition + catPickupRange.Item1 && fishX <= catPosition + catPickupRange.Item2)
                {
                    fishes.RemoveAt(i);
                    catchedFishCount++;
                    continue;
                }

                fishes[i] = (fishes[i].Item1, fishes[i].Item2 + fishes[i].Item3, fishes[i].Item3);
                int fishHeight = (int)fishes[i].Item2;
                for (int line = 0; line < art.Length; line++)
                {
                    int charPos = fishes[i].Item1 - 1;
                    foreach (char c in art[line])
                    {
                        charPos++;
                        grid[fishHeight + line, charPos].Set(c.ToString());
                    }
                }
            }
        }
        static void printGrid()
        {
            string toPrint = "";
            for (int col = 0; col < gridHeight; col++)
            {
                for (int row = 0; row < gridWidth; row++)
                {
                    toPrint += grid[col, row].val;
                    grid[col, row].Clear();
                }
                toPrint += "\n";
            }
            Console.WriteLine(toPrint);
        }
        static void createGrid()
        {
            for (int col = 0; col < gridHeight; col++)
            {
                for (int row = 0; row < gridWidth; row++)
                {
                    Cell cell = new Cell();
                    if (row == 0 || row > gridWidth - 2 || col == 0 || col > gridHeight - 2)
                    {
                        cell.Set("*");
                    }
                    else
                    {
                        cell.Clear();
                    }
                    grid[col, row] = cell;
                }
            }
        }
        public class Cell
        {
            public string val;
            public bool occupied;

            public void Clear()
            {
                if (val == "*")
                {
                    return;
                }
                if (occupied)
                {
                    occupied = false;
                    return;
                }
                val = " ";
            }

            public void Set(string _val)
            {
                if (val == "*")
                {
                    return;
                }
                occupied = true;
                val = _val;
            }
        }
    }
}
