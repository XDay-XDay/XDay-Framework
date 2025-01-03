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

using UnityEngine;
using XDay.CameraAPI;
using XDay.InputAPI;
using XDay.NavigationAPI;
using XDay.UtilityAPI;
using XDay.WorldAPI;

namespace XDay.API
{
    internal class XDayContext : IXDayContext
    {
        public IDeviceInput DeviceInput => m_Input;
        public IWorldManager WorldManager => m_WorldSystem;
        public ITaskSystem TaskSystem => m_TaskSystem;
        public INavigationManager NavigationManager => m_NavigationManager;

        public XDayContext(string worldSetupFilePath, IWorldAssetLoader loader)
        {
            Debug.Assert(!string.IsNullOrEmpty(worldSetupFilePath));
            Debug.Assert(loader != null);

            var createInfo = new TaskSystemCreateInfo();
            createInfo.LayerInfo.Add(new TaskLayerInfo(1, 1));
            m_TaskSystem = ITaskSystem.Create(createInfo);
            m_Input = IDeviceInput.Create();
            m_WorldSystem = new WorldManager();
            m_AssetLoader = loader;

            m_WorldSystem.Init(worldSetupFilePath, m_AssetLoader, m_TaskSystem, m_Input);
            m_NavigationManager = INavigationManager.Create();
        }

        public void OnDestroy()
        {
            m_WorldSystem?.Uninit();
            m_NavigationManager?.OnDestroy();
        }

        public void Update()
        {
            m_Input.Update();
            m_WorldSystem?.Update();
        }

        public void LateUpdate()
        {
            m_WorldSystem?.LateUpdate();
        }

        public ICameraManipulator CreateCameraManipulator(string configPath, Camera camera)
        {
            var text = m_AssetLoader.LoadText(configPath);
            var name = Helper.GetPathName(text, false);
            var setup = new CameraSetup(name);
            setup.Load(text);

            var manipulator = ICameraManipulator.Create(camera, setup, m_Input);
            return manipulator;
        }

        private readonly IWorldAssetLoader m_AssetLoader;
        private readonly WorldManager m_WorldSystem;
        private readonly IDeviceInput m_Input;
        private readonly ITaskSystem m_TaskSystem;
        private readonly INavigationManager m_NavigationManager;
    }
}


//XDay