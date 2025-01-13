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
using XDay.UtilityAPI;

namespace XDay.WorldAPI.Tile
{
    internal class FlatTileSystemRenderer
    {
        public GameObject Root => m_Root;

        public FlatTileSystemRenderer(FlatTileSystem tileSystem)
        {
            m_GameObjects = new GameObject[tileSystem.YTileCount * tileSystem.XTileCount];
            m_Root = new GameObject(tileSystem.Name);
            m_Root.transform.SetParent(tileSystem.World.Root.transform, true);
            m_Root.transform.SetPositionAndRotation(tileSystem.Center, tileSystem.Rotation);
            m_TileSystem = tileSystem;
        }

        public void OnDestroy()
        {
            foreach (var obj in m_GameObjects)
            {
                Helper.DestroyUnityObject(obj);
            }
            Helper.DestroyUnityObject(m_Root);
        }

        public void ToggleActivateState(int x, int y, FlatTileData tile, int lod)
        {
            if (tile.Visible)
            {
                ShowTile(x, y, tile, lod);
            }
            else
            {
                HideTile(x, y, tile, lod); 
            }
        }

        private void HideTile(int x, int y, FlatTileData tile, int lod)
        {
            var idx = y * m_TileSystem.XTileCount + x;
            if (m_GameObjects[idx] == null)
            {
                return;
            }

            m_TileSystem.World.GameObjectPool.Release(tile.GetPath(lod), m_GameObjects[idx]);
            m_GameObjects[idx] = null;
        }

        private void ShowTile(int x, int y, FlatTileData tile, int lod)
        {
            var idx = y * m_TileSystem.XTileCount + x;
            if (m_GameObjects[idx] != null)
            {
                return;
            }

            var gameObject = m_TileSystem.World.GameObjectPool.Get(tile.GetPath(lod));
            gameObject.transform.position = m_TileSystem.CoordinateToLocalPosition(x, y);
            gameObject.transform.SetParent(m_Root.transform, false);
            m_GameObjects[idx] = gameObject;
        }

        private readonly GameObject m_Root;
        private readonly GameObject[] m_GameObjects;
        private readonly FlatTileSystem m_TileSystem;
    };
}

//XDay