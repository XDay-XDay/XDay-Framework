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

using XDay.SerializationAPI;
using System.Collections.Generic;

namespace XDay.WorldAPI
{
    public class ResourceDescriptorSystem : ISerializable, IResourceDescriptorSystem
    {
        public virtual string TypeName => "ResourceDescriptorSystem";

        public ResourceDescriptorSystem()
        {
        }

        public void Init(IWorld world)
        {
            m_Descriptors = new(m_UninitedDescriptors.Count);
            foreach (var d in m_UninitedDescriptors)
            {
                m_Descriptors.Add(d.GetPath(0), d);
                d.Init(world);
            }
            m_UninitedDescriptors = null;
        }

        public void Uninit()
        {
            foreach (var kv in m_Descriptors)
            {
                kv.Value.Uninit();
            }
            m_Descriptors.Clear();
        }

        public void GameDeserialize(IDeserializer deserializer, string label)
        {
            deserializer.ReadInt32("ResourceDescriptorSystem.Version");

            m_UninitedDescriptors = deserializer.ReadList("Resource Descriptors", (index) =>
            {
                return deserializer.ReadSerializable<ResourceDescriptor>($"Resource Descriptor {index}", true);
            });
        }

        public IResourceDescriptor QueryDescriptor(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            m_Descriptors.TryGetValue(path, out var descriptor);
            return descriptor;
        }

        protected virtual ResourceDescriptor CreateDescriptor(int id, int index, string path)
        {
            return new ResourceDescriptor(id, index, path);
        }

        protected Dictionary<string, ResourceDescriptor> m_Descriptors = new();
        protected List<ResourceDescriptor> m_UninitedDescriptors = new();
    }
}

//XDay