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

using Cysharp.Threading.Tasks;
using XDay.CameraAPI;
using XDay.UtilityAPI;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XDay.SerializationAPI;

namespace XDay.WorldAPI
{
    public enum WorldObjectVisibility
    {
        Undefined,
        Invisible,
        Visible,
    }

    public enum WorldPluginUsage
    {
        BothInEditorAndGame,
        OnlyGenerateData,
        OnlyInEditor,
    }

    public interface IWorldObject : ISerializable
    {
        int ID { get; }
        int WorldID { get; }
        int ObjectIndex { get; }
        IWorld World { get; }
    }

    public interface IWorldPlugin
    {
        int ID { get; }
        string Name { get; }

        void Update();
    }

    public interface IWorld
    {
        int ID { get; }
        string Name { get; }
        float Width { get; }
        float Height { get; }
        Bounds Bounds { get; }
        int PluginCount { get; }
        string GameFolder { get; }
        string EditorFolder { get; }
        IWorldAssetLoader AssetLoader { get; }
        ICameraManipulator CameraManipulator { get; set; }
        GameObject Root { get; }
        IGameObjectPool GameObjectPool { get; }
        ICameraVisibleAreaCalculator CameraVisibleAreaCalculator { get; }
        IWorldLODSystem WorldLODSystem { get; }

        int AllocateObjectID();
        T QueryObject<T>(int objectID) where T : class, IWorldObject;
        T QueryPlugin<T>() where T : class, IWorldPlugin;
        T QueryPlugin<T>(string name) where T : class, IWorldPlugin;
        List<T> QueryPlugins<T>() where T : class, IWorldPlugin;
        IDeserializer QueryGameDataDeserializer(int worldID, string gameDataFileName);
    }

    public interface IWorldManager
    {
        IWorld FirstWorld { get; }
        IWorldAssetLoader WorldAssetLoader { get; }

        UniTask<IWorld> LoadWorldAsync(string name, Camera camera = null);
        IWorld LoadWorld(string name, Camera camera = null);
        void UnloadWorld(string name);
        void LoadWorldRenderer(string name);
        void UnloadWorldRenderer(string name);
        IWorld QueryWorld(int worldID);
    }

    public interface IWorldAssetLoader
    {
        void Uninit();
        T Load<T>(string path) where T : UnityEngine.Object;
        GameObject LoadGameObject(string path);
        byte[] LoadBytes(string path);
        string LoadText(string path);
        Stream LoadTextStream(string path);
        bool Exists(string path);
    }

    public interface ICameraVisibleAreaCalculator
    {
        Rect VisibleArea { get; }
        Rect ExpandedArea { get; }

        void Update(Camera camera);
        void DebugDraw();
    }

    public interface ICameraVisibleAreaUpdater
    {
        static ICameraVisibleAreaUpdater Create(ICameraVisibleAreaCalculator calculator)
        {
            return new CameraVisibleAreaUpdater(calculator);
        }

        void Reset();
        bool BeginUpdate();
        void EndUpdate();
    }

    public interface IResourceDescriptor : IWorldObject
    {
        int LODCount { get; }

        void Init(IWorld world);
        void Uninit();
        string GetPath(int lod);
        int QueryLODGroup(int lod);
    }

    public interface IResourceDescriptorSystem : ISerializable
    {
        IResourceDescriptor QueryDescriptor(string prefabPath);
    }

    public interface IWorldObjectContainer
    {
        int ID { get; }

        void AddObjectUndo(IWorldObject obj, int lod, int objectIndex);
        void DestroyObjectUndo(int objectID);
        IWorldObject QueryObjectUndo(int objectID);
    }

    public interface IDecorationSystem : IWorldPlugin
    {
        void PlayAnimation(int decorationID, string animationName, bool alwaysPlay = false);
        void QueryDecorationIDsInCircle(Vector3 center, float radius, List<int> decorationIDs);
        void ShowDecoration(int decorationID, bool show);
    }
}


//XDay