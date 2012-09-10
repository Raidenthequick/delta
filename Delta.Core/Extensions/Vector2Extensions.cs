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
using System.Diagnostics;
using System.Globalization;

namespace Delta
{
    public static class Vector2Extensions
    {

        public static void SafeNormalize(ref Vector2 v)
        {
            // if (v.LengthSquared() < EPSILONSQR) a more safe check.
            if (v != Vector2.Zero)
                Vector2.Normalize(ref v, out v);
            Debug.Assert(v.X.IsValid() && v.Y.IsValid());
        }
        
        public static void SafeNormalizeFast(ref Vector2 v)
        {
            float length = 1 / (v.Length() + float.MinValue);
            v.X = v.X * length;
            v.Y = v.Y * length;
        }

        public static float AngleBetween(this Vector2 a, Vector2 b)
        {
            SafeNormalize(ref a);
            SafeNormalize(ref b);
            return (float) Math.Cos(Vector2.Dot(a, b));
        }

        /// <summary>
        /// Rotate the vector.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="radians">Angle in radians.</param>
        /// <returns></returns>
        public static Vector2 Rotate(this Vector2 v, float radians)
        {
            // early exit if theta is 0 or PI ?
            Vector2 result = Vector2.Zero;
            float cos = (float)Math.Cos(radians);
            float sin = (float)Math.Sin(radians);
            result.X = v.X * cos - v.Y * sin;
            result.Y = v.X * sin + v.Y * cos;
            return result;
        }

        public static Vector2 RandomDirection()
        {
            Vector2 result = default(Vector2);
            result.X = (float)Math.Cos(G.Random.Next() * MathHelper.TwoPi);
            result.Y = (float)Math.Sin(G.Random.Next() * MathHelper.TwoPi);
            return result;
        }

        public static Vector2 DirectionBetween(float startAngle, float finishAngle)
        {
            float theta = G.Random.Between(startAngle, finishAngle);
            Vector2 result = default(Vector2);
            result.X = (float)Math.Cos(theta);
            result.Y = -(float)Math.Sin(theta); // adjust for xna
            return result;
        }

        public static Vector2 PerpendicularLeft(Vector2 v)
        {
            Vector2 result = Vector2.Zero;
            result.X = -v.Y;
            result.Y = v.X;
            return result;
        }

        public static Vector2 PerpendicularRight(Vector2 v)
        {
            Vector2 result = Vector2.Zero;
            result.X = v.Y;
            result.Y = -v.X;
            return result;
        }

        public static Vector2 ProjectOnto(Vector2 a, Vector2 b)
        {
            Vector2 result = Vector2.Zero;
            SafeNormalize(ref b);
            result = Vector2.Dot(a, b) * b; 
            return result;
        }

        public static bool AlmostZero(Vector2 a)
        {
            return AlmostEqual(a, Vector2.Zero);
        }

        public static bool AlmostEqual(ref Vector2 a, ref Vector2 b)
        {
            return AlmostEqual(a, b);
        }

        public static bool AlmostEqual(Vector2 a, Vector2 b)
        {
            return (FloatExtensions.AlmostEqual(a.X, b.X, (float)MathExtensions.EPSILON) && FloatExtensions.AlmostEqual(a.Y, b.Y, (float)MathExtensions.EPSILON));
        }

        public static bool WithinDistanceQuick(Vector2 a, Vector2 b, float targetDistanceSqr)
        {
            float distanceSqr = Vector2.DistanceSquared(a, b);
            return distanceSqr < targetDistanceSqr;
        }

        public static bool WithinDistance(Vector2 a, Vector2 b, float targetDistance)
        {
            float distanceSqr = Vector2.Distance(a, b);
            return distanceSqr < targetDistance;
        }

        public static Point ToPoint(this Vector2 v)
        {
            Point p = Point.Zero;
            p.X = (int) v.X;
            p.Y = (int) v.Y;
            return p;
        }

        public static float CrossProduct(Vector2 a, Vector2 b)
        {
            return a.X * b.Y - b.X * a.Y;
        }

        public static bool IsParallel(Vector2 a, Vector2 b)
        {
            return CrossProduct(a, b) < MathExtensions.EPSILON;
        }

        public static Vector2 Parse(string value, IFormatProvider provider)
        {
            string[] split = value.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length >= 2)
            {
                return new Vector2(
                    float.Parse(split[0], provider),
                    float.Parse(split[1], provider)
                    );
            }
            else
                return new Vector2(
                    float.Parse(split[0], provider),
                    float.Parse(split[0], provider)
                    );
        }

        public static Vector2 Parse(string value)
        {
            return Parse(value, CultureInfo.InvariantCulture);
        }

        public static string ToString(Vector2 v)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(v.X);
            sb.Append(",");
            sb.Append(v.Y);
            return sb.ToString();
        }

        public static string ToString(Vector3 v)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(v.X);
            sb.Append(",");
            sb.Append(v.Y);
            sb.Append(",");
            sb.Append(v.Z);
            return sb.ToString();
        }
    }
}
