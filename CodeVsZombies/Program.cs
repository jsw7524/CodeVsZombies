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

    public KMeans(IEnumerable<Point> points)
    {
        this.points = points.ToList();
        centroids = new List<Point>();
        this.pointNumber = this.points.Count();
        //this.clusterNumber = clusterNumber;
        _rand = new Random(7524);
    }


    public void KMeans_Inital_RandomPartition()
    {
        for (int i = 0; i < pointNumber; i++)
        {
            points[i].cluster = i % clusterNumber;
        }
        Update();
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
            centroids.Add(points[i]);
        }
    }

    int EUCLID(int x, int y)
    {
        return ((x) * (x)) + ((y) * (y));
    }

    int Assign()
    {
        int sum = 0;
        foreach (Point p in points)
        {
            int min = int.MaxValue;
            int j = 0;
            foreach (Point cp in centroids)
            {
                int tmp = EUCLID(p.X - cp.X, p.Y - cp.Y);
                if (min > tmp)
                {
                    min = tmp;
                    p.cluster = j;
                }
                j += 1;
            }
            sum += min;
        }
        return sum;
    }

    void Update()
    {
        centroids = points.GroupBy(p => p.cluster).Select(g => new Point() { X = (int)g.Average(a => a.X), Y = (int)g.Average(b => b.Y) }).ToList();
    }

    public IEnumerable<Point> getCentroids(int clusterNumber)
    {
        this.clusterNumber = Math.Min(clusterNumber, pointNumber);
        this.centroids = new List<Point>(this.clusterNumber);
        int Sum = int.MaxValue;
        int OldSum = int.MaxValue;
        KMeans_Inital_Forgy();
        while (true)
        {
            Sum = Assign();
            if (Sum == OldSum)
            {
                break;
            }
            Update();
            OldSum = Sum;
        }
        return centroids;
    }
}

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

    public GameObject GetClosestHuman(Board board)
    {
        List<GameObject> humans = board._gameObjects.Where(obj => obj.type != GameObjectType.ZOMBIE).ToList();
        return humans.OrderBy(h => Math.Sqrt((h.X - this.X) * (h.X - this.X) + (h.Y - this.Y) * (h.Y - this.Y))).FirstOrDefault();
    }

    public GameObject GetClosestHumanNotAsh(Board board)
    {
        List<GameObject> humans = board._gameObjects.Where(obj => obj.type == GameObjectType.HUMAN).ToList();
        return humans.OrderBy(h => Math.Sqrt((h.X - this.X) * (h.X - this.X) + (h.Y - this.Y) * (h.Y - this.Y))).FirstOrDefault();
    }
    public GameObject GetClosestZombie(Board board)
    {
        List<GameObject> humans = board._gameObjects.Where(obj => obj.type == GameObjectType.ZOMBIE).ToList();
        return humans.OrderBy(h => Math.Sqrt((h.X - this.X) * (h.X - this.X) + (h.Y - this.Y) * (h.Y - this.Y))).FirstOrDefault();
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
            //Console.Error.WriteLine($"Z:{z.id} {z.X} {z.Y} ; Ash:{this.X} {this.Y}");
            z.isDead = true;
            killed++;
            board._zombieCount -= 1;
        }
        board._gameObjects = board._gameObjects.Where(g => g.isDead == false).ToList();
        return killed;
    }
}

public class Zombie : GameObject
{
    public int nextX;
    public int nextY;

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
            board._humanCount -= 1;
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

    public Board DeepCopy()
    {
        List<GameObject> tmp = new List<GameObject>();

        foreach (GameObject g in _gameObjects)
        {
            if (g is Ash)
            {
                tmp.Add(new Ash(g.X, g.Y));
            }
            else if (g is Human)
            {
                tmp.Add(new Human(g.id, g.X, g.Y));
            }
            else if (g is Zombie)
            {
                Zombie k = g as Zombie;
                tmp.Add(new Zombie(k.id, k.X, k.Y, k.nextX, k.nextY));
            }
        }
        return new Board(tmp) { _zombieCount = this._zombieCount, _humanCount = this._humanCount, score = this.score };
    }

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
    public Ash currentPlayer;
    public (int, Point) GameTree(Board board, int depth)
    {
        if (board._humanCount == 0)
        {
            return (-999999, null);
        }
        if (depth == 5)
        {
            //int bonus = board._humanCount  - board._zombieCount ;
            int bonus = 0;
            foreach (GameObject h in board._gameObjects.Where(g => g.type == GameObjectType.HUMAN))
            {
                var cz = h.GetClosestZombie(board);
                if (cz != null)
                {
                    bonus += (int)Math.Sqrt((h.X - cz.X) * (h.X - cz.X) + (h.Y - cz.Y) * (h.Y - cz.Y));
                }
                else
                {
                    bonus += 999999;
                }
            }
            return (bonus, null);
        }
        else
        {
            List<Point> dest = AshNextMove(board, 4).ToList();
            List<(Point, Board)> nextMoves = new List<(Point, Board)>();
            foreach (Point p in dest)
            {
                Board copy = board.DeepCopy();
                Board tmpBoard = Simulate(copy, p.X, p.Y);
                if (tmpBoard._humanCount == 0)
                {
                    return (-999999, null);
                }
                int ns;
                Point pt;
                (ns, pt) = GameTree(tmpBoard, depth + 1);
                copy.score += ns;
                nextMoves.Add((p, copy));
            }
            var next = nextMoves.OrderByDescending(b => b.Item2.score).FirstOrDefault();
            return (next.Item2.score, new Point() { X = next.Item1.X, Y = next.Item1.Y });
        }

    }

    public Point NextMove(Board board)
    {
        if (board._humanCount == 1)
        {
            Human theOne = board._gameObjects.Where(g => g.type == GameObjectType.HUMAN).FirstOrDefault() as Human;
            return new Point() { X = theOne.X, Y = theOne.Y };
        }
        int ns;
        Point pt;
        (ns, pt) = GameTree(board, 0);

        if (pt == null)
        {
            Human theOne = currentPlayer.GetClosestHumanNotAsh(board) as Human;
            return new Point() { X = theOne.X, Y = theOne.Y };
        }

        return pt;
    }


    public int Fibonnacci(int n)
    {
        if (n <= 2)
        {
            return n;
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

    public IEnumerable<Point> AshNextMove(Board board, int availableOptions)
    {
        //List<Point> points = board._gameObjects.Where(g => g.type != GameObjectType.ASH).Select(a => new Point() { X = a.X, Y = a.Y }).ToList();
        List<Point> points = board._gameObjects.Select(a => new Point() { X = a.X, Y = a.Y }).ToList();
        KMeans km = new KMeans(points);
        List<Point> centroids = km.getCentroids(clusterNumber: availableOptions).ToList();

        return centroids;
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
        //Console.Error.WriteLine($"Ash:{ash.X} {ash.Y}");
        //Any zombie within a 2000 unit range around Ash is destroyed.
        int killedZombies = ash.Attack(board);
        int addedScore = ComputeKillScore(board, killedZombies);
        //Zombies eat any human they share coordinates with.
        foreach (Zombie z in zombies)
        {
            z.Attack(board);
        }
        board.score += addedScore;
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
            gm.currentPlayer = player;
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
            Point pt = gm.NextMove(board);
            Console.WriteLine($"{pt.X} {pt.Y}"); // Your destination coordinates
        }
    }
}