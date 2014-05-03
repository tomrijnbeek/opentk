﻿#region License
//
// NativeWindowBase.cs
//
// Author:
//       Stefanos A. <stapostol@gmail.com>
//
// Copyright (c) 2006-2014 Stefanos Apostolopoulos
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
#endregion

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using OpenTK.Input;

namespace OpenTK.Platform
{
    // Common base class for all INativeWindow implementations
    abstract class NativeWindowBase : INativeWindow
    {
        readonly LegacyInputDriver LegacyInputDriver;

        readonly MouseButtonEventArgs MouseDownArgs = new MouseButtonEventArgs();
        readonly MouseButtonEventArgs MouseUpArgs = new MouseButtonEventArgs();
        readonly MouseMoveEventArgs MouseMoveArgs = new MouseMoveEventArgs();
        readonly MouseWheelEventArgs MouseWheelArgs = new MouseWheelEventArgs();

        protected readonly KeyboardKeyEventArgs KeyDownArgs = new KeyboardKeyEventArgs();
        protected readonly KeyboardKeyEventArgs KeyUpArgs = new KeyboardKeyEventArgs();
        protected readonly KeyPressEventArgs KeyPressArgs = new KeyPressEventArgs((char)0);

        // In order to simplify mouse event implementation,
        // we can store the current mouse state here.
        protected MouseState MouseState = new MouseState();
        MouseState PreviousMouseState = new MouseState();

        internal NativeWindowBase()
        {
            LegacyInputDriver = new LegacyInputDriver(this);
            MouseState.SetIsConnected(true);
            PreviousMouseState.SetIsConnected(true);
        }

        #region Protected Members

        protected void OnMove(EventArgs e)
        {
            Move(this, e);
        }

        protected void OnResize(EventArgs e)
        {
            Resize(this, e);
        }

        protected void OnClosing(CancelEventArgs e)
        {
            Closing(this, e);
        }

        protected void OnClosed(EventArgs e)
        {
            Closed(this, e);
        }

        protected void OnDisposed(EventArgs e)
        {
            Disposed(this, e);
        }

        protected void OnIconChanged(EventArgs e)
        {
            IconChanged(this, e);
        }

        protected void OnTitleChanged(EventArgs e)
        {
            TitleChanged(this, e);
        }

        protected void OnVisibleChanged(EventArgs e)
        {
            VisibleChanged(this, e);
        }

        protected void OnFocusedChanged(EventArgs e)
        {
            FocusedChanged(this, e);
        }

        protected void OnWindowBorderChanged(EventArgs e)
        {
            WindowBorderChanged(this, e);
        }

        protected void OnWindowStateChanged(EventArgs e)
        {
            WindowStateChanged(this, e);
        }

        protected void OnKeyDown(KeyboardKeyEventArgs e)
        {
            KeyDown(this, e);
        }

        protected void OnKeyPress(KeyPressEventArgs e)
        {
            KeyPress(this, e);
        }

        protected void OnKeyUp(KeyboardKeyEventArgs e)
        {
            KeyUp(this, e);
        }

        protected void OnMouseLeave(EventArgs e)
        {
            MouseLeave(this, e);
        }

        protected void OnMouseEnter(EventArgs e)
        {
            MouseEnter(this, e);
        }

        protected void OnMouseDown()
        {
            var e = MouseDownArgs;
            e.Mouse = MouseState;

            // Find which button caused this event
            for (MouseButton b = MouseButton.Left; b < MouseButton.LastButton; b++)
            {
                if (!PreviousMouseState[b] && MouseState[b])
                {
                    e.Button = b;
                    PreviousMouseState = MouseState;
                    MouseDown(this, e);
                    return;
                }
            }

            Debug.WriteLine("OnMouseDown called without pressing a button");
        }

        protected void OnMouseUp()
        {
            var e = MouseUpArgs;
            e.Mouse = MouseState;

            // Find which button caused this event
            for (MouseButton b = MouseButton.Left; b < MouseButton.LastButton; b++)
            {
                if (PreviousMouseState[b] && !MouseState[b])
                {
                    e.Button = b;
                    PreviousMouseState = MouseState;
                    MouseUp(this, e);
                    return;
                }
            }

            Debug.WriteLine("OnMouseUp called without pressing a button");
        }

        protected void OnMouseMove()
        {
            var e = MouseMoveArgs;
            e.Mouse = MouseState;
            e.XDelta = MouseState.X - PreviousMouseState.X;
            e.YDelta = MouseState.Y - PreviousMouseState.Y;

            if (e.XDelta == 0 && e.YDelta == 0)
            {
                Debug.WriteLine("OnMouseMove called without moving the mouse");
            }

            PreviousMouseState = MouseState;
            MouseMove(this, e);
        }

