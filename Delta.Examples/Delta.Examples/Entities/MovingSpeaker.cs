﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Delta.Audio;

namespace Delta.Examples.Entities
{
    public class MovingSpeaker : TransformableEntity
    {

        float _rotation;
        Texture2D _texture;
        TransformableEntity _orbiting;

        public bool IsOrbiting
        {
            get;
            private set;
        }

        public float OrbitLength
        {
            get;
            set;
        }

        public Vector2 _center
        {
            get;
            private set;
        }

        public MovingSpeaker(): base()
        {
            Name = "Speaker";
        }

        public void Orbit(TransformableEntity e)
        {
            _orbiting = e;
            IsOrbiting = true;
        }

        public void Orbit(Vector2 position)
        {
            _orbiting = null;
            _center = position;
            IsOrbiting = true;
        }

        public void StopOrbit()
        {
            IsOrbiting = false;
        }

        protected override void LoadContent()
        {
            _texture = G.Content.Load<Texture2D>(@"Graphics\Player");
            base.LoadContent();
        }

        protected override void Initialize()
        {
            G.Audio.PlayPositionalSound("BGM_Ice", this);
            base.Initialize();
        }

        protected override void LightUpdate(DeltaGameTime time)
        {
            if (!IsOrbiting)
                return;

            _rotation = MathHelper.WrapAngle(_rotation + 0.005f);
            Vector2 newPosition = Vector2.Zero;
            if (_orbiting == null)
                newPosition += _center;
            else
                newPosition += _orbiting.Position;
            newPosition.X = (float)Math.Cos(_rotation) * OrbitLength;
            newPosition.Y = (float)Math.Sin(_rotation) * OrbitLength;
            Position = newPosition;
            base.LightUpdate(time);
        }

        protected override void Draw(DeltaGameTime time, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Position, Color.White);
            base.Draw(time, spriteBatch);
        }

    }
}
