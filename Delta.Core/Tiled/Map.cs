using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

#if WINDOWS
using System.Xml;
using System.Reflection;
using System.Globalization;
using Polenter.Serialization;
#endif

using Delta.Graphics;
using System.IO.Compression;

namespace Delta.Tiled
{
    public enum MapOrientation : byte
    {
        Orthogonal,
        Isometric,
    }

    public class Map : EntityParent<Entity>
    {
        internal static Map Instance { get; set; }

#if WINDOWS
        public static Dictionary<string, Entity> GlobalObjectStyles = new Dictionary<string, Entity>();

        public static void LoadStyleSheet(string fileName)
        {
            XmlDocument document = new XmlDocument(); document.Load(fileName);
            foreach (XmlNode node in document.DocumentElement.ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Comment)
                    continue;
                string typeName = node.Attributes["Type"] == null ? string.Empty : node.Attributes["Type"].Value;
                if (string.IsNullOrEmpty(typeName))
                    continue;
                Entity entity = LoadObject(typeName);
                if (entity == null)
                    continue;
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    if (!entity.SetValue(childNode.Name.ToLower(), childNode.InnerText))
                        throw new Exception(String.Format("Could not import XML property '{0}', no such property exists for '{1}'.", childNode.Name.ToLower(), entity.GetType().Name));
                }
                if (!GlobalObjectStyles.ContainsKey(node.Name))
                    GlobalObjectStyles.Add(node.Name, entity);
                else
                    GlobalObjectStyles[node.Name] = entity;
            }
        }

        public static Entity LoadObject(string name)
        {
            if (GlobalObjectStyles.ContainsKey(name))
                return GlobalObjectStyles[name].Copy() as Entity;
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = assembly.GetType(name, false, true);
                if (type != null)
                    return Activator.CreateInstance(type, true) as Entity;
            }
            return null;
        }
#endif

        [EditorBrowsable(EditorBrowsableState.Never)]
        [ContentSerializer(FlattenContent = true, ElementName="Tileset")]
        public List<Tileset> _tilesets = new List<Tileset>();
        [EditorBrowsable(EditorBrowsableState.Never)]
        [ContentSerializer(ElementName = "SpriteSheet")]
        public string _spriteSheetName = string.Empty;
        internal SpriteSheet _spriteSheet = null;

        [ContentSerializer]
        public string Version { get; private set; }
        [ContentSerializer]
        public int TileWidth { get; private set; }
        [ContentSerializer]
        public int TileHeight { get; private set; }
        [ContentSerializer]
        public int Width { get; private set; }
        [ContentSerializer]
        public int Height { get; private set; }
        [ContentSerializer]
        public MapOrientation Orientation { get; private set; }
        [ContentSerializer]
        private int BelowGroundIndex;
        [ContentSerializer]
        private int GroundIndex;
        [ContentSerializer]
        private int AboveGroundIndex;

        public Map()
            : base()
        {
            Instance = this;
        }

