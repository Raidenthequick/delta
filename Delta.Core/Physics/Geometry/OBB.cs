﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Delta.Physics.Geometry
{
    /// <summary>
    /// An Orientated Bounding Box. ie. it will reflect rotations.
    /// </summary>
    public class OBB : Box
    {
        public object Tag;

        /// <summary>
        /// The Unit vector that points along the x-axis.
        /// </summary>
        public Vector2 Facing
        {
            get
            {
                return new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));
            }
        }

        public Vector2 PerpFacing
        {
            get
            {
                return Vector2Extensions.PerpendicularLeft(Facing);
            }
        }

        public Vector2 HalfSize
        {
            get
            {
                return new Vector2(Width / 2, Height / 2);
            }
        }

        public Vector2 HalfWidthX
        {
            get
            {
                return new Vector2(HalfSize.X, HalfSize.X) * Facing;
            }
        }

        public Vector2 HalfWidthY
        {
            get
            {
                return new Vector2(HalfSize.Y, HalfSize.Y) * Vector2Extensions.PerpendicularLeft(Facing);
            }
        }

        public OBB(float width, float height) : base()
        {
            Width = width;
            Height = height;

            // define the vertices: top-left, top-right, bottom-right, bottom left.
            LocalVertices = new Vector2[] {
                new Vector2(width / 2, height / 2),
                new Vector2(-width / 2, height / 2),
                new Vector2(-width / 2, -height / 2),
                new Vector2(width / 2, -height / 2),
            };
        }

        public void ProjectOntoAxis(Vector2 axisNormal, out Vector2 projection)
        {
            projection.X = Vector2.Dot(HalfWidthX, axisNormal);
            projection.Y = Vector2.Dot(HalfWidthY, axisNormal);
        }

    }
}
