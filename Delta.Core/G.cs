using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Delta.UI;
using Delta.Input;
using Delta.Audio;
using Delta.Graphics;
using Delta.Graphics.Effects;
using Delta.Collision;

#if WINDOWS
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using Delta.Editor;
using Polenter.Serialization;
#endif

namespace Delta
{
    /// <summary>
    /// Static game class wrapper for Delta games.
    /// </summary>
    public class G : Game
    {
        internal const bool LETTERBOX = false;
        internal const float ASPECT_RATIO_SD = 800f / 600f;
        internal const float ASPECT_RATIO_HD = 1920f / 1080f;
        internal const Microsoft.Xna.Framework.Input.Keys EDITOR_KEY = Microsoft.Xna.Framework.Input.Keys.F12;

#if WINDOWS
        internal static bool _refreshPropertyGrid = false;
        internal static float _refreshPropertyGridTimer = 0;
        int _debugObjectIndex = 0;
        List<Entity> _debugObjects = new List<Entity>();
        public static SharpSerializer sharpSerializer;
#endif

        internal static ResourceContentManager _embedded = null;
        internal static GraphicsDeviceManager _graphicsDeviceManager = null;
        internal static bool _lateInitialized = false;
        internal static Game _instance = null;
        internal static DeltaGameTime _time = new DeltaGameTime();

        public new static ContentManager Content { get; private set; }
        public static InputManager Input { get; private set; }
        public static AudioManager Audio { get; private set; }
        public static World World { get; private set; }
        public static UIManager UI { get; private set; }
        public static PostEffects CurrentPostEffects { get; set; }

        public new static GraphicsDevice GraphicsDevice { get { return _instance.GraphicsDevice; } }
        public new static bool IsMouseVisible { get { return _instance.IsMouseVisible; } set { _instance.IsMouseVisible = value; } }
        public static bool IsVSyncEnabled { get { return _graphicsDeviceManager.SynchronizeWithVerticalRetrace; } }
        public static SpriteBatch SpriteBatch { get; private set; }
        public static PrimitiveBatch PrimitiveBatch { get; private set; }
        public static CollisionWorld Collision { get; private set; }
        public static Texture2D PixelTexture { get; private set; }
        public static SpriteFont Font { get; private set; }
        public static Random Random { get; private set; }
        public static Rectangle ScreenArea { get; private set; }
        public static Vector2 ScreenCenter { get; private set; }

        public static DeltaEffect DeltaEffect { get; private set; }
        public static SimpleEffect SimpleEffect { get; private set; }

#if WINDOWS
        public static Process Process { get; set; }
        public static Form GameForm { get; set; }
        public static EditorForm EditorForm { get; set; }
#endif

        public G(int screenWidth, int screenHeight)
            : this(screenWidth, screenHeight, true, "Content")
        {
        }

        public G(int screenWidth, int screenHeight, bool vSync, string contentDirectory)
            : base()
        {
            _instance = this;
            Content = base.Content;
            Content.RootDirectory = contentDirectory;
            IsFixedTimeStep = true;
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            _graphicsDeviceManager.PreferredBackBufferWidth = screenWidth;
            _graphicsDeviceManager.PreferredBackBufferHeight = screenHeight;
            _graphicsDeviceManager.DeviceReset += OnDeviceReset;
            _graphicsDeviceManager.PreparingDeviceSettings += OnPreparingDeviceSettings;
            _graphicsDeviceManager.SynchronizeWithVerticalRetrace = vSync;
            _embedded = new ResourceContentManager(Services, EmbeddedContent.ResourceManager);
#if DEBUG
            base.IsMouseVisible = true;
            Window.AllowUserResizing = true;
#endif
            World = new World();
            UI = new UIManager();
            Random = new Random();
            Input = new InputManager();
            Audio = new AudioManager(@"Content\Audio\audio.xgs", @"Content\Audio\Sound Bank.xsb", @"Content\Audio\Wave Bank.xwb", @"Content\Audio\StreamingBank.xwb");
            Collision = new CollisionWorld(new SeperatingAxisNarrowphase(), new UniformGridBroadphase());
            ScreenArea = new Rectangle(0, 0, screenWidth, screenHeight);
            ScreenCenter = ScreenArea.Center.ToVector2();
#if WINDOWS
            Process = Process.GetCurrentProcess();
            GameForm = (Form)Control.FromHandle(Window.Handle);
            EditorForm = new EditorForm();
            EditorForm.Icon = GameForm.Icon;

            //load initial dependencies
            EditorForm.LoadDependencies();

            //initialize serializer here
            InitializeSharpSerializer();
#endif
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            PrimitiveBatch = new PrimitiveBatch(GraphicsDevice);
            PixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            PixelTexture.SetData<Color>(new Color[] { Color.White });
            Font = _embedded.Load<SpriteFont>("TinyFont");
            DeltaEffect = new DeltaEffect(_embedded.Load<Effect>("DeltaEffect"));
            SimpleEffect = new SimpleEffect(_embedded.Load<Effect>("SimpleEffect"));
          
            World.InternalLoadContent();
            UI.InternalLoadContent();
        }

