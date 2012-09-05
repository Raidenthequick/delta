using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Delta.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Delta.Collision;
using System.ComponentModel;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace Delta.Examples.Entities
{
    public class Barrel : AnimatedSpriteEntity
    {
        public Barrel()
        {
            SpriteSheetName = @"Graphics\SpriteSheets\16x16";
            Origin = Vector2.One * 0.5f;
            Play("barrel");
        }

        protected override void Initialize()
        {
            WrappedBody = CollisionBody.CreateBody(new Box(16, 16));
            WrappedBody.Collision += OnCollision;
            WrappedBody.SetGroup(CollisionGroups.Group2);
            base.Initialize();
        }

        protected bool OnCollision(IWrappedBody them, Vector2 normal)
        {
            Lily link = them.Owner as Lily;
            if (link != null)
            {
                Explode();
                RemoveNextFrame();
            }
            return true;
        }

        protected override void OnPositionChanged()
        {
            base.OnPositionChanged();
            Depth = Position.Y;
        }

        public void Explode()
        {
            G.Audio.PlaySound("SFX_LargeExplosion", true);
            Visuals.Create(@"Graphics\SpriteSheets\16x16", "explode", Position);
            Visuals.CreateShatter(@"Graphics\SpriteSheets\16x16", "barrelDebris", Position, 13);
            G.World.Camera.Shake(10, 0.5f, ShakeAxis.X | ShakeAxis.Y);
        }

    }
}
