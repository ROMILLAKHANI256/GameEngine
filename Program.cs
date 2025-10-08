using System;
using OpenTK.Windowing.Desktop;

namespace PhongOpenTK
{
    class Program
    {
        static void Main()
        {
            var nativeSettings = new NativeWindowSettings()
            {
                Size = new OpenTK.Mathematics.Vector2i(1280, 720),
                Title = "Romil GAM-531 Assignment-05 Phong Lighting - OpenTK"
            };

            var game = new Game(GameWindowSettings.Default, nativeSettings);
            game.Run();
        }
    }
}
