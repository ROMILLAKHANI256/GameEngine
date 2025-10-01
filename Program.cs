using System;
using OpenTK.Mathematics; // Needed for Vector3, Matrix4
using WindowEngine;

namespace WindowEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            // === Vector Operations Demo ===
            Console.WriteLine("=== Vector Operations ===");
            Vector3 v1 = new Vector3(1, 3, 4);
            Vector3 v2 = new Vector3(4, 6, 9);

            Console.WriteLine($"v1 + v2 = {v1 + v2}");
            Console.WriteLine($"v1 - v2 = {v1 - v2}");
            Console.WriteLine($"Dot(v1, v2) = {Vector3.Dot(v1, v2)}");
            Console.WriteLine($"Cross(v1, v2) = {Vector3.Cross(v1, v2)}");

            // === Matrix Operations Demo ===
            Console.WriteLine("\n=== Matrix Operations ===");
            Matrix4 identity = Matrix4.Identity;
            Matrix4 scale = Matrix4.CreateScale(2f, 2f, 2f);
            Matrix4 rotate = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(90));
            Matrix4 transform = scale * rotate;

            Vector3 point = new Vector3(1, 0, 0);
            Vector3 transformedPoint = Vector3.TransformPosition(point, transform);

            Console.WriteLine($"Identity Matrix:\n{identity}");
            Console.WriteLine($"Transformed Point (scale + rotate): {transformedPoint}");

            // === Start Game Window ===
            using (Game game = new Game())
            {
                game.Run();
            }
        }
    }
}