#if WINDOWS
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Map(string fileName)
            : this()
        {
            //initialize serializer for content
            G.InitializeSharpSerializer();

            XmlDocument document = new XmlDocument(); document.Load(fileName);
            XmlNode node = document["map"];

            Version = node.Attributes["version"].Value;
            TileWidth = int.Parse(node.Attributes["tilewidth"].Value, CultureInfo.InvariantCulture);
            TileHeight = int.Parse(node.Attributes["tileheight"].Value, CultureInfo.InvariantCulture);
            Width = int.Parse(node.Attributes["width"].Value, CultureInfo.InvariantCulture);
            Height = int.Parse(node.Attributes["height"].Value, CultureInfo.InvariantCulture);
            Orientation = (MapOrientation)Enum.Parse(typeof(MapOrientation), node.Attributes["orientation"].Value, true);

            if (Orientation != MapOrientation.Orthogonal)
                throw new NotSupportedException(String.Format("{0} does not have built in support for rendering isometric Tiled maps.", Assembly.GetExecutingAssembly().GetName().Name));

            foreach (XmlNode tilesetNode in node.SelectNodes("tileset"))
                _tilesets.Add(new Tileset(tilesetNode));

            //sort the tilesets by largest GID, this allows proper tile setup.
            _tilesets.Sort((a, b) => (-a.FirstGID.CompareTo(b.FirstGID)));

            string layerName = string.Empty;
            int layerOrder = 0;
            bool layerIsVisible = false;
            foreach (XmlNode layerNode in node.SelectNodes("layer|objectgroup"))
            {
                layerName = layerNode.Attributes["name"].Value;
                layerIsVisible = (node.Attributes["visible"] != null) ? int.Parse(node.Attributes["visible"].Value, CultureInfo.InvariantCulture) == 1 : true;
                switch (layerNode.Name.ToLower())
                {
                    case "layer":
                        if (!layerIsVisible)
                            continue;
                        Add(new TileLayer(fileName, layerNode, layerName) { Name = layerName, OriginalName = layerName, Depth = layerOrder});
                        break;
                    case "objectgroup":
                        EntityLayer entityLayer = new EntityLayer(fileName, layerNode, layerIsVisible) { Name = layerName, OriginalName = layerName, Depth = layerOrder }; 
                        switch (layerName.ToLower())
                        {
                            case "delta.belowground":
                            case "delta.bg":
                            case "d.bg":
                                BelowGroundIndex = layerOrder;
                                break;
                            case "delta.ground":
                            case "delta.g":
                            case "d.g":
                                GroundIndex = layerOrder;
                                break;
                            case "delta.aboveground":
                            case "delta.ag":
                            case "d.ag":
                                AboveGroundIndex = layerOrder;
                                break;
                        }
                        Add(entityLayer);
                        break;
                    default:
                        throw new Exception(String.Format("Unknown layer type '{0}'.", layerNode.Name));
                }
                layerOrder++;
            }
        }

        /// <summary>
        /// saves map information out to tmx file
        /// </summary>
        /// <param name="tmxBase">the existing XML to use as a base</param>
        /// <param name="tmxOutPath">where to save TMX to</param>
        public void SaveToTMX(XmlDocument tmxBase, string tmxOutPath)
        {
            //parse old TMX
            foreach (EntityLayer layer in
                from l in Children
                where l is EntityLayer
                select l as EntityLayer)
            {
                try
                {
                    //select this object layer
                    XmlNode layerNode = tmxBase.SelectSingleNode("map/objectgroup[@name='" + layer.OriginalName + "']");

                    //replace its inner text with serialized objects
                    layerNode.InnerXml = SerializeObjects(layer, G.sharpSerializer);
                }
                catch (Exception e)
                {
                    System.Windows.Forms.MessageBox.Show("Invalid TMX file loaded: " + e.Message);
                    return;
                }
            }

            //finally save to file
            try
            {
                tmxBase.Save(new FileStream(tmxOutPath, FileMode.Create));
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Could not save TMX file: " + e.Message);
            }
        }

        /// <summary>
        /// serialize all objects in layer
        /// </summary>
        /// <param name="layer">the object layer</param>
        /// <param name="ss">the serializer</param>
        /// <returns>a combined XML string of all serialized child objects</returns>
        private string SerializeObjects(EntityLayer layer, SharpSerializer ss)
        {
            string result = "";

            using (var stream = new MemoryStream())
            {
                foreach (var child in layer.Children)
                {
                    ss.Serialize(child, stream);
                    stream.Write(System.Text.Encoding.UTF8.GetBytes("\r\n"), 0, 2);
                }

                //convert to string
                result = System.Text.Encoding.UTF8.GetString(stream.ToArray());
            }

            return result;
        }
#endif

        protected override void LoadContent()
        {
            if (!string.IsNullOrEmpty(_spriteSheetName))
                _spriteSheet = G.Content.Load<SpriteSheet>(_spriteSheetName);
            base.LoadContent();
        }

        protected internal override void OnAdded()
        {
            if (BelowGroundIndex > 0)
                G.World.BelowGround = Children[BelowGroundIndex] as IEntityCollection;
            if (GroundIndex > 0)
                G.World.Ground = Children[GroundIndex] as IEntityCollection;
            if (AboveGroundIndex > 0)
                G.World.AboveGround = Children[AboveGroundIndex] as IEntityCollection;
            base.OnAdded();
        }

        protected internal override void OnRemoved()
        {
            G.World.BelowGround = null;
            G.World.Ground = null;
            G.World.AboveGround = null;
            base.OnRemoved();
        }

        public override string ToString()
        {
            string info = String.Empty;
            foreach (Entity gameComponent in _children)
                info += gameComponent.ToString() + "\n";
            return info;
        }
    }

    internal static class MapHelper
    {
        internal static void ImportTiledProperties(this Entity entity, XmlNode node)
        {
            if (node == null)
                return;
            bool isFound = false;
            foreach (XmlNode propertyNode in node.ChildNodes)
            {
                isFound = false;
                string name = propertyNode.Attributes["name"] == null ? string.Empty : propertyNode.Attributes["name"].Value.ToLower();
                string value = propertyNode.Attributes["value"].Value;
                if (!string.IsNullOrEmpty(name))
                    isFound = entity.SetValue(name, value);
                else
                    continue;
                if (!isFound)
                    throw new Exception(String.Format("Could not import Tiled XML property '{0}', no such property exists for '{1}'.", name, entity.GetType().Name));
            }
        }
    }

}
