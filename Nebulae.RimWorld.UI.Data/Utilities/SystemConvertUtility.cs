// The MIT License (MIT)

// Copyright (c) .NET Foundation and Contributors

// All rights reserved.

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;

namespace Nebulae.RimWorld.UI.Data.Utilities
{
    internal static class SystemConvertUtility
    {
        // list of types supported by System.Convert (from the SDK)
        internal static readonly Type[] SupportedTypes = {
            typeof(string),                 // put common types up front
            typeof(int),    typeof(long),   typeof(float),  typeof(double),
            typeof(decimal),typeof(bool),
            typeof(byte),   typeof(short),
            typeof(uint),   typeof(ulong),  typeof(ushort), typeof(sbyte),  // non-CLS compliant types
        };

        /// <summary>
        /// <see cref="Convert"/> 支持转换的 <see cref="char"/> 类型
        /// </summary>
        internal static readonly Type[] CharSupportedTypes = {
            typeof(string),                 // put common types up front
            typeof(int),    typeof(long),   typeof(byte),   typeof(short),
            typeof(uint),   typeof(ulong),  typeof(ushort), typeof(sbyte),  // non-CLS compliant types
        };


        internal static bool CanConvert(Type sourceType, Type targetType)
        {
            // DateTime can only be converted to and from String type
            if (sourceType == typeof(DateTime))
                return targetType == typeof(string);
            if (targetType == typeof(DateTime))
                return sourceType == typeof(string);

            // Char can only be converted to a subset of supported types
            if (sourceType == typeof(char))
                return CanConvertChar(targetType);
            if (targetType == typeof(char))
                return CanConvertChar(sourceType);

            // Using nested loops is up to 40% more efficient than using one loop
            for (int i = 0; i < SupportedTypes.Length; ++i)
            {
                if (sourceType == SupportedTypes[i])
                {
                    ++i;    // assuming (sourceType != targetType), start at next type
                    for (; i < SupportedTypes.Length; ++i)
                    {
                        if (targetType == SupportedTypes[i])
                            return true;
                    }
                }
                else if (targetType == SupportedTypes[i])
                {
                    ++i;    // assuming (sourceType != targetType), start at next type
                    for (; i < SupportedTypes.Length; ++i)
                    {
                        if (sourceType == SupportedTypes[i])
                            return true;
                    }
                }
            }

            return false;
        }

        private static bool CanConvertChar(Type type)
        {
            for (int i = 0; i < CharSupportedTypes.Length; ++i)
            {
                if (type == CharSupportedTypes[i])
                    return true;
            }
            return false;
        }
    }
}
