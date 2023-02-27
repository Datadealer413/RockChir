using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RockChoir
{
    public static class Utility
    {
        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
}