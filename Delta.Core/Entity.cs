using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Delta
{
    /// <summary>
    /// Base class for all game entites.
    /// </summary>
    [DefaultPropertyAttribute("Name")]
    public abstract class Entity : Poolable
    {
        /// <summary>
        /// Retrieves an <see cref="Entity"/> by it's name.
        /// </summary>
        /// <param name="name">The name of the <see cref="Entity"/> to retrieve.</param>
        /// <returns>The <see cref="Entity"/> with the specified name. Returns <see cref="null"/> if an <see cref="Entity"/> with the specified name could not be found.</returns>
        public static Entity Get(string name)
        {
            return EntityHelper.Get(name);
        }

        /// <summary>
        /// Retrieves the global <see cref="List{T}"/> of entities.
        /// </summary>
        public static List<Entity> GlobalEntities { get { return EntityHelper._globalEntityReferences; } }

        bool _flaggedForRemoval = false;

        /// <summary>
        /// Gets the <see cref="IEntityCollection"/> which is responsible for the <see cref="Entity"/>.
        /// </summary>
        [ContentSerializerIgnore]
        protected internal IEntityCollection ParentCollection { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Entity"/> has initialized.
        /// </summary>
        [ContentSerializerIgnore, Browsable(false)]
        protected bool HasInitialized { get; private set; }
        /// <summary>
        /// Gets a value indicating whether the <see cref="Entity"/> has loaded it's content.
        /// </summary>
        [ContentSerializerIgnore, Browsable(false)]
        protected internal bool HasLoadedContent { get; private set; }
        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Entity"/> needs to perform a heavy update.
        /// </summary>
        /// <remarks>Once a heavy update starts, this value is automatically set to false.</remarks>
        [ContentSerializerIgnore, Browsable(false)]
        protected bool NeedsHeavyUpdate { get; set; }

        string _name = string.Empty;
        /// <summary>
        /// Gets the name of the <see cref="Entity"/>.
        /// </summary>
        [ContentSerializer, Description("The name of the game object.\nDefault is null."), Category("General"), DefaultValue("")]
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// original, non-case-transformed name
        /// </summary>
        [ContentSerializer, Description("The name of the game object before case transforming is done to it."), Category("General"), DefaultValue("")]
        public string OriginalName { get; set; }

        bool _isEnabled = true;
        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Entity"/> is updated.
        /// </summary>
        /// <remarks>The default is true with a <see cref="bool"/> value of true.</remarks>
        [ContentSerializer, DisplayName("Enabled"), Description("Indicates whether the game object is updated.\nDefault is true."), Category("General"), DefaultValue(true)]
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnIsEnabledChanged();
                    OnPropertyChanged();
                }
            }
        }

        bool _isVisible = true;
        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Entity"/> is drawn.
        /// </summary>
        /// <remarks>The default is true with a <see cref="bool"/> value of true.</remarks>
        [ContentSerializer, DisplayName("Visible"), Description("Indicates whether the game object is drawn.\nDefault is true."), Category("General"), DefaultValue(true)]
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    OnIsVisibleChanged();
                    OnPropertyChanged();
                }
            }
        }

        float _depth = 0.0f;
        /// <summary>
        /// Gets or sets the depth (update and draw order) of the <see cref="Entity"/> in the <see cref="ParentCollection"/>.
        /// </summary>
        /// <remarks>The default is zero with a <see cref="float"/> value of 0.0f.</remarks>
        [ContentSerializer, Description("The layer depth of the game object.\nDefault is 0."), Category("General"), DefaultValue(0.0f)]
        public float Depth
        {
            get { return _depth; }
            set
            {
                if (_depth != value)
                {
                    _depth = value;
                    if (ParentCollection != null)
                        ParentCollection.NeedsToSort = true;
                    OnPropertyChanged();
                }
            }
        }

        PostEffects _postEffects = PostEffects.None;
        /// <summary>
        /// Gets or sets the <see cref="PostEffects"/> of the <see cref="Entity"/> used when drawing.
        /// </summary>
        [ContentSerializer, Description("The post effects used when drawing the game object.\nDefault is None."), Category("General"), DefaultValue(PostEffects.None)]
        public PostEffects PostEffects
        {
            get { return _postEffects; }
            set
            {
                if (_postEffects != value)
                {
                    _postEffects = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        protected Entity()
            : base()
        {
            IsVisible = true;
            IsEnabled = true;
            NeedsHeavyUpdate = true;
        }

        /// <summary>
        /// Recycles the <see cref="Entity"/> so it may be re-used.
        /// </summary>
        /// <param name="isReleasing">Indicates whether the <see cref="Entity"/> is being released back to the <see cref="Pool"/>.</param>
        protected override void Recycle(bool isReleasing)
        {
            if (isReleasing)
                if (ParentCollection != null)
                    ParentCollection.UnsafeRemove(this);
            ParentCollection = null;
            _flaggedForRemoval = false;
            _isEnabled = true;
            _isVisible = true;
            _depth = 0;
            _postEffects = PostEffects.None;
            Name = string.Empty;
            HasInitialized = false;
            HasLoadedContent = false;
            NeedsHeavyUpdate = false;
            base.Recycle(isReleasing);
        }

#if WINDOWS
        protected internal virtual string GetValue(string name)
        {
            switch (name)
            {
                case "visible":
                case "isvisible":
                    return IsVisible.ToString().ToLower();
                case "enabled":
                case "isenabled":
                    return IsEnabled.ToString().ToLower();
                case "depth":
                case "layer":
                case "order":
                case "draworder":
                case "updateorder":
                    return Depth.ToString(CultureInfo.InvariantCulture);
                case "overlay":
                case "isoverlay":
                    if ((PostEffects & PostEffects.Overlay) != 0)
                        return "true";
                    else
                        return "false";
            }
            return string.Empty;
        }

        protected internal virtual bool SetValue(string name, string value)
        {
            switch (name)
            {
                case "visible":
                case "isvisible":
                    IsVisible = bool.Parse(value);
                    return true;
                case "enabled":
                case "isenabled":
                    IsEnabled = bool.Parse(value);
                    return true;
                case "depth":
                case "layer":
                case "order":
                case "draworder":
                case "updateorder":
                    Depth = float.Parse(value, CultureInfo.InvariantCulture);
                    return true;
                case "overlay":
                case "isoverlay":
                    PostEffects |= PostEffects.Overlay;
                    return true;
             }
            return false;
        }
#endif

        internal void InternalInitliaze()
        {
            if (!HasInitialized)
            {
                Initialize();
                HasInitialized = true;
            }
        }

        /// <summary>
        /// Initializes the <see cref="Entity"/>.
        /// </summary>
        protected virtual void Initialize()
        {
        }

        internal void InternalLoadContent()
        {
            if (!HasLoadedContent && HasInitialized)
            {
                LoadContent();
                HasLoadedContent = true;
            }
        }

        /// <summary>
        /// Loads content handled by the <see cref="Entity"/>.
        /// </summary>
        protected virtual void LoadContent()
        {
        }

        /// <summary>
        /// Completely updates the <see cref="Entity"/>.
        /// </summary>
        /// <param name="time">time</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void InternalUpdate(DeltaGameTime time)
        {
            if (_flaggedForRemoval)
                RemoveImmediately();
            InternalInitliaze();
            InternalLoadContent();
            if (CanUpdate())
            {
                OnBeginUpdate(time);
                LightUpdate(time);
                if (NeedsHeavyUpdate)
                {
                    NeedsHeavyUpdate = false;
                    HeavyUpdate(time);
                }
                OnEndUpdate(time);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Entity"/> is allowed to update.
        /// </summary>
        /// <returns>A value indicating whether the <see cref="Entity"/> is allowed to update.</returns>
        protected virtual bool CanUpdate()
        {
            return IsEnabled;
        }

        /// <summary>
        /// Updates the <see cref="Entity"/>. Override this method to add custom update logic which is executed every frame.
        /// </summary>
        /// <param name="time">time</param>
        protected virtual void LightUpdate(DeltaGameTime time)
        {
        }  

        /// <summary>
        /// Updates the <see cref="Entity"/>. Override this method to add custom update logic which is too expensive to execute every frame.
        /// </summary>
        /// <param name="time">time</param>
        protected internal virtual void HeavyUpdate(DeltaGameTime time)
        {
        }

        /// <summary>
        /// Completely draws the <see cref="Entity"/>.
        /// </summary>
        /// <param name="time">time</param>
        /// <param name="spriteBatch">spriteBatch</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void InternalDraw(DeltaGameTime time, SpriteBatch spriteBatch)
        {
            if (CanDraw())
            {
                OnBeginDraw(time, spriteBatch);
                if (G.CurrentPostEffects == PostEffects)
                    Draw(time, spriteBatch);
                OnEndDraw(time, spriteBatch);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Entity"/> is allowed to draw.
        /// </summary>
        /// <returns>A value indicating whether the <see cref="Entity"/> is allowed to draw.</returns>
        protected virtual bool CanDraw()
        {
            return IsVisible; 
        }

        /// <summary>
        /// Draws the <see cref="Entity"/>. Override to add custom draw logic which is executed every frame.
        /// </summary>
        /// <param name="time">time</param>
        /// <param name="spriteBatch">spriteBatch</param>
        protected virtual void Draw(DeltaGameTime time, SpriteBatch spriteBatch)
        {
        }

        /// <summary>
        /// Called when the <see cref="Entity"/>'s <see cref="IsEnabled"/> has changed.
        /// </summary>
        protected internal virtual void OnIsEnabledChanged()
        {
        } 

        /// <summary>
        /// Called when the <see cref="Entity"/>'s <see cref="IsVisible"/> has changed.
        /// </summary>
        protected internal virtual void OnIsVisibleChanged()
        {
        }

        /// <summary>
        /// Called when the <see cref="Entity"/> is starting to update.
        /// </summary>
        /// <param name="time">time</param>
        protected virtual void OnBeginUpdate(DeltaGameTime time)
        {
        }

        /// <summary>
        /// Called when the <see cref="Entity"/> is finished updating.
        /// </summary>
        /// <param name="time">time</param>
        protected virtual void OnEndUpdate(DeltaGameTime time)
        {
        }

        /// <summary>
        /// Called when the <see cref="Entity"/> is starting to draw.
        /// </summary>
        /// <param name="time">time</param>
        /// <param name="spriteBatch">spriteBatch</param>
        protected virtual void OnBeginDraw(DeltaGameTime time, SpriteBatch spriteBatch)
        {
        }

        /// <summary>
        /// Called when the <see cref="Entity"/> is finished drawing.
        /// </summary>
        /// <param name="time">time</param>
        /// <param name="spriteBatch">spriteBatch</param>
        protected virtual void OnEndDraw(DeltaGameTime time, SpriteBatch spriteBatch)
        {
        }

        /// <summary>
        /// Called when the <see cref="Entity"/> has been added to an <see cref="IEntityCollection"/>.
        /// </summary>
        protected internal virtual void OnAdded()
        {
        }

#if WINDOWS
        protected internal void OnPropertyChanged()
        {
            //tell G that we are waiting for a PropertyGrid refresh. This implementaion prevents PropertyGrid.Refresh() to be called multiple times in short amount of time, potentitally improving performance.
            G._refreshPropertyGrid = true;
            G._refreshPropertyGridTimer = 0.1f;
        }
#endif

        /// <summary>
        /// Called when the <see cref="Entity"/> has been removed from an <see cref="IEntityCollection"/>.
        /// </summary>
        protected internal virtual void OnRemoved()
        {
            _flaggedForRemoval = false;
            ParentCollection = null;
        }

        /// <summary>
        /// Flags the <see cref="Entity"/> to be removed from the <see cref="ParentCollection"/> upon the start of the next frame.
        /// </summary>
        public void RemoveNextFrame()
        {
            _flaggedForRemoval = true;
        }

        /// <summary>
        /// Removes the <see cref="Entity"/> immediately from the <see cref="ParentCollection"/>.
        /// </summary>
        public void RemoveImmediately() //Hopefully this sounds intimidating lol.
        {
            if (ParentCollection != null)
                ParentCollection.UnsafeRemove(this);
        }
    }

}
