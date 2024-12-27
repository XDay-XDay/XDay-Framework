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

using Cysharp.Threading.Tasks;
using XDay.SerializationAPI;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace XDay.WorldAPI
{
    public abstract class WorldPlugin : WorldObject, IWorldPlugin
    {
        public abstract string Name { get; set; }
        public virtual bool Inited { get => m_Inited; set => m_Inited = value; }
        public virtual IPluginLODSystem LODSystem => null;
        public abstract List<string> GameFileNames { get; }
        public virtual WorldPluginUsage Usage { get; } = WorldPluginUsage.BothInEditorAndGame;
        public string FileName => Name.Replace(" ", "_");
        public virtual Bounds Bounds => throw new NotImplementedException();

        public WorldPlugin()
        {
        }

        public WorldPlugin(int id, int index) 
            : base(id, index)
        {
        }

        public sealed override void Init(IWorld world)
        {
            PreInit(world);

            InitInternal();

            PostInit(world);
        }

        public async UniTask InitAsync(IWorld world, CancellationToken token)
        {
            PreInit(world);

            await InitAsyncInternal(token);

            PostInit(world);
        }

        public sealed override void Uninit()
        {
            UninitInternal();

            base.Uninit();

            m_Inited = false;
        }

        public override void GameDeserialize(IDeserializer deserializer, string mark)
        {
            deserializer.ReadInt32("WorldPlugin.Version");

            Debug.Assert(false, "WorldPlugin import: todo");
        }

        public void LoadGameData(World world, string pluginName)
        {
            m_ID = world.AllocateObjectID();
            m_Index = world.PluginCount;

            LoadGameDataInternal(world, pluginName);
        }

        public void Update()
        {
            if (m_Inited)
            {
                UpdateInternal();
            }
        }

        internal void InitRenderer()
        {
            InitRendererInternal();
        }

        internal void UninitRenderer()
        {
            UninitRendererInternal();
        }

        private void PreInit(IWorld world)
        {
            base.Init(world);
        }

        private void PostInit(IWorld world)
        {
            m_Inited = true;

            PostInitInternal();
        }

        protected virtual void LoadGameDataInternal(IWorld world, string pluginName)
        {
            throw new NotImplementedException();
        }
        protected abstract void InitInternal();
        protected virtual void PostInitInternal() { }
        protected abstract void UninitInternal();
        protected virtual void InitRendererInternal() { }
        protected virtual void UninitRendererInternal() { }
        protected virtual void UpdateInternal() { }
        protected virtual async UniTask InitAsyncInternal(CancellationToken token)
        {
            InitInternal();
            await UniTask.CompletedTask;
        }

        private bool m_Inited = false;
    }

    public class WorldPluginInfo
    {
        public WorldPluginInfo(Type pluginType, string editorFileName, string displayName, bool isSingleton, Type createWindowType)
        {
            Debug.Assert(createWindowType != null, "Invalid create window type!");

            PluginType = pluginType;
            EditorFileName = editorFileName;
            DisplayName = displayName;
            IsSingleton = isSingleton;
            PluginCreateWindowType = createWindowType;
        }

        public string EditorFileName { get; }
        public string DisplayName { get; }
        public Type PluginCreateWindowType { get; }
        public Type PluginType { get; }
        public bool IsSingleton { get; }
    }
}

//XDay