using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Delta.Extensions;
using System.Globalization;
using Delta.Transformations;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.ComponentModel;

namespace Delta.Graphics
{
    public abstract class Emitter : TransformableEntity
    {
        [ContentSerializer]
        string _fadeInMethodString;
        Interpolation.InterpolationMethod _fadeInInterpolator;
        [ContentSerializer]
        string _fadeOutMethodString;
        Interpolation.InterpolationMethod _fadeOutInterpolator;

        [ContentSerializerIgnore]
        protected BlendState _blend;
        [ContentSerializerIgnore]
        protected float _frequency;
        [ContentSerializerIgnore]
        protected bool _explode;
        [ContentSerializerIgnore]
        protected Range _quantityRange;
        [ContentSerializerIgnore]
        protected Range _lifespanRange;
        [ContentSerializerIgnore]
        protected Range _velocityMagnitudeRange;
        [ContentSerializerIgnore]
        protected Range _accelerationMagnitudeRange;
        [ContentSerializerIgnore]
        protected Range _rotationRange;
        [ContentSerializerIgnore]
        protected Range _velocityAngleRange;
        [ContentSerializerIgnore]
        protected Range _accelerationAngleRange;
        [ContentSerializerIgnore]
        protected Range _scaleRange;
        [ContentSerializerIgnore]
        protected Range _fadeInRange;
        [ContentSerializerIgnore]
        protected Range _fadeOutRange;

