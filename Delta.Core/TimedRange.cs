using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;
using Microsoft.Xna.Framework.Content;
using Delta.Design;

namespace Delta
{
    /// <summary>
    /// Defines a timed range; starting at the lower bound reaching the upper bound when the duration has full elapsed.
    /// </summary>
    [TypeConverter(typeof(TimedRangeConverter))]
    public struct TimedRange : IEquatable<TimedRange>
    {

        /// <summary>
        /// Pull the lower, upper and duration values from strings in the format: range(lower, upper, duration).
        /// </summary>
        static Regex _timedRangeRegex = new Regex(@"range\(\s*(?<value1>(\d*\.?\d+))\s*,\s*(?<value2>(\d*\.?\d+))\s*,\s*(?<duration>(\d*\.?\d+)\s*)\)");

        public float Lower;
        public float Upper;
        public float Duration;

        [ContentSerializerIgnore]
        public static TimedRange Empty = new TimedRange();

        public TimedRange(float scalar)
        {
            Lower = scalar;
            Upper = scalar;
            Duration = 0.0f;
        }

        public TimedRange(float lower, float upper)
        {
            Lower = lower;
            Upper = upper;
            Duration = 0.0f;
        }

        public TimedRange(float lower, float upper, float duration)
        {
            Lower = lower;
            Upper = upper;
            Duration = duration;
        }

        public override bool Equals(object obj)
        {
            if (obj is TimedRange)
                return Equals((TimedRange)obj);
            return base.Equals(obj);
        }

        public bool Equals(TimedRange other)
        {
            return (Lower == other.Lower && Upper == other.Upper && Duration == other.Duration);
        }

        public static bool operator ==(TimedRange x, TimedRange y)
        {
            if (Object.ReferenceEquals(x, null))
            {
                if (Object.ReferenceEquals(y, null))
                    return true;
                return false;
            }
            return x.Equals(y);
        }

        public static bool operator !=(TimedRange x, TimedRange y)
        {
            return !(x == y);
        }

        public static TimedRange Parse(string value)
        {
            TimedRange range = Empty;
            value = value.Trim().ToLower();  // don't worry about matching whitespace inside of the string.
            Match match = _timedRangeRegex.Match(value);
            if (match.Success)
            {
                range.Lower = float.Parse(match.Groups["value1"].Value, CultureInfo.InvariantCulture);
                range.Upper = float.Parse(match.Groups["value2"].Value, CultureInfo.InvariantCulture);
                range.Duration = float.Parse(match.Groups["duration"].Value, CultureInfo.InvariantCulture);
            }
            return range;
        }

        public static explicit operator Range(TimedRange timedRange)
        {
            return new Range(timedRange.Lower, timedRange.Upper);
        }

        public bool IsEmpty()
        {
            return Lower == default(float) && Upper == default(float) && Duration == default(float);
        }

        public override string ToString()
        {
            return String.Format("{0}:{1}:{2}", Lower, Upper, Duration);
        }

    }
}
