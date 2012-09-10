using System;
using System.ComponentModel;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Delta.Transformations;
using Delta.Collision;

#if WINDOWS
using System.Drawing.Design;
using Delta.Editor;
#endif

namespace Delta
{
    /// <summary>
    /// Base class for all transformable game entities.
    /// </summary>
    public abstract class TransformableEntity : Entity
    {
        Transformer _transformer;

        /// <summary>
        /// Gets the position used when rendering the <see cref="TransformableEntity"/>.
        /// </summary>
        /// <remarks>To move this <see cref="TransformableEntity"/> see <see cref="Position"/> and <see cref="Offset"/></remarks>
        [ContentSerializerIgnore, Browsable(false)]
        protected internal Vector2 RenderPosition { get; private set; }
        /// <summary>
        /// Gets the origin used when rendering the <see cref="TransformableEntity"/>.
        /// </summary>
        [ContentSerializerIgnore, Browsable(false)]
        protected internal Vector2 RenderOrigin { get; private set; }
        /// <summary>
        /// Gets the rotation used when rendering the <see cref="TransformableEntity"/> expressed in radians.
        /// </summary>
        [ContentSerializerIgnore, Browsable(false)]
        protected internal float RenderRotation { get; private set; }
        /// <summary>
        /// Gets the size used when rendering the <see cref="TransformableEntity"/>.
        /// </summary>
        [ContentSerializerIgnore, Browsable(false)]
        protected internal Vector2 RenderSize { get; private set; }
        /// <summary>
        /// Gets the premultipled color used when rendering the <see cref="TransformableEntity"/>.
        /// </summary>
        [ContentSerializer, Browsable(false)]
        protected internal Color RenderColor { get; private set; }

