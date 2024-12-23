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

#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using XDay.API;
using XDay.CameraAPI;
using XDay.UtilityAPI;

namespace XDay.WorldAPI
{
    internal class WorldPreview : MonoBehaviour
    {
        public Camera Camera;
        public float MoveOffset = 10;

        private async void Start()
        {
            Application.runInBackground = true;

            m_XDay = IXDayContext.Create(EditorHelper.QueryAssetFilePath<WorldSetupManager>(), new EditorWorldAssetLoader());

            await m_XDay.WorldSystem.LoadWorldAsync("");
        }

        private void OnDestroy()
        {
            m_XDay.OnDestroy();
        }

        private void Update()
        {
            m_XDay.Update();

            MoveCamera();
        }

        private void LateUpdate()
        {
            m_XDay.LateUpdate();
        }

        private void OnGUI()
        {
            if (m_Style == null)
            {
                m_Style = new GUIStyle(GUI.skin.label);
                m_Style.fontSize = 50;
            }

            
            if (m_XDay.WorldSystem != null)
            {
                var world = m_XDay.WorldSystem.FirstWorld;
                foreach (var plugin in world.QueryPlugins<WorldPlugin>())
                {
                    if (plugin.LODSystem != null)
                    {
                        GUILayout.Label($"{plugin.Name} LOD: {plugin.LODSystem.CurrentLOD}", m_Style);
                    }
                }
            }
        }

        private void MoveCamera()
        {
            if (m_XDay.WorldSystem == null)
            {
                return;
            }

            var manipulator = m_XDay.WorldSystem.FirstWorld.CameraManipulator;

            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (Input.GetKeyDown(KeyCode.A))
                {
                    manipulator.SetPosition(manipulator.RenderPosition + new Vector3(-MoveOffset, 0, 0));
                }
                if (Input.GetKeyDown(KeyCode.D))
                {
                    manipulator.SetPosition(manipulator.RenderPosition + new Vector3(MoveOffset, 0, 0));
                }
                if (Input.GetKeyDown(KeyCode.W))
                {
                    manipulator.SetPosition(manipulator.RenderPosition + new Vector3(0, 0, MoveOffset));
                }
                if (Input.GetKeyDown(KeyCode.S))
                {
                    manipulator.SetPosition(manipulator.RenderPosition + new Vector3(0, 0, -MoveOffset));
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                var param = new FocusParam(new Vector3(50, 0, 100), 30);
                manipulator.Focus(param);
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                manipulator.Shake(2, 10, 0.2f, true);
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                manipulator.SetPosition(new Vector3(110, 30, 120));
            }
        }

        [MenuItem("XDay/World/Preview", false, 1)]
        static void Open()
        {
            var sceneGUIDs = AssetDatabase.FindAssets("t:Scene");
            if (sceneGUIDs.Length > 0)
            {
                foreach (var guid in sceneGUIDs)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    var name = Path.GetFileName(path);
                    if (name == "WorldPreview.unity")
                    {
                        EditorSceneManager.OpenScene(path);
                    }
                }
            }
        }

        
        private GUIStyle m_Style;
        private IXDayContext m_XDay;
    }
}


#endif