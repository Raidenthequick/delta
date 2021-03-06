﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Delta;
using Delta.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace Delta.Examples.Entities
{
    public class GravitySink : TransformableEntity
    {
        const int NUM_PARTICLES = 1000;
        const float GRAVITY = 6.6f;
        const float M1 = 100;
        const float M2 = 2;
        Lucas _lucas;

        internal struct Particle {
            public Vector2 Position;
            public Vector2 Velocity;
        }

        Particle[] _particles;

        public GravitySink()
        {
            _particles = new Particle[NUM_PARTICLES];
        }

        protected override void Initialize()
        {
            for (int i = 0; i < NUM_PARTICLES; i++)
                _particles[i] = new Particle()
                {
                    Velocity = Vector2.Zero,
                    Position = Position + new Vector2(G.Random.Between(10, 20), G.Random.Between(10, 20)),
                };

            base.Initialize();
        }

        protected override void LightUpdate(DeltaGameTime time)
        {
            if (_lucas == null)
                _lucas = TransformableEntity.Get("Lucas") as Lucas;
            for (int i = 0; i < NUM_PARTICLES; i++)
            {
                _particles[i].Velocity = -GRAVITY * (M1 * M2 / MathExtensions.Square(Vector2.Distance(_particles[i].Position, _lucas.Position))) * (_particles[i].Position - _lucas.Position);
                _particles[i].Position += _particles[i].Velocity * (float)time.ElapsedSeconds;
            }
        }

        protected override void Draw(DeltaGameTime time, SpriteBatch spriteBatch)
        {
            foreach (Particle p in _particles)
                spriteBatch.Draw(G.PixelTexture, p.Position, Color.White);
        }
        
    }
}