        protected virtual void LateInitialize()
        {
            GC.Collect();
            ResetElapsedTime();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _time.ElapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _time.TotalSeconds += _time.ElapsedSeconds;
            Input.Update(_time);
            Audio.Update(_time);
            if (!_lateInitialized)
            {
                _lateInitialized = true;
                LateInitialize();
            }
            //if (_lateInitialized) // only update after the game has late initialized, otherwise entities will lateinitialize first.
            //{
                World.InternalUpdate(_time);
                UI.InternalUpdate(_time);
            //}
            Collision.Simulate(_time.ElapsedSeconds); // simulate after the world update! otherwise simulating a previous frame's worldstate.
#if WINDOWS
            if (G.Input.Keyboard.IsPressed(EDITOR_KEY))
            {
                if (!EditorForm.Visible)
                {
                    EditorForm.Show();
                    EditorForm.Focus();
                }
                else
                {
                    EditorForm.Hide();
                }
            }
            if (_refreshPropertyGrid)
            {
                if (_refreshPropertyGridTimer <= 0)
                {
                    G.EditorForm.grdProperty.Refresh();
                    _refreshPropertyGridTimer = 0;
                    _refreshPropertyGrid = false;
                }
                else
                    _refreshPropertyGridTimer -= _time.ElapsedSeconds;
            }

            //mouse events
            if (G.Input.Mouse.MouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed
                && Form.ActiveForm == Form.FromHandle(this.Window.Handle))
            {
                OnLeftClick();
            }

            //key events
            if (G.Input.Keyboard.IsPressed(Microsoft.Xna.Framework.Input.Keys.LeftControl))
            {
                OnLeftControl();
            }
            if (G.Input.Keyboard.IsPressed(Microsoft.Xna.Framework.Input.Keys.RightControl))
            {
                OnRightControl();
            }
#endif
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            World.InternalDraw(World.Time, G.SpriteBatch);
            UI.InternalDraw(_time, G.SpriteBatch);
        }

        public static void ToggleFullScreen()
        {
            _graphicsDeviceManager.ToggleFullScreen();
        }

        void OnDeviceReset(object sender, EventArgs e) 
        {
            var pp = GraphicsDevice.PresentationParameters;
            int width = pp.BackBufferWidth;
            int height = pp.BackBufferHeight;

            // scale height to maintain widescreen aspect ratio
            if (width / height == (int)ASPECT_RATIO_SD && LETTERBOX)
            {
                height = (int)((float)width * (1f / ASPECT_RATIO_HD));
                GraphicsDevice.Viewport = new Viewport(0, (pp.BackBufferHeight - height) / 2, width, height);
            }

            // xna will try to maintain the backbuffer resolution, however the monitor may not support it.
            // xna will then pick the next best resolution. eg. 1920x1080 fullscreened becomes 1600x900.
            // therefore the original resolution is not maintained and ScreenArea needs to update accordingly.
            Vector2 increaseFactor = new Vector2((float)width / (float)ScreenArea.Width, (float)height / (float)ScreenArea.Height);
            ScreenArea = new Rectangle(0, 0, width, height);
            ScreenCenter = ScreenArea.Center.ToVector2();

            // re-align the camera's offset
            World.Camera.Offset = World.Camera.Offset * increaseFactor;
        }

        void OnPreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            var pp = e.GraphicsDeviceInformation.PresentationParameters;
            if (pp.IsFullScreen)
            {
                // testing: manually set the resolution; needs a supported check.
                //pp.BackBufferWidth = 1920;
                //pp.BackBufferHeight = 1080;
            }
        }

#if WINDOWS
        //grabs all entities at current mouse cursor coordinates
        protected void GetEntitiesAtMouse()
        {
            _debugObjects.Clear();

            foreach (var entity in
            from gameComponent in Entity.GlobalEntities
            select gameComponent as TransformableEntity)
            {
                if (entity != null && entity.IsVisible
                    && entity.BoundingBox.Contains(G.World.Camera.ToWorldPosition(G.Input.Mouse.Position).ToPoint()))
                {
                    _debugObjects.Add(entity);
                }
            }
        }

        //selects an entity in the property grid at the current index within the debug entity list
        private void SelectEntity()
        {
            if (_debugObjects.Count > 0)
            {
                G.EditorForm.SelectObject(_debugObjects[_debugObjectIndex]);
            }
        }

        //OnClick "event", override for custom mouse behavior
        protected virtual void OnLeftClick()
        {
            //trap all found at mouse
            GetEntitiesAtMouse();

            //choose first found
            _debugObjectIndex = 0;
            SelectEntity();
        }

        //custom key events
        protected virtual void OnLeftControl()
        {
            //for left control, decrement index
            _debugObjectIndex = (_debugObjectIndex - 1).Wrap(0, _debugObjects.Count - 1);
            SelectEntity();
        }

        protected virtual void OnRightControl()
        {
            //for right control, increment index
            _debugObjectIndex = (_debugObjectIndex + 1).Wrap(0, _debugObjects.Count - 1);
            SelectEntity();
        }

        public static void InitializeSharpSerializer()
        {
            var sxs = new SharpSerializerXmlSettings();
            sxs.AdvancedSettings.AttributesToIgnore.Add(typeof(ContentSerializerIgnoreAttribute));
            sxs.IncludeAssemblyVersionInTypeName = false;
            sxs.IncludeCultureInTypeName = false;
            sxs.IncludePublicKeyTokenInTypeName = false;

            //simple types to include
            sxs.AdvancedSettings.SimpleTypes = new Polenter.Serialization.Core.SimpleTypes(new Type[]
            {
                typeof(Range),
                typeof(TimedRange),
                typeof(Vector2),
                typeof(Vector3)
            });
            sxs.AdvancedSettings.SimpleValueConverter = new Delta.Serialization.DeltaSimpleValueConverter();
            sharpSerializer = new SharpSerializer(sxs);
        }
#endif

    }
}
