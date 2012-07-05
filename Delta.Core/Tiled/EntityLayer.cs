using System;

#if WINDOWS
using System.Xml;
using System.Reflection;
using System.Globalization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Delta.Physics.Geometry;
using Delta.Physics;
#endif

namespace Delta.Tiled
{

    public class EntityLayer : EntityParent<IEntity>, ILayer
    {
        public float Parallax { get; set; }

        public EntityLayer()
            : base()
        {
        }

#if WINDOWS
        public EntityLayer(string fileName, XmlNode node)
            : base()
        {
            this.ImportLayer(node);

            foreach (XmlNode objectNode in node.SelectNodes("object"))
            {
                IEntity entity = null;
                if (objectNode.Attributes["type"] != null)
                {
                    string typeName = objectNode.Attributes["type"].Value;
                    foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        Type type = assembly.GetType(typeName, false, true);
                        if (type != null)
                            entity = Activator.CreateInstance(type) as IEntity;
                    }
                }

                if (entity == null)
                    continue;

                // parse all the data from the tiled object.
                String name = objectNode.Attributes["name"] == null ? String.Empty : objectNode.Attributes["name"].Value;
                Vector2 position = new Vector2(
                    objectNode.Attributes["x"] == null ? 0 : float.Parse(objectNode.Attributes["x"].Value, CultureInfo.InvariantCulture),
                    objectNode.Attributes["y"] == null ? 0 : float.Parse(objectNode.Attributes["y"].Value, CultureInfo.InvariantCulture)
                );
                Vector2 size = new Vector2(
                    objectNode.Attributes["width"] == null ? 0 : float.Parse(objectNode.Attributes["width"].Value, CultureInfo.InvariantCulture),
                    objectNode.Attributes["height"] == null ? 0 : float.Parse(objectNode.Attributes["height"].Value, CultureInfo.InvariantCulture)
                );
                
                // the distinction between polygon and polyline is determined by the entity type.
                bool IsPolygon;
                List<Vector2> polyVertices = new List<Vector2>();
                XmlNode polyNode = objectNode["polygon"];
                if (IsPolygon = polyNode == null)
                    polyNode = objectNode["polyline"];
                if (polyNode != null)
                {
                    foreach (string point in polyNode.Attributes["points"].Value.ToString().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string[] split = point.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        if (split.Length == 2)
                            polyVertices.Add(position + new Vector2(float.Parse(split[0], CultureInfo.InvariantCulture), float.Parse(split[1], CultureInfo.InvariantCulture)));
                        else
                            throw new Exception(string.Format("The poly point'{0}' is not the format 'x,y'. Map: {1}", point, fileName));
                    }
                }

                // all entities get a name.
                entity.ID = name;

                // attempt to set entity specific properties.
                TransformableEntity transformableEntity = entity as TransformableEntity;
                if (transformableEntity != null)
                {
                    transformableEntity.ID = position.ToString();
                    transformableEntity.Position = position;
                }

                CollideableEntity collideableEntity = entity as CollideableEntity;
                if (collideableEntity != null)
                {
                    if (size != Vector2.Zero)
                    {
                        collideableEntity.Polygon = new Box()
                        {
                            HalfWidth = (int)size.X / 2,
                            HalfHeight = (int)size.Y / 2,
                        };
                    }
                    else if (IsPolygon)
                    {
                        collideableEntity.Polygon = new Polygon(polyVertices.ToArray());
                        //collideableEntity.Position = position; // side effect due to how polygons are constructed.
                    }
                    else
                    {
                        if (polyVertices[0] == polyVertices[polyVertices.Count - 1])
                            polyVertices.RemoveAt(polyVertices.Count - 1);
                        collideableEntity.Polygon = new Polygon(polyVertices.ToArray());
                        //collideableEntity.Position = position; // side effect due to how polygons are constructed.
                    }
                }

                // populate entity properties with tiled properties or try invoking a method.
                entity.ImportXmlProperties(objectNode["properties"]);

                // tiled doesn't store rotation so we're using a property to set an entity's rotation.
                if (transformableEntity != null)
                    transformableEntity.Rotation = MathHelper.ToRadians(transformableEntity.Rotation);

                Add(entity);
            }
        }
#endif
    }
}