        [ContentSerializer, DisplayName("Frequency"),Description(""), Category("Emitter"), Browsable(true), ReadOnly(false), DefaultValue(false)]
        public float Frequency { get { return _frequency; } set { _frequency = value; } }
        [ContentSerializer, DisplayName("Explode"), Description(""), Category("Emitter"), Browsable(true), ReadOnly(false), DefaultValue(false), Editor(typeof(Delta.Editor.RangeUIEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public bool Explode { get { return _explode; } set { _explode = value; } }
        [ContentSerializer, DisplayName("Quantity"), Description(""), Category("Emitter"), Browsable(true), ReadOnly(false), DefaultValue(false), Editor(typeof(Delta.Editor.RangeUIEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public Range QuantityRange { get { return _quantityRange; } set { _quantityRange = value; } }
        [ContentSerializer, DisplayName("Lifespan"), Description(""), Category("Emitter"), Browsable(true), ReadOnly(false), DefaultValue(false), Editor(typeof(Delta.Editor.RangeUIEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public Range LifespanRange { get { return _lifespanRange; } set { _lifespanRange = value; } }
        [ContentSerializer, DisplayName("Velocity"), Description(""), Category("Emitter"), Browsable(true), ReadOnly(false), DefaultValue(false), Editor(typeof(Delta.Editor.RangeUIEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public Range VelocityMagnitudeRange { get { return _velocityMagnitudeRange; } set { _velocityMagnitudeRange = value; } }
        [ContentSerializer, DisplayName("Acceleration"), Description(""), Category("Emitter"), Browsable(true), ReadOnly(false), DefaultValue(false), Editor(typeof(Delta.Editor.RangeUIEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public Range AccelerationMagnitudeRange { get { return _accelerationMagnitudeRange; } set { _accelerationMagnitudeRange = value; } }
        [ContentSerializer, DisplayName("Rotation"), Description(""), Category("Emitter"), Browsable(true), ReadOnly(false), DefaultValue(false), Editor(typeof(Delta.Editor.RangeUIEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public Range RotationRange { get { return _rotationRange; } set { _rotationRange = value; } }
        [ContentSerializer, DisplayName("Velocity Angle"), Description(""), Category("Emitter"), Browsable(true), ReadOnly(false), DefaultValue(false), Editor(typeof(Delta.Editor.RangeUIEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public Range VelocityAngleRange { get { return _velocityAngleRange; } set { _velocityAngleRange = value; } }
        [ContentSerializer, DisplayName("Acceleration Angle"), Description(""), Category("Emitter"), Browsable(true), ReadOnly(false), DefaultValue(false), Editor(typeof(Delta.Editor.RangeUIEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public Range AccelerationAngleRange { get { return _accelerationAngleRange; } set { _accelerationAngleRange = value; } }
        [ContentSerializer, DisplayName("Scale"), Description(""), Category("Emitter"), Browsable(true), ReadOnly(false), DefaultValue(false), Editor(typeof(Delta.Editor.RangeUIEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public Range ScaleRange { get { return _scaleRange; } set { _scaleRange = value; } }
        [ContentSerializer, DisplayName("Fade In"), Description(""), Category("Emitter"), Browsable(true), ReadOnly(false), DefaultValue(false), Editor(typeof(Delta.Editor.RangeUIEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public Range FadeInRange { get { return _fadeInRange; } set { _fadeInRange = value; } }
        [ContentSerializer, DisplayName("Fade Out"), Description(""), Category("Emitter"), Browsable(true), ReadOnly(false), DefaultValue(false), Editor(typeof(Delta.Editor.RangeUIEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public Range FadeOutRange { get { return _fadeOutRange; } set { _fadeOutRange = value; } }

        // fuck you for not serializing blendstates; the graphicsdevice isn't setup @ buildtime; warning will hardcrash visual studio
        [ContentSerializerIgnore, DisplayName("Blend"), Browsable(false)]
        public BlendState Blend
        {
            get
            {
                return _blend;
            }
            set
            {
                _blend = value;
            }
        }

        // fuck you for not serializing delegates
        [ContentSerializerIgnore, Browsable(false)]
        public Interpolation.InterpolationMethod FadeInInterpolator
        {
            get
            {
                if (_fadeInInterpolator == null) 
                    _fadeInInterpolator = Interpolation.Parse(_fadeInMethodString); 
                return _fadeInInterpolator;
            }
        }

        // fuck you for not serializing delegates
        [ContentSerializerIgnore, Browsable(false)]
        public Interpolation.InterpolationMethod FadeOutInterpolator
        {
            get
            {
                if (_fadeOutInterpolator == null) 
                    _fadeOutInterpolator = Interpolation.Parse(_fadeOutMethodString); 
                return _fadeOutInterpolator;
            }
        }

        public Emitter()
        {
            _velocityAngleRange = new Range(0, 360);
            _scaleRange = new Range(1);
            _quantityRange = new Range(1, 1);
            _fadeInMethodString = "Linear";
            _fadeOutMethodString = "Linear";
            _blend = BlendState.AlphaBlend;
        }

        protected override void Recycle(bool isReleasing)
        {
            _frequency = 0;
            _explode = false;
            _quantityRange = new Range(1, 1);
            _lifespanRange = Range.Empty;
            _rotationRange = Range.Empty;
            _velocityMagnitudeRange = Range.Empty;
            _velocityAngleRange = Range.Empty;
            _accelerationMagnitudeRange = Range.Empty;
            _accelerationAngleRange = Range.Empty;
            _scaleRange = new Range(1, 1);
            _fadeInRange = Range.Empty;
            _fadeOutRange = Range.Empty;
            _fadeInMethodString = "Linear";
            _fadeOutMethodString = "Linear";
            _blend = BlendState.AlphaBlend;
            base.Recycle(isReleasing);
        }

        protected internal override bool SetValue(string name, string value)
        {
            switch (name)
            {
                case "frequency":
                    _frequency = float.Parse(value, CultureInfo.InvariantCulture);
                    return true;
                case "lifespan":
                    _lifespanRange = Range.TryParse(value);
                    return true;
                case "rotation":
                case "rotationspeed":
                    _rotationRange = Range.TryParse(value);
                    return true;
                case "v":
                case "vel":
                case "velocity":
                    _velocityMagnitudeRange = Range.TryParse(value);
                    return true;
                case "vangle":
                case "velangle":
                case "velocityangle":
                    _velocityAngleRange = Range.TryParse(value);
                    return true;
                case "a":
                case "accel":
                case "acceleration":
                    _accelerationMagnitudeRange = Range.TryParse(value);
                    return true;
                case "aangle":
                case "accelangle":
                case "accelerationangle":
                    _accelerationAngleRange = Range.TryParse(value);
                    return true;
                case "scale":
                case "size":
                    _scaleRange = Range.TryParse(value);
                    return true;
                case "explode":
                    _explode = true;
                    _quantityRange = Range.TryParse(value);
                    return true;
                case "quantity":
                    _quantityRange = Range.TryParse(value);
                    return true;
                case "fadein":
                    _fadeInRange = Range.TryParse(value);
                    return true;
                case "fadeout":
                    _fadeOutRange = Range.TryParse(value);
                    return true;
                case "fadeinmethod":
                    _fadeInMethodString = value;
                    return true;
                case "fadeoutmethod":
                    _fadeOutMethodString = value;
                    return true;
                case "blend":
                    _blend = BlendStateExtensions.Parse(value);
                    return true;
            }
            return base.SetValue(name, value);
        }

        internal class Particle<T> : Poolable where T: TransformableEntity
        {
            public Emitter Emitter;

            public T Entity;

            /// <summary>
            /// The amount of time spent living.
            /// </summary>
            public float Life;

            /// <summary>
            /// The total amount of time to live.
            /// </summary>
            public float Lifespan;

            public Vector2 Acceleration;
            public Vector2 Velocity;
            public float AngularVelocity;

            public float FadeOutPercent;
            public float FadeInPercent;

            public bool IsDead { get { return Life >= Lifespan; } }


            protected override void Recycle(bool isReleasing)
            {
                if (isReleasing)
                    Entity.Recycle();
                Entity = null;
                Life = 0;
                Lifespan = 0;
                Acceleration = Vector2.Zero;
                Velocity = Vector2.Zero;
                AngularVelocity = 0;
                FadeOutPercent = 0;
                FadeInPercent = 0;
                base.Recycle(isReleasing);
            }

            public virtual void Update(DeltaGameTime time)
            {
                if (FadeInPercent > 0)
                    Entity.Alpha = Emitter.FadeInInterpolator(0, 1, Life / (FadeInPercent * Lifespan)); 
                if (FadeOutPercent > 0)
                    Entity.Alpha = Entity.Alpha - Emitter.FadeOutInterpolator(0, 1, (Life - (Lifespan - FadeOutPercent * Lifespan)) / (FadeOutPercent * Lifespan));
            }

            public virtual void Draw(DeltaGameTime time, SpriteBatch spriteBatch) { }
            public virtual void OnEmitted() { }

        }
    }
}
