/*
 * Copyright (c) 2024-2025 XDay
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to
 * permit persons to whom the Software is furnished to do so, subject to
 * the following conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
 * CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections.Generic;

namespace XDay.SerializationAPI.Editor
{
    internal enum FieldType
    {
        POD,
        Compound,
    }

    internal enum ContainerType
    {
        None,
        List,
        Array,
    }

    internal class SerializableClassFieldInfo
    {
        public string Name;
        public string Label;
        public int ExistedMinVersion;
        public FieldType FieldType;
        public Type Type;
        public ContainerType ContainerType;
    }

    internal class SerializableClassInfo
    {
        public string ClassName { get; set; }
        public string ClassLabel { get; set; }
        public string NamespaceName { get; set; }
        public string Modifier { get; set; }
        public string FilePath { get; set; }
        public bool HasParentClass { get; set; }
        public bool HasPostLoad { get; set; }
        public List<SerializableClassFieldInfo> Fields { get; set; } = new();
    }
}
