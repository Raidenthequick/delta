using System;
using Delta;
using System.Diagnostics;
using Delta.Examples;

namespace Delta.Bootstrap
{
#if WINDOWS || XBOX
    static class Program
    {
        enum Examples
        {
            Input,
            Animation,
            Game,
            Audio,
            Tiled,
            Collision,
            Racing,
            Zelda,
        }

#if WINDOWS
        [STAThread]
#endif
        static void Main(string[] args)
        {
            Examples example = Examples.Animation;

            switch (example)
            {
                case Examples.Input:
                    RunExample<InputExample>();
                    break;
                case Examples.Animation:
                    RunExample<AnimationExample>();
                    break;
                case Examples.Game:
                    RunExample<GameExample>();
                    break;
                case Examples.Audio:
                    RunExample<AudioExample>();
                    break;
                case Examples.Tiled:
                    RunExample<TiledExample>();
                    break;
                case Examples.Collision:
                    RunExample<CollisionExample>();
                    break;
                case Examples.Racing:
                    RunExample<RacingExample>();
                    break;
                case Examples.Zelda:
                    RunExample<ZeldaExample>();
                    break;
            }
        }

        static void RunExample<T>() where T: G, new()
        {
            using (T game = new T())
                game.Run();
        }
    }
#endif
}

