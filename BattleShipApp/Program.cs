using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace BattleShipApp
{
    public abstract class Ship
    {
        public int length;
        public ConsoleColor color = ConsoleColor.DarkGray;
        public string content;
    }

    public class AircraftCarrier : Ship
    {
        public AircraftCarrier()
        {
            length = 5;
            content = "A";
        }
    }

    public class BattleShip : Ship
    {
        public BattleShip()
        {
            length = 4;
            content = "B";
        }
    }

    public class Cruiser : Ship
    {
        public Cruiser()
        {
            length = 3;
            content = "C";
        }
    }

    public class Destroyer : Ship
    {
        public Destroyer()
        {
            length = 2;
            content = "D";
        }
    }

    public class Submarine : Ship
    {
        public Submarine()
        {
            length = 1;
            content = "S";
        }
    }

    struct Tile
    {
        public int x;
        public int y;
        public string content;
        public ConsoleColor color;

        public Tile(int x, int y, string content, ConsoleColor color)
        {
            this.x = x; this.y = y; this.content = content; this.color = color;
        }

        public bool IsFree()
        {
            return (content == ".") ? true : false;
        }
    }

    class Player
    {
        Random rand;
        List<Tile> playermap;
        List<Tile> enemymap;

        // Column and row lists to be shown for choosing the X and Y coordinates
        List<string> columnTitleList = new List<string>{ "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" };
        List<int> rowTitleList = new List<int> {0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        // Public variables for handling the console output and max scores
        public int firedAgain = 0;
        public string infoText = "";
        public bool isMaxScores = false;

        int score;
        string name;
        Player enemy;

        public Player(string name)
        {
            score = 0;
            this.name = name;
        }

        public void InitPlayer()
        {
            CreateMapBoard();
            SetUpUnits();
        }

        void CreateMapBoard()
        {
            playermap = new List<Tile>();
            enemymap = new List<Tile>();

            for (int iterA = 0; iterA < 10; iterA++)
            {
                for (int iterB = 0; iterB < 10; iterB++)
                {
                    playermap.Add(new Tile(iterA, iterB, ".", ConsoleColor.Blue));
                    enemymap.Add(new Tile(iterA, iterB, ".", ConsoleColor.Blue));
                }
            }
        }

        void SetUpUnits()
        {
            rand = new Random(DateTime.Now.Millisecond);

            // Create ships
            AircraftCarrier a = new AircraftCarrier();
            BattleShip b = new BattleShip();
            Cruiser c = new Cruiser();
            Destroyer d = new Destroyer();
            Submarine s = new Submarine();

            List<Ship> shipList = new List<Ship>() { a, b, c, d, s };

            bool placementOK = false;

            foreach (Ship ship in shipList)
            {
                int x, y, direction;
                do
                {
                    x = rand.Next(10);
                    y = rand.Next(10);
                    direction = (rand.Next(10)) % 2; // 0 horizontal, 1 = vertical

                    if (direction == 0)
                    {
                        if (x + ship.length <= 10)
                        {
                            for (int iterX = x; iterX < ship.length; iterX++)
                            {
                                placementOK = true;
                                if (!playermap.ElementAt<Tile>(y * 10 + iterX).IsFree())
                                {
                                    placementOK = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (y + ship.length <= 10)
                        {
                            for (int iterY = y; iterY < ship.length; iterY++)
                            {
                                placementOK = true;

                                if (!playermap.ElementAt<Tile>(iterY * 10 + x).IsFree())
                                {
                                    placementOK = false;
                                }
                            }
                        }
                    }
                } while (!placementOK);

                // Set ship on board
                if (direction == 0)
                {
                    for (int iterXX = 0; iterXX < ship.length; iterXX++)
                    {
                        int position = (y * 10) + (x + iterXX);
                        ChangeMapTile(playermap, position, ship.color, ship.content);
                    }
                }
                else
                {
                    for (int iterYY = 0; iterYY < ship.length; iterYY++)
                    {
                        int position = ((y + iterYY) * 10) + x;
                        ChangeMapTile(playermap, position, ship.color, ship.content);
                    }
                }

                placementOK = false;
            }
        }

        public void SetEnemy(Player enemy)
        {
            this.enemy = enemy;

            // For testing only...
            /*
            ConsoleColor dummycolor;
            string dummycontent;
            for (int iter = 0; iter < 100; iter++)
            {
                dummycolor = enemy.playermap.ElementAt<Tile>(iter).color;
                dummycontent = enemy.playermap.ElementAt<Tile>(iter).content;
                ChangeMapTile(enemymap, iter, dummycolor, dummycontent);
            }
            */
        }

        void ChangeMapTile(List<Tile> map, int position, ConsoleColor color, string content)
        {
            Tile dummy = map.ElementAt<Tile>(position);
            dummy.color = color;
            dummy.content = content;
            map.RemoveAt(position);
            map.Insert(position, dummy);
        }

        public void DrawMap(string message = "")
        {
            Console.Clear();

            int baseX = 2, baseY = 1;
            Console.Write("Player is " + name + " with score " + score + ".");

            baseX = 2; baseY = 3;
            int x, y;
                        
            for (int iter = 0; iter < 100; iter++)
            {
                // Show the column (baseX) and row (baseY) texts
                if ( iter < 10)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.SetCursorPosition(iter + baseX, baseY - 1);
                    Console.WriteLine(columnTitleList[iter]);
                    Console.SetCursorPosition(baseX - 1, iter + baseY);
                    Console.WriteLine(rowTitleList[iter]);
                }

                x = playermap.ElementAt<Tile>(iter).x;
                y = playermap.ElementAt<Tile>(iter).y;

                Console.SetCursorPosition(baseX + x, baseY + y);
                Console.BackgroundColor = playermap.ElementAt<Tile>(iter).color;
                Console.Write(playermap.ElementAt<Tile>(iter).content);                
            }

            baseX = 15; baseY = 3;
                        
            for (int iter = 0; iter < 100; iter++)
            {
                // Show the column (baseX) and row (baseY) texts
                if (iter < 10)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.SetCursorPosition(iter + baseX, baseY - 1);
                    Console.WriteLine(columnTitleList[iter]);
                    Console.SetCursorPosition(baseX - 1, iter + baseY);
                    Console.WriteLine(rowTitleList[iter]);
                }
                
                x = enemymap.ElementAt<Tile>(iter).x;
                y = enemymap.ElementAt<Tile>(iter).y;

                Console.SetCursorPosition(baseX + x, baseY + y);
                Console.BackgroundColor = enemymap.ElementAt<Tile>(iter).color;
                Console.Write(enemymap.ElementAt<Tile>(iter).content);
            }

            baseX = 2; baseY = 17;
            Console.SetCursorPosition(baseX, baseY);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine(message);
        }

        public void Fire()
        {
            Console.WriteLine("Fire Now!");

            bool validResponse = false;
            int responseX, responseY;

            do
            {
                // User's input to x
                Console.WriteLine("X as a column (A=0, B=1, C=2 etc...)");
                string guessInput = Console.ReadLine();

                // Convert string to number
                validResponse = int.TryParse(guessInput, out responseX);

                if (validResponse)
                {
                    if (responseX < 0 || responseX > 9) { validResponse = false; }
                }

            } while (!validResponse);

            do
            {
                // User's input to Y
                Console.WriteLine("Y as a row (0, 1, 2 etc...)");
                string guessInput = Console.ReadLine();

                // Convert string to number
                validResponse = int.TryParse(guessInput, out responseY);

                if (validResponse)
                {
                    if (responseY < 0 || responseY > 9) { validResponse = false; }
                }

            } while (!validResponse);

            int position = (responseX * 10) + responseY;
            // Enemy's baseX and baseY
            int baseX = 15; 
            int baseY = 3;
            int cursorX = Console.CursorLeft;
            int cursorY = Console.CursorTop;

            // Evaluate if hit or miss
            // If miss
            if (enemy.playermap.ElementAt<Tile>(position).content == ".")
            {
                ChangeMapTile(enemymap, position, ConsoleColor.Red, "!");
                // Show the miss also on the map
                Console.SetCursorPosition((baseX + responseX), (baseY + responseY));
                Console.BackgroundColor = ConsoleColor.Red;
                Console.Write(enemymap.ElementAt<Tile>(position).content);
                // Inform player
                infoText = "Sorry, you missed!\n";
            }
            else if (
                enemymap.ElementAt<Tile>(position).content != "A" &&
                enemymap.ElementAt<Tile>(position).content != "B" &&
                enemymap.ElementAt<Tile>(position).content != "C" &&
                enemymap.ElementAt<Tile>(position).content != "D" &&
                enemymap.ElementAt<Tile>(position).content != "S"
                )
            {
                score++;

                string content = enemy.playermap.ElementAt<Tile>(position).content;

                // Show the hit on the enemymap
                ChangeMapTile(enemymap, position, ConsoleColor.Green, content);
                Console.SetCursorPosition((baseX + responseX), (baseY + responseY));
                Console.BackgroundColor = ConsoleColor.Green;
                Console.Write(enemymap.ElementAt<Tile>(position).content);

                // Show score and the info text
                int startX = 0, startY = 0;
                cursorY++;
                Console.SetCursorPosition(startX, startY);
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write("Player is " + name + " with score " + score + ".");

                // Check the scores
                if (score < 15)
                {
                    Console.SetCursorPosition((cursorX), (cursorY));
                    Console.WriteLine("You hit enemy! Fire again ...");

                    Fire();
                    // Fired more than once 
                    firedAgain++;
                }
                else
                {
                    isMaxScores = true;
                    Console.SetCursorPosition((cursorX), (cursorY));
                    Console.WriteLine("This was the last one, all ships are hit. Congratulations!");
                    Console.WriteLine("<Return> to finished the came...");
                    Console.ReadLine();
                }
            }
            // Already used the same coordinates when hit succesfully, inform the player
            else
            {
                infoText = "You have already hit with the same coordinates!\n";
            }

            // Show only once when missed for the first round or fired again and then missed
            if (firedAgain == 0 && !isMaxScores)
            {
                Console.SetCursorPosition((cursorX), (cursorY + 1));
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine(infoText + "<Return> to Change Player...");
                Console.ReadLine();
            }
        }

        public bool DidWin()
        {
            bool winner = (score >= 15) ? true : false;

            // Show the winner
            if (winner)
            {
                Console.WriteLine("Winner is " + name);
            }

            return winner;
        }
    }

    class Game
    {
        Player player1;
        Player player2;

        public Game()
        {
            // Initiate players
            player1 = new Player("Player 1");
            player2 = new Player("Player 2");

            // Initiate players and setup units
            player1.InitPlayer();
            player2.InitPlayer();

            // Player 1's enemymap is player2 map and vice versa
            player1.SetEnemy(player2);
            player2.SetEnemy(player1);
        }

        public void PlayGame()
        {
            // Run game loop
            bool didWin = false;

            do
            {
                // Player1
                player1.DrawMap();
                player1.Fire();
                player1.firedAgain = 0;
                player1.infoText = "";
                player1.DrawMap();
                didWin = player1.DidWin();

                if (!didWin)
                {
                    // Player2
                    player2.DrawMap();
                    player2.Fire();
                    player2.firedAgain = 0;
                    player1.infoText = "";
                    player2.DrawMap();
                    didWin = player2.DidWin();
                }               

            } while (!didWin);

            Console.WriteLine("Game over, <Return> to exit.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Set the game and play
            Game game = new Game();
            game.PlayGame();

            Console.ReadLine();
        }
    }
}
