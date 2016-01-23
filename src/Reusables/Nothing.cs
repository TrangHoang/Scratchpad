﻿using System;

namespace Reusables
{
    public struct Nothing : IEquatable<Nothing>, IComparable<Nothing>
    {
        public bool Equals(Nothing other)
        {
            return true;
        }

        public int CompareTo(Nothing other)
        {
            return 0;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is Nothing;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public static bool operator ==(Nothing first, Nothing second)
        {
            return true;
        }

        public static bool operator !=(Nothing first, Nothing second)
        {
            return false;
        }
    }
}
