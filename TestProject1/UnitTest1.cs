using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
namespace TestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Board board = new Board();
            Zombie zombie = new Zombie(0, 100, 100, 100, 100);
            board.AddGameObject(zombie);
            Human target = new Human(0, 200, 200);
            board.AddGameObject(target);
            Assert.AreSame(target, zombie.GetClosestHuman(board));
        }

        [TestMethod]
        public void TestMethod2()
        {
            Board board = new Board();
            Zombie zombie = new Zombie(0, 100, 100, 100, 100);
            board.AddGameObject(zombie);
            Human h1 = new Human(0, 200, 200);
            Human h2 = new Human(1, 150, 150);
            Human h3 = new Human(1, 170, 170);
            board.AddGameObject(h1);
            board.AddGameObject(h2);
            board.AddGameObject(h3);
            Assert.AreSame(h2, zombie.GetClosestHuman(board));
        }

        [TestMethod]
        public void TestMethod3()
        {
            Board board = new Board();
            Zombie zombie = new Zombie(0, 100, 100, 100, 100);
            board.AddGameObject(zombie);
            Human h1 = new Human(0, 200, 200);
            Human h2 = new Human(1, 150, 150);
            Human h3 = new Human(1, 170, 170);
            board.AddGameObject(h1);
            board.AddGameObject(h2);
            board.AddGameObject(h3);
            zombie.Attack(board);
            Assert.AreEqual(0, board._gameObjects.Where(g => g.type == GameObjectType.HUMAN && g.isDead == false).Count());
        }
        [TestMethod]
        public void TestMethod4()
        {
            Board board = new Board();
            Zombie zombie = new Zombie(0, 100, 100, 100, 100);
            board.AddGameObject(zombie);
            Human h1 = new Human(0, 200, 200);
            Human h2 = new Human(1, 150, 150);
            Human h3 = new Human(1, 100, 1000);
            board.AddGameObject(h1);
            board.AddGameObject(h2);
            board.AddGameObject(h3);
            zombie.Attack(board);
            Assert.AreEqual(1, board._gameObjects.Where(g => g.type == GameObjectType.HUMAN && g.isDead == false).Count());
        }

        [TestMethod]
        public void TestMethod5()
        {
            Board board = new Board();

            Ash player = new Ash(0, 0);
            board.AddGameObject(player);
            Zombie zombie = new Zombie(0, 100, 100, 100, 100);
            board.AddGameObject(zombie);
            Human h1 = new Human(0, 200, 200);
            Human h2 = new Human(1, 150, 150);
            Human h3 = new Human(1, 100, 1000);
            board.AddGameObject(h1);
            board.AddGameObject(h2);
            board.AddGameObject(h3);

            player.Attack(board);
            Assert.AreEqual(0, board._gameObjects.Where(g => g.type == GameObjectType.ZOMBIE && g.isDead == false).Count());
            //Assert.AreEqual(3, board._gameObjects.Where(g => g.type == GameObjectType.HUMAN && g.isDead == false).Count());
        }



        [TestMethod]
        public void TestMethod6()
        {
            Board board = new Board();

            Zombie zombie = new Zombie(0, 0, 0, 282, 282);
            board.AddGameObject(zombie);
            zombie.Move(board, 500, 500, 400);
            zombie.Attack(board);
            Assert.AreEqual(282, zombie.X);
            Assert.AreEqual(282, zombie.Y);
            //Assert.AreEqual(3, board._gameObjects.Where(g => g.type == GameObjectType.HUMAN && g.isDead == false).Count());
        }

        [TestMethod]
        public void TestMethod7()
        {
            Board board = new Board();

            Ash player = new Ash(100, 100);
            board.AddGameObject(player);
            Zombie zombie = new Zombie(0, 100, 100, 100, 100);
            board.AddGameObject(zombie);
            Human h1 = new Human(0, 200, 200);
            Human h2 = new Human(1, 150, 150);
            Human h3 = new Human(1, 100, 1000);
            board.AddGameObject(h1);
            board.AddGameObject(h2);
            board.AddGameObject(h3);
            player.Move(board, 400, 400, 1000);
            player.Attack(board);
            Assert.AreEqual(400, player.X);
            Assert.AreEqual(400, player.Y);
        }

        [TestMethod]
        public void TestMethod8()
        {
            Board board = new Board();

            Ash player = new Ash(100, 0);

            Zombie zombie = new Zombie(0, 100, 2100, 100, 2100);
            board.AddGameObject(zombie);
            Human h1 = new Human(0, 200, 200);
            Human h2 = new Human(1, 150, 150);
            Human h3 = new Human(1, 100, 1000);
            board.AddGameObject(h1);
            board.AddGameObject(h2);
            board.AddGameObject(h3);
            player.Move(board, 100, 400, 1000);
            player.Attack(board);
            Assert.AreEqual(0, board._gameObjects.Where(g => g.type == GameObjectType.ZOMBIE && g.isDead == false).Count());
            //Assert.AreEqual(3, board._gameObjects.Where(g => g.type == GameObjectType.HUMAN && g.isDead == false).Count());
        }

        [TestMethod]
        public void TestMethod9()
        {
            GameManager gameManager = new GameManager();

            Assert.AreEqual(1,gameManager.Fibonnacci(1));
            Assert.AreEqual(2, gameManager.Fibonnacci(2));
            Assert.AreEqual(3, gameManager.Fibonnacci(3));
            Assert.AreEqual(5, gameManager.Fibonnacci(4));
            Assert.AreEqual(8, gameManager.Fibonnacci(5));
        }

        [TestMethod]
        public void TestMethod10()
        {
            Board board = new Board();

            Ash player = new Ash(0, 1000);

            Zombie zombie = new Zombie(0, 2600, 1000, 2200, 1000);
            board.AddGameObject(zombie);
            Human h1 = new Human(0, 0, 1000);
            Human h2 = new Human(1, 0, 8000);
            board.AddGameObject(h1);
            board.AddGameObject(h2);
            int a=player.Attack(board);
            Assert.AreEqual(1, board._gameObjects.Where(g => g.type == GameObjectType.ZOMBIE && g.isDead == false).Count());
        }

        [TestMethod]
        public void TestMethod11()
        {
            Board board = new Board();

            Ash player = new Ash(100, 4000);
            board.AddGameObject(player);
            Zombie zombie = new Zombie(0, 2246, 3715, 1848, 3666);
            board.AddGameObject(zombie);
            Human h1 = new Human(0, 100, 4000);
            board.AddGameObject(h1);
            GameManager gameManager = new GameManager();
            gameManager.Simulate(board, 100, 4000);
            Assert.AreEqual(0, board._gameObjects.Where(g => g.type == GameObjectType.ZOMBIE && g.isDead == false).Count());
        }


    }
}