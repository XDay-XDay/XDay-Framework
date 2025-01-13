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




namespace XDay.WorldAPI.Tile
{
    internal class FlatTileData
    {
        public bool Visible { get => m_Visible; set => m_Visible = value; }

        public FlatTileData(string path)
        {
            m_Path = path;
        }

        public void Init(IResourceDescriptorSystem system)
        {
            m_Descriptor = system.QueryDescriptor(m_Path);
            m_Path = null;
        }

        public string GetPath(int lod)
        {
            return m_Descriptor.GetPath(lod);
        }

        private string m_Path;
        private IResourceDescriptor m_Descriptor;
        private bool m_Visible = false;
    }
}

//XDay