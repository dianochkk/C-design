using Turtle1;
namespace TurtleTests
{
    [TestFixture]
    public class Tests
    {
        private Turtle turtle;
        private Field field;

        [SetUp]
        public void Setup()
        {
            // Инициализация перед каждым тестом
            turtle = new Turtle();
            field = new Field();
        }
        [Test]
        public void Test_MoveCommand_DrawsLine()
        {
            // Act
            field.ProcessCommand("pd");
            field.ProcessCommand("move 10");

            // Assert
            Assert.AreEqual(10, turtle.X);
            Assert.AreEqual(0, turtle.Y);
        }
        [Test]
        public void TestTurtleInitialization()
        {
            // Проверяем начальные значения черепашки
            Assert.AreEqual(0, turtle.X);
            Assert.AreEqual(0, turtle.Y);
            Assert.AreEqual(0, turtle.Angle);
            Assert.IsFalse(turtle.Tail);
            Assert.AreEqual("black", turtle.Color);
        }

        

        [Test]
        public void TestChangeAngle()
        {
            
            field.ProcessCommand("angle 90");
            Assert.AreEqual(90, turtle.Angle);

            

            // Act
            field.ProcessCommand("angle 180");
            
            Assert.AreEqual(180, turtle.Angle);
        }

        [Test]
        public void TestChangeColor()
        {
           

            // Act
            field.ProcessCommand("color red");

            Assert.AreEqual("red", turtle.Color);

            // Снова меняем цвет пера
            
            // Act
            field.ProcessCommand("color blue");
            
            Assert.AreEqual("blue", turtle.Color);
        }

        [Test]
        public void sqare()
        {
            // Опускаем перо и двигаем черепашку
            

            // Act
            field.ProcessCommand("pd");
            field.ProcessCommand("move 10");
            field.ProcessCommand("angle 90");
            field.ProcessCommand("move 10");
            field.ProcessCommand("angle 180");
            field.ProcessCommand("move 10");
            field.ProcessCommand("angle 270");
            field.ProcessCommand("move 10");

            // Проверяем, что черепашка нарисовала квадрат
            Assert.IsTrue(turtle.Tail);
            Assert.AreEqual(0, turtle.X);
            Assert.AreEqual(0, turtle.Y);
            Assert.AreEqual(1, field.figures.Count);
        }
    }
}
