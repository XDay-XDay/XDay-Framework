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

namespace XDay.InputAPI
{
    internal class MouseScrollMotion : Motion, IMouseScrollMotion
    {
        public float Delta => m_Delta;
        public Vector2 Position => m_Position;
        public override MotionType Type => MotionType.MouseScroll;

        public MouseScrollMotion(int id, IDeviceInput device)
            : base(id, device) 
        {
        }

        protected override void OnReset()
        {
            m_Delta = 0;
            m_Position = Vector2.zero;
        }

        protected override bool Match()
        {
            if (m_Device.SceneTouchCount != 1)
            {
                Reset(true);
                return false;
            }

            var touch = m_Device.GetSceneTouch(0);
            if (touch.State == TouchState.Start)
            {
                m_Delta = touch.Scroll;
                m_Position = touch.Current;
                return m_Delta != 0;
            }

            return false;
        }

        private float m_Delta;
        private Vector2 m_Position;
    }
}

//XDay