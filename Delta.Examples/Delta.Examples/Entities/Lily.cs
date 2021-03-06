using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Delta;
using Delta.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Delta.Collision;

namespace Delta.Examples.Entities
{
    public class Lily : TransformableEntity
    {
        float SPEED = 30;

        AnimatedSpriteEntity _sprite;

        public Vector2 Input { get; set; }
        public Vector2 Velocity;

        float _trailInterval = 0.1f;
        float _trailTime = 0;

        CardinalSpriteController _spriteController;

        public Lily() : base()
        {
            Name = "Lily";

            _sprite = AnimatedSpriteEntity.Create(@"Graphics\SpriteSheets\16x16");
            _sprite.Origin = new Vector2(0.5f, 0.5f);
            _sprite.Play("walkdown");

            _spriteController = new CardinalSpriteController(_sprite);
        }

        protected override void Initialize()
        {
            WrappedBody = CollisionBody.CreateBody(new Box(16, 16));
            WrappedBody.Flags = CollisionFlags.Response;
            base.Initialize();
        }

        public void SwitchBody()
        {
            if (WrappedBody.Shape is Circle)
            {
                WrappedBody.Shape= new Box(16, 16);
            }
            else if (WrappedBody.Shape is Box)
            {
                WrappedBody.Shape = new Circle(8);
            }
        }

        protected override void LightUpdate(DeltaGameTime time)
        {

            base.LightUpdate(time);

            _sprite.Position = WrappedBody.SimulationPosition;
            _sprite.InternalUpdate(time);
            Depth = Position.Y;

            if (G.Input.Keyboard.IsDown(Keys.Q))
                Rotation = (Rotation + (0.5f));
            else if (G.Input.Keyboard.IsDown(Keys.E))
                Rotation = (Rotation - (0.5f));
            Rotation = Rotation.Wrap(0, 360);

            Vector2 direction = G.Input.ArrowDirection;
            _spriteController.Walk(direction);

            Velocity = ((G.Input.Keyboard.IsDown(Keys.LeftShift)) ? direction * 2.5f : direction) * SPEED;
            Position += Velocity * (float)time.ElapsedSeconds;

            if (G.Input.Keyboard.IsPressed(Keys.Tab))
                SwitchBody();

            // if boosting leave a motion trail
            //if (G.World.SecondsPast(_trailTime + _trailInterval) && G.Input.Keyboard.IsDown(Keys.LeftShift))
            //{
            //    Visuals.CreateTrail(@"Graphics\SpriteSheets\16x16", _spriteController.ToString(), Position);
            //    _trailTime = (float)time.TotalSeconds;
            //}
        }

        protected override void Draw(DeltaGameTime time, SpriteBatch spriteBatch)
        {
            _sprite.InternalDraw(time, spriteBatch);
        }
    }
}
