using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks.Sources;

/**
 * Save humans, destroy zombies!
 **/
////////////////////////////////
namespace Kmeans
{
    public class Point
    {
        public int X;
        public int Y;
        public int cluster;
    }
    public class KMeans
    {
        public List<Point> points;
        public List<Point> centroids;
        public int pointNumber;
        public int clusterNumber;

        private Random _rand;

        public KMeans(IEnumerable<Point> points, int clusterNumber = 2)
        {
            this.points = points.ToList();
            centroids = new List<Point>();
            this.pointNumber = this.points.Count();
            this.clusterNumber = clusterNumber;
            _rand = new Random(7524);
        }

        int MyCMP(Point a, Point b)
        {
            return a.cluster - b.cluster;
        }

        public void KMeans_Inital_RandomPartition()
        {
            for (int i = 0; i < pointNumber; i++)
            {
                points[i].cluster = i % clusterNumber;
            }
            KMeans_Update();
        }

        public void KMeans_Inital_Forgy()
        {
            int i, j, k;
            Point MyTemp;
            for (i = 0; i < pointNumber; i++)
            {
                k = _rand.Next() % pointNumber;
                MyTemp = points[i];
                points[i] = points[k];
                points[k] = MyTemp;
            }

            for (i = 0; i < clusterNumber; i++)
            {
                centroids[i] = points[i];
            }
        }

        int KMeans_GeneratePoints(int N)
        {
            int I, J, K;
            for (I = 0; I < N & I < 1024; I++)
            {
                points[I].X = _rand.Next() % 1000;
                points[I].Y = _rand.Next() % 1000;
                points[I].cluster = int.MaxValue;
            }
            return I;
        }

        //#define EUCLID(X,Y) ((X)*(X))+((Y)*(Y))

        int EUCLID(int x, int y)
        {
            return ((x) * (x)) + ((y) * (y));
        }

        int KMeans_Assign()
        {
            int i, j;
            int Min, Sum;

            for (i = 0, Sum = 0; i < pointNumber; i++)
            {
                Min = int.MaxValue;
                for (j = 0; j < clusterNumber; j++)
                {
                    if (Min > EUCLID(points[i].X - centroids[j].X, points[i].Y - centroids[j].Y))
                    {
                        Min = EUCLID(points[i].X - centroids[j].X, points[i].Y - centroids[j].Y);
                        points[i].cluster = j;
                    }
                }
                Sum += Min;
            }

            return Sum;
        }

        void KMeans_Update()
        {
            centroids = points.GroupBy(p => p.cluster).Select(g => new Point() { X = (int)g.Average(a => a.X), Y = (int)g.Average(b => b.Y) }).ToList();
        }

        public IEnumerable<Point> getCentroids()
        {
            int Sum = int.MaxValue;
            int OldSum = int.MaxValue;
            KMeans_Inital_Forgy();
            while (true)
            {
                Sum = KMeans_Assign();
                if (Sum == OldSum)
                {
                    break;
                }
                KMeans_Update();
                OldSum = Sum;
            }
            return centroids;
        }
    }
}
////////////////////////////////
public enum GameObjectType
{
    ASH,
    HUMAN,
    ZOMBIE
}

public class GameObject
{
    public int id;
    public GameObjectType type;
    public int X;
    public int Y;
    public bool isDead = false;

    public virtual int Attack(Board board) { return 0; }


    public void Move(Board board, int targetX, int targetY, int power)
    {
        double distanceTarget = Math.Sqrt((targetX - this.X) * (targetX - this.X) + (targetY - this.Y) * (targetY - this.Y));
        double ratio = 1.0;
        if (distanceTarget <= power)
        {
            this.X = targetX;
            this.Y = targetY;
        }
        else
        {
            double ang = Math.Atan(((double)targetY - this.Y) / ((double)targetX - this.X));
            ratio = ((double)power / distanceTarget);
            this.X = (int)(this.X + ratio * (targetX - this.X));
            this.Y = (int)(this.Y + ratio * (targetY - this.Y));
        }

    }

}

public class Human : GameObject
{
    public Human(int id, int x, int y)
    {
        this.id = 100 + id;
        type = GameObjectType.HUMAN;
        this.X = x;
        this.Y = y;
    }

}

public class Ash : Human
{
    public Ash(int x, int y) : base(1, x, y)
    {
        type = GameObjectType.ASH;
    }

    public override int Attack(Board board)
    {
        List<GameObject> zombies = board._gameObjects.Where(obj => obj.type == GameObjectType.ZOMBIE).ToList();
        int killed = 0;
        var zs = zombies.Where(h => Math.Sqrt(((double)h.X - this.X) * ((double)h.X - this.X) + ((double)h.Y - this.Y) * ((double)h.Y - this.Y)) <= 2000.0);
        foreach (Zombie z in zs)
        {
            Console.Error.WriteLine($"Z:{z.id} {z.X} {z.Y} ; Ash:{this.X} {this.Y}");
            z.isDead = true;
            killed++;
        }
        board._gameObjects = board._gameObjects.Where(g => g.isDead == false).ToList();
        return killed;
    }
}

public class Zombie : GameObject
{
    public int nextX;
    public int nextY;
    public GameObject GetClosestHuman(Board board)
    {
        List<GameObject> humans = board._gameObjects.Where(obj => obj.type != GameObjectType.ZOMBIE).ToList();
        return humans.OrderBy(h => Math.Sqrt((h.X - this.X) * (h.X - this.X) + (h.Y - this.Y) * (h.Y - this.Y))).FirstOrDefault();
    }
    public Zombie(int id, int x, int y, int nx, int ny)
    {
        this.id = 200 + id;
        type = GameObjectType.ZOMBIE;
        this.X = x;
        this.Y = y;
        this.nextX = nx;
        this.nextY = ny;
    }

