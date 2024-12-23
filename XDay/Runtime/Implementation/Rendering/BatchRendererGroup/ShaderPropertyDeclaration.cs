/*
 * Copyright (c) 2024 XDay
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



using UnityEngine;

namespace XDay.RenderingAPI.BRG
{
    internal class ShaderPropertyDeclaration : IShaderPropertyDeclaration
    {
        public string Name => m_Name;
        public bool IsPerInstance => m_PerInstance;
        public int DataSize => m_DataSize;

        public ShaderPropertyDeclaration(string name, bool perInstance, ShaderPropertyType type)
        {
            m_Name = name;
            m_PerInstance = perInstance;
            m_DataSize = Stride(type);
        }

        private int Stride(ShaderPropertyType type)
        {
            switch (type)
            {
                case ShaderPropertyType.PackedMatrix:
                    return 48;
                case ShaderPropertyType.Vector:
                case ShaderPropertyType.Color:
                    return 16;
                case ShaderPropertyType.Int:
                case ShaderPropertyType.Float:
                    return 4;
                default:
                    Debug.Assert(false);
                    return 0;
            }
        }

        private string m_Name;
        private bool m_PerInstance;
        private int m_DataSize;
    }
}

//XDay