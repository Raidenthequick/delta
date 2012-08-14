using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Delta.Examples.Entities;
using Delta.Collision;
using Delta.Graphics;
using Delta.Tiled;
using Delta.UI.Controls;

namespace Delta.Examples
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class BlossomExample : ExampleBase
    {
        const string CONTROLS = "[wads] camera movement.";
        Map _map;
        TransformableEntity _player;

        public BlossomExample() : base("BlossomExample")
        {
            ClearColor = Color.Black;
            G.World.Camera.Offset = G.ScreenCenter;
            G.World.Camera.ZoomImmediate(4);
        }

        protected override void LoadContent()
        {
            _map = Content.Load<Map>(@"Maps\Plains\3");
            G.World.Camera.BoundsEnabled = true;
            G.World.Camera.BoundedArea = new Rectangle(0, 0, _map.Width * _map.TileWidth, _map.Height * _map.TileHeight);
            G.Collision.DefineWorld(_map.Width * _map.TileWidth, _map.Height * _map.TileHeight, 32);

            G.World.Add(_map);
            G.World.Camera.Follow(_player = TransformableEntity.Get("Lily") as TransformableEntity);

            //add objects to editor form
            List<object> objs = Entity.GlobalEntities.Cast<object>().ToList<object>();
            G.EditorForm.AddObjects(objs);

            Label lblControls = new Label();
            lblControls.Text.Append(CONTROLS);
            lblControls.Position = new Vector2(G.ScreenCenter.X, 0);
            lblControls.BackColor = Color.Gray;
            lblControls.ForeColor = Color.White;
            G.UI.HUD.Add(lblControls);

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            G.World.Camera.Position += G.Input.WasdDirection;
            base.Update(gameTime);
        }

        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(ClearColor);
            base.Draw(gameTime);
#if DEBUG
            Matrix view = G.World.Camera.View;
            Matrix projection = G.World.Camera.Projection;
            stringBuilder.Clear();
            foreach (var pool in Pool._pools)
            {
                stringBuilder.AppendLine(pool.ToString());
            }
            G.SpriteBatch.Begin();
            G.SpriteBatch.DrawString(G.Font, stringBuilder, Vector2.One * 50, Color.White);
            G.SpriteBatch.End();
            G.Collision.DrawDebug(ref view, ref projection);
            //PoolManager.DebugDraw();
            //G.World.DebugDraw();
#endif
        }
    }
}