        protected void OnMouseWheel()
        {
            var e = MouseWheelArgs;
            e.Mouse = MouseState;
            e.ValuePrecise = MouseState.Scroll.Y;
            e.DeltaPrecise = MouseState.Scroll.Y - PreviousMouseState.Scroll.Y;

            if (e.DeltaPrecise == 0)
            {
                Debug.WriteLine("OnMouseWheel called without moving the mouse wheel.");
            }

            PreviousMouseState = MouseState;
            MouseWheel(this, e);
        }

        #endregion

        #region INativeWindow Members

        public event EventHandler<EventArgs> Move = delegate { };
        public event EventHandler<EventArgs> Resize = delegate { };
        public event EventHandler<System.ComponentModel.CancelEventArgs> Closing = delegate { };
        public event EventHandler<EventArgs> Closed = delegate { };
        public event EventHandler<EventArgs> Disposed = delegate { };
        public event EventHandler<EventArgs> IconChanged = delegate { };
        public event EventHandler<EventArgs> TitleChanged = delegate { };
        public event EventHandler<EventArgs> VisibleChanged = delegate { };
        public event EventHandler<EventArgs> FocusedChanged = delegate { };
        public event EventHandler<EventArgs> WindowBorderChanged = delegate { };
        public event EventHandler<EventArgs> WindowStateChanged = delegate { };
        public event EventHandler<KeyboardKeyEventArgs> KeyDown = delegate { };
        public event EventHandler<KeyPressEventArgs> KeyPress = delegate { };
        public event EventHandler<KeyboardKeyEventArgs> KeyUp = delegate { };
        public event EventHandler<EventArgs> MouseLeave = delegate { };
        public event EventHandler<EventArgs> MouseEnter = delegate { };
        public event EventHandler<MouseButtonEventArgs> MouseDown = delegate { };
        public event EventHandler<MouseButtonEventArgs> MouseUp = delegate { };
        public event EventHandler<MouseMoveEventArgs> MouseMove = delegate { };
        public event EventHandler<MouseWheelEventArgs> MouseWheel = delegate { };

        public abstract void Close();

        public abstract void ProcessEvents();

        public abstract Point PointToClient(Point point);

        public abstract Point PointToScreen(Point point);

        public abstract Icon Icon { get; set; }

        public abstract string Title { get; set; }

        public abstract bool Focused { get; }

        public abstract bool Visible { get; set; }

        public abstract bool Exists { get; }

        public abstract IWindowInfo WindowInfo { get; }

        public abstract WindowState WindowState { get; set; }

        public abstract WindowBorder WindowBorder { get; set; }

        public abstract Rectangle Bounds { get; set; }

        public virtual Point Location
        {
            get
            {
                return Bounds.Location;
            }
            set
            {
                Bounds = new Rectangle(value, Bounds.Size);
            }
        }

        public virtual Size Size
        {
            get
            {
                return Bounds.Size;
            }
            set
            {
                Bounds = new Rectangle(Bounds.Location, value);
            }
        }

        public int X
        {
            get
            {
                return Bounds.X;
            }
            set
            {
                Rectangle old = Bounds;
                Bounds = new Rectangle(value, old.Y, old.Width, old.Height);
            }
        }

        public int Y
        {
            get
            {
                return Bounds.Y;
            }
            set
            {
                Rectangle old = Bounds;
                Bounds = new Rectangle(old.X, value, old.Width, old.Height);
            }
        }

        public int Width
        {
            get
            {
                return ClientSize.Width;
            }
            set
            {
                Rectangle old = ClientRectangle;
                ClientRectangle = new Rectangle(old.X, old.Y, value, old.Height);
            }
        }

        public int Height
        {
            get
            {
                return ClientSize.Height;
            }
            set
            {
                Rectangle old = ClientRectangle;
                Bounds = new Rectangle(old.X, old.Y, old.Width, value);
            }
        }

        public Rectangle ClientRectangle
        {
            get
            {
                return new Rectangle(Point.Empty, ClientSize);
            }
            set
            {
                ClientSize = value.Size;
            }
        }

        public abstract Size ClientSize { get; set; }

        public virtual IInputDriver InputDriver
        {
            get
            {
                return LegacyInputDriver;
            }
        }

        public abstract bool CursorVisible { get; set; }

        public abstract MouseCursor Cursor { get; set; }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected abstract void Dispose(bool disposing);

        ~NativeWindowBase()
        {
            Debug.Print("NativeWindowBase leaked, did you forget to call Dispose()?");
            Dispose(false);
        }

        #endregion
    }
}

