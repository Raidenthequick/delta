﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Delta.Input;
using Delta.Input.States;

namespace Delta.UI
{
    public class UIManager : EntityManager<BaseScreen>
    {
        public HUD HUD { get; internal set; }
        public BaseScreen ActiveScreen { get; internal set; }
        public BaseControl FocusedControl { get; set; }
        public BaseControl ClickedControl { get; set; }
        public BaseControl CaptureControl { get; set; }
        public BaseControl EnteredControl { get; set; }

        internal Action<Keys> _keyDown;
        internal Action<Keys> _keyPress;
        internal Action<Keys> _keyUp;

        int _keyboardDelay = 0;
        public int KeyboardDelay
        {
            get
            {
                if (_keyboardDelay == 0)
                    _keyboardDelay = System.Windows.Forms.SystemInformation.KeyboardDelay;
                return _keyboardDelay;
            }
        }

        public UIManager()
            : base()
        {
            HUD = new HUD();
            _keyDown = KeyDown;
            _keyPress = KeyPress;
            _keyUp = KeyUp;
        }

        protected override void LightUpdate(DeltaTime time)
        {
            base.LightUpdate(time);
            HUD.InternalUpdate(time);
        }

        protected override void Draw(DeltaTime time, SpriteBatch spriteBatch)
        {
            base.Draw(time, spriteBatch);
            HUD.InternalDraw(time, spriteBatch);
            if (ActiveScreen != null)
                ActiveScreen.InternalDraw(time, spriteBatch);
        }

        public override void Add(BaseScreen item)
        {
            base.Add(item);
            if (ActiveScreen == null)
                ActiveScreen = item;
        }

        public override void Remove(BaseScreen item)
        {
            if (ActiveScreen == item)
                ActiveScreen = null;
            base.Remove(item);
        }

#if WINDOWS
        internal void MouseMove()
        {
            bool handled = false;
            if (CaptureControl != null)
                handled = CaptureControl.ProcessMouseMove();
            if (!handled && ActiveScreen != null)
                handled = ActiveScreen.ProcessMouseMove();
            if (!handled)
                for (int x = 0; x < Children.Count; x++)
                    handled = Children[x].ProcessMouseMove();
            if (!handled)
                HUD.ProcessMouseMove();
        }

        internal void MouseDown()
        {
            bool handled = false;
            if (CaptureControl != null)
                handled = CaptureControl.ProcessMouseUp();
            if (!handled)
                for (int x = 0; x < Children.Count; x++)
                    handled = Children[x].ProcessMouseDown();
            if (!handled)
                HUD.ProcessMouseDown();
        }

        internal void MouseUp()
        {
            bool handled = false;
            if (CaptureControl != null)
                handled = CaptureControl.ProcessMouseUp();
            if (!handled)
                for (int x = 0; x < Children.Count; x++)
                    handled = Children[x].ProcessMouseUp();
            if (!handled)
                HUD.ProcessMouseUp();
        }

        internal void KeyDown(Keys key)
        {
            if (FocusedControl != null)
                FocusedControl.ProcessKeyDown(key);
        }

        internal void KeyPress(Keys key)
        {
            if (FocusedControl != null)
                FocusedControl.ProcessKeyPress(key);
        }

        internal void KeyUp(Keys key)
        {
            if (FocusedControl != null)
                FocusedControl.ProcessKeyUp(key);
        }
#endif

    }
}