    public override int Attack(Board board)
    {
        List<GameObject> deadmen = board._gameObjects.Where(obj => obj.type == GameObjectType.HUMAN &&
                    Math.Sqrt((obj.X - this.X) * (obj.X - this.X) + (obj.Y - this.Y) * (obj.Y - this.Y)) < 400.0
        ).ToList();
        int killed = 0;
        foreach (Human h in deadmen)
        {
            h.isDead = true;
            killed++;
        }
        board._gameObjects = board._gameObjects.Where(g => g.isDead == false).ToList();
        return killed;
    }

}

public class Board
{
    public int _height = 9000;
    public int _width = 16000;
    public int _humanCount;
    public int _zombieCount;
    public List<GameObject> _gameObjects;
    public int score = 0;
    public Board(IEnumerable<GameObject> gameObjects)
    {

        _gameObjects = gameObjects.ToList();
        _humanCount = _gameObjects.Where(obj => obj.type == GameObjectType.HUMAN).Count();
        _zombieCount = _gameObjects.Where(obj => obj.type == GameObjectType.ZOMBIE).Count();

    }

    public Board()
    {
        _gameObjects = new List<GameObject>();
    }

    public void AddGameObject(GameObject gameObject)
    {
        _gameObjects.Add(gameObject);
        if (gameObject.type == GameObjectType.ZOMBIE)
        {
            _zombieCount++;
        }
        else if (gameObject.type == GameObjectType.HUMAN)
        {
            _humanCount++;
        }
    }
}

public class GameManager
{
    public int gameScore = 0;

    public int Fibonnacci(int n)
    {
        if (n == 1)
        {
            return 1;
        }
        if (n == 2)
        {
            return 2;
        }
        int a = 1;
        int b = 2;
        int tmp = 3;
        for (int i = 3; i <= n; i++)
        {
            tmp = a + b;
            a = b;
            b = tmp;
        }
        return tmp;
    }

    public int ComputeKillScore(Board board, int kill)
    {
        if (kill == 1)
        {
            return 10 * (board._humanCount * board._humanCount);
        }
        else if (kill >= 2)
        {
            int score = 0;
            for (int i = 1; i <= kill; i++)
            {
                score += Fibonnacci(i) * 10 * (board._humanCount * board._humanCount);
            }
            return score;
        }
        return 0;
    }


    public Board Simulate(Board board, int PlayerTargrtX, int PlayerTargetY)
    {
        //Zombies move towards their targets.
        List<Zombie> zombies = board._gameObjects.Where(g => g.type == GameObjectType.ZOMBIE).Cast<Zombie>().ToList<Zombie>();
        foreach (Zombie z in zombies)
        {
            Human targetHuman = z.GetClosestHuman(board) as Human;
            z.Move(board, targetHuman.X, targetHuman.Y, 400);
        }
        //Ash moves towards his target.
        Ash ash = board._gameObjects.Where(g => g.type == GameObjectType.ASH).Cast<Ash>().FirstOrDefault();
        ash.Move(board, PlayerTargrtX, PlayerTargetY, 1000);
        Console.Error.WriteLine($"Ash:{ash.X} {ash.Y}");
        //Any zombie within a 2000 unit range around Ash is destroyed.
        int killedZombies = ash.Attack(board);
        int addedScore = ComputeKillScore(board, killedZombies);
        //Zombies eat any human they share coordinates with.
        foreach (Zombie z in zombies)
        {
            z.Attack(board);
        }
        board.score = addedScore;
        return board;
    }
}

public class Game
{
    static void Main(string[] args)
    {
        string[] inputs;
        GameManager gm = new GameManager();
        // game loop
        while (true)
        {
            Board board = new Board();
            inputs = Console.ReadLine().Split(' ');
            int x = int.Parse(inputs[0]);
            int y = int.Parse(inputs[1]);
            int humanCount = int.Parse(Console.ReadLine());
            Ash player = new Ash(x, y);
            board.AddGameObject(player);
            for (int i = 0; i < humanCount; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int humanId = int.Parse(inputs[0]);
                int humanX = int.Parse(inputs[1]);
                int humanY = int.Parse(inputs[2]);
                board.AddGameObject(new Human(humanId, humanX, humanY));
            }
            int zombieCount = int.Parse(Console.ReadLine());
            for (int i = 0; i < zombieCount; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int zombieId = int.Parse(inputs[0]);
                int zombieX = int.Parse(inputs[1]);
                int zombieY = int.Parse(inputs[2]);
                int zombieXNext = int.Parse(inputs[3]);
                int zombieYNext = int.Parse(inputs[4]);
                board.AddGameObject(new Zombie(zombieId, zombieX, zombieY, zombieXNext, zombieYNext));
            }

            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");
            List<Human> humans = board._gameObjects.Where(g => g.type == GameObjectType.HUMAN).Cast<Human>().ToList<Human>();
            Board tmpBoard = gm.Simulate(board, humans[0].X, humans[0].Y);
            gm.gameScore += tmpBoard.score;

            Console.WriteLine($"{humans[0].X} {humans[0].Y} {gm.gameScore}"); // Your destination coordinates

        }
    }
}