        Vector2 _position = Vector2.Zero;
        /// <summary>
        /// Gets or sets the position of the <see cref="TransformableEntity"/>.
        /// </summary>
        /// <remarks>The default is zero with a <see cref="Vector2"/> value of {0,0}.</remarks>
        [ContentSerializer, Description("The position of the game object expressed in pixels.\nDefault is (0, 0)."), Category("Transform"), DefaultValue(typeof(Vector2), "0,0")]
        public virtual Vector2 Position
        {
            get { return _position; }
            set
            {
                if (_position != value)
                {
                    _position = value;
                    OnPositionChanged();
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// X position accessor
        /// </summary>
        [Polenter.Serialization.XMLAttribute("x")]
        [Browsable(false)]
        public float TiledX
        {
            get { return BoundingBox.X; }
            set { _position.X = value; }
        }

        /// <summary>
        /// Y position accessor
        /// </summary>
        [Polenter.Serialization.XMLAttribute("y")]
        [Browsable(false)]
        public float TiledY
        {
            get { return BoundingBox.Y; }
            set { _position.Y = value; }
        }

        Vector2 _offset = Vector2.Zero;
        /// <summary>
        /// Gets or sets the positional offset of the <see cref="TransformableEntity"/>.
        /// </summary>
        /// <remarks>The default is zero with a <see cref="Vector2"/> value of {0,0}.</remarks>
        [ContentSerializer, Description("The positional offset of the game object expressed in pixels.\nDefault is (0, 0)."), Category("Transform"), DefaultValue(typeof(Vector2), "0,0")]
        public virtual Vector2 Offset
        {
            get { return _offset; }
            set
            {
                if (_offset != value)
                {
                    _offset = value;
                    OnPositionChanged();
                    OnPropertyChanged();
                }
            }
        }

        Vector2 _size = Vector2.Zero;
        /// <summary>
        /// Gets or sets the size of the <see cref="TransformableEntity"/> <b>before it's scaled</b>.
        /// </summary>
        /// <remarks>The default is zero with a <see cref="Vector2"/> value of {0,0}.</remarks>
        [ContentSerializer, Description("The size of the game object expressed in pixels before it's scaled.\nDefault is (0, 0)."), Category("Transform"), DefaultValue(typeof(Vector2), "0,0")]
        public virtual Vector2 Size
        {
            get { return _size; }
            set
            {
                if (_size != value)
                {
                    _size = value;
                    OnSizeChanged();
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// width accessor
        /// </summary>
        [Polenter.Serialization.XMLAttribute("width")]
        [Browsable(false)]
        public float TiledWidth
        {
            get { return _size.X; }
            set { _size.X = value; }
        }

        /// <summary>
        /// height accessor
        /// </summary>
        [Polenter.Serialization.XMLAttribute("height")]
        [Browsable(false)]
        public float TiledHeight
        {
            get { return _size.Y; }
            set { _size.Y = value; }
        }

        Vector2 _scale = Vector2.One;
        /// <summary>
        /// Gets or sets the scale of the <see cref="TransformableEntity"/> expressed in decimal percentage.
        /// </summary>
        /// <remarks>The default is 100% with a <see cref="Vector2"/> value of {1.0f,1.0f}.</remarks>
        [ContentSerializer, Description("The scale of the game object expressed in decimal percentage.\nDefault is (1, 1)."), Category("Transform"), DefaultValue(typeof(Vector2), "1,1")]
        public virtual Vector2 Scale
        {
            get { return _scale; }
            set
            {
                if (_scale != value)
                {
                    _scale = value;
                    OnScaleChanged();
                    OnPropertyChanged();
                }
            }
        }

        float _rotation = 0.0f;
        /// <summary>
        /// Gets or sets the rotation of the <see cref="TransformableEntity"/> expressed in degrees.
        /// </summary>
        /// <remarks>The default is zero with a <see cref="float"/> value of 0.0f. <b>Positive</b> values result in a <b>clockwise</b> rotation. <b>Negative</b> values result in a <b>counter-clockwise</b> rotation.</remarks>
        [ContentSerializer, Description("The rotation of the game object expressed in degrees.\nDefault is 0."), Category("Transform"), DefaultValue(0.0f)]
        public virtual float Rotation
        {
            get { return _rotation; }
            set
            {
                if (_rotation != value)
                {
                    _rotation = value;
                    OnRotationChanged();
                    OnPropertyChanged();
                }
            }
        }

        Vector2 _origin = Vector2.Zero;
        /// <summary>
        /// Gets or sets the positional origin of the <see cref="TransformableEntity"/> expressed in decimal percetange relative to the <see cref="RenderSize"/>.
        /// </summary>
        /// <remarks>The default is 0% (the top left) with a <see cref="Vector2"/> value of {0.0f, 0.0f}.</remarks>
        [ContentSerializer, Description("The positional origin of the game object expressed in decimal percentage.\nDefault is (0, 0)."), Category("Transform"), DefaultValue(typeof(Vector2), "0,0")]
        public Vector2 Origin
        {
            get { return _origin; }
            set
            {
                if (_origin != value)
                {
                    _origin = value.Clamp(Vector2.Zero, Vector2.One);
                    OnOriginChanged();
                    OnPropertyChanged();
                }
            }
        }

        Vector2 _pivot = Vector2.One * 0.5f;
        /// <summary>
        /// Gets or sets the positional pivot of the <see cref="TransformableEntity"/> expressed in decimal percentange relative to the <see cref="RenderSize"/>.
        /// </summary>
        /// <remarks>The default is 50% (the center) with a <see cref="Vector2"/> value of {0.5f, 0.5f}.</remarks>
        [ContentSerializer, Description("The positional pivot of the game object expressed in decimal percentage.\nDefault is (0.5, 0.5)."), Category("Transform"), DefaultValue(typeof(Vector2), "0.5,0.5")]
        public Vector2 Pivot
        {
            get { return _pivot; }
            set
            {
                if (_pivot != value)
                {
                    _pivot = value.Clamp(Vector2.Zero, Vector2.One);
                    OnPivotChanged();
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// return a rectangle of the entity's bounding box
        /// </summary>
        public Rectangle BoundingBox
        {
            get { return new Rectangle((int)(Position.X + Offset.X - RenderOrigin.X), (int)(Position.Y + Offset.Y - RenderOrigin.Y), (int)(Size.X * Scale.X), (int)(Size.Y * Scale.Y)); }
        }

        [ContentSerializer(ElementName = "Color")]
        Color _tint = Color.White;
        /// <summary>
        /// Gets or sets the color of the <see cref="TransformableEntity"/> <b>before it's multipled with <see cref="Alpha"/></b>.
        /// </summary>
        /// <remarks>The default is white with a <see cref="Color"/> value of {255, 255, 255, 255}.</remarks>
        [ContentSerializerIgnore, Description("The tint color of the game object.\nDefault is (255, 255, 255, 255)."), Category("Transform"), DefaultValue(typeof(Color), "255,255,255,255"), 
#if WINDOWS
        Editor(typeof(ColorUIEditor), typeof(UITypeEditor))
#endif
        ]
        public virtual Color Tint
        {
            get { return _tint; }
            set
            {
                if (_tint != value)
                {
                    _tint = value;
                    OnTintChanged();
                    OnPropertyChanged();
                }
            }
        }

        float _alpha = 1.0f;
        /// <summary>
        /// Gets or sets the alpha component of the <see cref="TransformableEntity"/> expressed in decimal percentage. Used to premultiple the <see cref="Tint"/>.
        /// </summary>
        /// <remarks>The default is 100% with a <see cref="float"/> value of 1.0f.</remarks>
        [ContentSerializer, Description("The alpha component of the game object expressed in decimal percentage.\nDefault is 1."), Category("Transform"), DefaultValue(1.0f)]
        public virtual float Alpha
        {
            get { return _alpha; }
            set
            {
                value = value.Clamp(0.0f, 1.0f);
                if (_alpha != value)
                {
                    _alpha = value;
                    OnAlphaChanged();
                    OnPropertyChanged();
                }
            }
        }

        IWrappedBody _wrappedBody = null;
        /// <summary>
        /// Gets or sets the <see cref="IWrappedBody"/> of the <see cref="TransformableEntity"/>.
        /// </summary>
        [ContentSerializerIgnore, Browsable(false)]
        public IWrappedBody WrappedBody
        {
            get { return _wrappedBody; }
            set
            {
                if (value != null)
                {
                    _wrappedBody = value;
                    OnWrappedBodyChanged();
                }
            }
        }

        float _timeScale = 1.0f;
        /// <summary>
        /// Gets or sets the time scale of the <see cref="TransformableEntity"/> expressed in decimal percentage.
        /// </summary>
        [ContentSerializer, Description("The time scale the game object expressed in decimal percentage.\nDefault is 1."), Category("Transform"), DefaultValue(1.0f)]
        public float TimeScale
        {
            get { return _timeScale; }
            set
            {
                if (_timeScale != value)
                {
                    _timeScale = value;
                    OnPropertyChanged();
                }
            }
        }

        bool _fadeRandomly = false;
        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="TransformableEntity"/> fades randomly.
        /// </summary>
        [ContentSerializer, Description("Indicates whether the game object fades in/out randomly.\nDefault is 0."), Category("Transform"), DefaultValue(false)]
        public bool FadeRandomly
        {
            get { return _fadeRandomly; }
            set
            {
                if (_fadeRandomly != value)
                {
                    _fadeRandomly = value;
                    OnPropertyChanged();
                }
            }
        }

        TimedRange _fadeRange = TimedRange.Empty;
        /// <summary>
        /// Gets or sets the fade timed range of the <see cref="TransformableEntity"/>.
        /// </summary>
        [ContentSerializer, Description("The fade timed range of the game object.\nDefault is (0, 0, 0)."), Category("Transform"), DefaultValue(typeof(TimedRange), "0,0,0")]
        public TimedRange FadeRange
        {
            get { return _fadeRange; }
            set
            {
                if (_fadeRange != value)
                {
                    _fadeRange = value;
                    if (_transformer != null)
                        _transformer.ClearSequence();
                    if (!value.IsEmpty())
                    {
                        if (FadeRandomly)
                        {
                            // start the transformer off in a random position within the range.
                            float startupAlpha = G.Random.Between(_fadeRange.Lower, _fadeRange.Upper);
                            _transformer = Transformer.ThisEntity(this).FadeTo(startupAlpha, (startupAlpha / _fadeRange.Upper) * _fadeRange.Duration);
                            _transformer.OnTransformFinished(() =>
                            {
                                // remove the start-up transform logic.
                                _transformer.ClearSequence();
                                _transformer.OnTransformFinished(null);
                                // 50/50 chance to fade from lower to upper or from upper to lower. provides more fade varieties.
                                if (G.Random.FiftyFifty())
                                    _transformer.FadeTo(_fadeRange.Upper, _fadeRange.Duration).FadeTo(_fadeRange.Lower, _fadeRange.Duration);
                                else
                                    _transformer.FadeTo(_fadeRange.Lower, _fadeRange.Duration).FadeTo(_fadeRange.Upper, _fadeRange.Duration);
                                _transformer.Loop();
                            });
                        }
                        else
                        {
                            _transformer = Transformer.ThisEntity(this).FadeTo(_fadeRange.Lower, _fadeRange.Duration).FadeTo(_fadeRange.Upper, _fadeRange.Duration);
                            _transformer.Loop();
                        }
                    }
                    else
                        _transformer = null;
                    OnPropertyChanged();
                }
            }
        }

        TimedRange _flickerRange = TimedRange.Empty;
        /// <summary>
        /// Gets or sets the flicker range of the <see cref="TransformableEntity"/>.
        /// </summary>
        [ContentSerializer, Description("The flicker timed range of the game object.\nDefault is (0, 0, 0)."), Category("Transform"), DefaultValue(typeof(TimedRange), "0,0,0")]
        public TimedRange FlickerRange
        {
            get { return _flickerRange; }
            set
            {
                if (_flickerRange != value)
                {
                    _flickerRange = value;
                    if (_transformer != null)
                        _transformer.ClearSequence();
                    if (!value.IsEmpty())
                    {
                        _transformer = Transformer.ThisEntity(this).FlickerFor(_flickerRange.Lower, _flickerRange.Upper, _flickerRange.Duration);
                        _transformer.Loop();
                    }
                    else
                        _transformer = null;
                    OnPropertyChanged();
                }
            }
        }

        TimedRange _blinkRange = TimedRange.Empty;
        /// <summary>
        /// Gets or sets the blink range of the <see cref="TransformableEntity"/>.
        /// </summary>
        [ContentSerializer, Description("The blink timed range of the game object.\nDefault is (0, 0, 0)."), Category("Transform"), DefaultValue(typeof(TimedRange), "0,0,0")]
        public TimedRange BlinkRange
        {
            get { return _blinkRange; }
            set
            {
                if (_blinkRange != value)
                {
                    _blinkRange = value;
                    if (_transformer != null)
                        _transformer.ClearSequence();
                    if (!value.IsEmpty())
                    {
                        _transformer = Transformer.ThisEntity(this).BlinkFor(_blinkRange.Lower, _blinkRange.Duration);
                        _transformer.Loop();
                    }
                    else
                        _transformer = null;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        public TransformableEntity()
            : base()
        {
            RenderColor = Color.White;
        }

        protected override void Recycle(bool isReleasing)
        {
            RenderPosition = Vector2.Zero;
            RenderOrigin = Vector2.Zero;
            RenderRotation = 0.0f;
            RenderSize = Vector2.Zero;
            RenderColor = Color.White;
            _alpha = 1.0f;
            _offset = Vector2.Zero;
            _origin = Vector2.Zero;
            _pivot = Vector2.One * 0.5f;
            _position = Vector2.Zero;
            _rotation = 0.0f;
            _scale = Vector2.One;
            _size = Vector2.Zero;
            _tint = Color.White;
            _wrappedBody = null;
            base.Recycle(isReleasing);
        }

#if WINDOWS
        protected internal override bool SetValue(string name, string value)
        {
            switch (name)
            {
                case "pos":
                case "position":
                    Position = Vector2Extensions.Parse(value);
                    return true;
                case "offset":
                    Offset = Vector2Extensions.Parse(value);
                    return true;
                case "scale":
                    Scale = Vector2Extensions.Parse(value);
                    return true;
                case "rot":
                case "rotation":
                    Rotation = float.Parse(value, CultureInfo.InvariantCulture);
                    return true;
                case "tint":
                case "color":
                    Tint = value.ToColor();
                    return true;
                case "opacity":
                case "alpha":
                case "a":
                    Alpha = float.Parse(value, CultureInfo.InvariantCulture);
                    return true;
                case "fade":
                    _fadeRange = TimedRange.Parse(value);
                    return true;
                case "faderandom":
                    _fadeRange = TimedRange.Parse(value);
                    FadeRandomly = true;
                    return true;
                case "flicker":
                    _flickerRange = TimedRange.Parse(value);
                    return true;
                case "blink":
                    _blinkRange = TimedRange.Parse(value);
                    return true;
            }
            return base.SetValue(name, value);
        }
#endif

        /// <summary>
        /// Updates the <see cref="TransformableEntity"/>'s <see cref="RenderSize"/>.
        /// </summary>
        protected virtual void UpdateRenderSize()
        {
            RenderSize = Size * Scale;
        }

        /// <summary>
        /// Updates the <see cref="TransformableEntity"/>'s <see cref="RenderPosition"/>.
        /// </summary>
        protected virtual void UpdateRenderPosition()
        {
            RenderPosition = Position + Offset - (Origin * RenderSize) + (RenderOrigin * Scale); //Multiply the scale in for a correct position.
        }

        /// <summary>
        /// Updates the <see cref="TransformableEntity"/>'s <see cref="RenderOrigin"/>.
        /// </summary>
        protected virtual void UpdateRenderOrigin()
        {
            RenderOrigin = Pivot * Size; //don't multiply the scale! SpriteBatch automatically adds the scale in for us!
        }

        /// <summary>
        /// Updates the <see cref="TransformableEntity"/>'s <see cref="RenderRotation"/>.
        /// </summary>
        protected virtual void UpdateRenderRotation()
        {
            RenderRotation = Rotation.ToRadians();
        }

        /// <summary>
        /// Updates the <see cref="TransformableEntity"/> to the <see cref="WrappedBody"/>.
        /// </summary>
        protected virtual void UpdateToWrappedBody()
        {
            if (WrappedBody != null)
            {
                WrappedBody.SimulationPosition = Position + Offset;
                WrappedBody.SimulationRotation = RenderRotation;
            }
        }

        /// <summary>
        /// Updates the <see cref="TransformableEntity"/> from the <see cref="WrappedBody"/>.
        /// </summary>
        protected virtual void UpdateFromWrappedBody()
        {
            if (WrappedBody != null)
            {
                //don't fire OnChanged functions! We don't want to get into a fight between reading and writing off the WrappedBody!
                _position = WrappedBody.SimulationPosition - Offset;
                _rotation = WrappedBody.SimulationRotation.ToDegrees();
                UpdateRenderPosition();
                UpdateRenderRotation();
            }
        }

        /// <summary>
        /// Called when the <see cref="TransformableEntity"/>'s <see cref="Position"/> has changed.
        /// </summary>
        protected internal virtual void OnPositionChanged()
        {
            UpdateRenderPosition();
            UpdateToWrappedBody();
        }

        /// <summary>
        /// Called when the <see cref="TransformableEntity"/>'s <see cref="Size"/> has changed.
        /// </summary>
        protected internal virtual void OnSizeChanged()
        {
            UpdateRenderSize();
            UpdateRenderOrigin();
            UpdateRenderPosition();
        }

        /// <summary>
        /// Called when the <see cref="TransformableEntity"/>'s <see cref="Scale"/> has changed.
        /// </summary>
        protected internal virtual void OnScaleChanged()
        {
            UpdateRenderSize();
            UpdateRenderOrigin();
            UpdateRenderPosition();
        }

        /// <summary>
        /// Called when the <see cref="TransformableEntity"/>'s <see cref="Rotation"/> has changed.
        /// </summary>
        protected internal virtual void OnRotationChanged()
        {
            UpdateRenderRotation();
            UpdateToWrappedBody();
        }

        /// <summary>
        /// Called when the <see cref="TransformableEntity"/>'s <see cref="Origin"/> has changed.
        /// </summary>
        protected virtual void OnOriginChanged()
        {
            UpdateRenderOrigin();
            UpdateRenderPosition();
        }

        /// <summary>
        /// Called when the <see cref="TransformableEntity"/>'s <see cref="Pivot"/> has changed.
        /// </summary>
        protected virtual void OnPivotChanged()
        {
            UpdateRenderOrigin();
            UpdateRenderPosition();
        }

        /// <summary>
        /// Called when the <see cref="TransformableEntity"/>'s <see cref="Tint"/> has changed.
        /// </summary>
        protected virtual void OnTintChanged()
        {
            RenderColor = Tint * Alpha;
        }

        /// <summary>
        /// Called when the <see cref="TransformableEntity"/>'s <see cref="Alpha"/> has changed.
        /// </summary>
        protected virtual void OnAlphaChanged()
        {
            RenderColor = Tint * Alpha;
        }

        /// <summary>
        /// Called when the <see cref="TransformableEntity"/>'s <see cref="WrappedBody"/> has changed.
        /// </summary>
        protected virtual void OnWrappedBodyChanged()
        {
            WrappedBody.AddToSimulation();
            WrappedBody.Owner = this;
            UpdateToWrappedBody();
        }

        /// <summary>
        /// Updates the <see cref="TransformableEntity"/>. Override this method to add custom update logic which is executed every frame.
        /// </summary>
        /// <param name="time">time</param>
        protected override void LightUpdate(DeltaGameTime time)
        {
            UpdateFromWrappedBody();
            base.LightUpdate(time);
        }

        /// <summary>
        /// Called when the <see cref="Entity"/> has been removed from an <see cref="IEntityCollection"/>.
        /// </summary>
        protected internal override void OnRemoved()
        {
            if (WrappedBody != null)
                WrappedBody.RemoveFromSimulation();
            base.OnRemoved();
        }
    }
}
