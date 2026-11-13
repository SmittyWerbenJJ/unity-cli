using System;
using System.Collections.Generic;

namespace IlInterpreter
{
    internal static class IlInterpreterCompat
    {
        public static unsafe float Int32BitsToSingle(int value) => *(float*)&value;
        public static unsafe int SingleToInt32Bits(float value) => *(int*)&value;
    }
}

namespace System.Collections.Generic
{
    internal static class KeyValuePairDeconstructExtensions
    {
        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair, out TKey key, out TValue value)
        {
            key = pair.Key;
            value = pair.Value;
        }
    }
}
