﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using AvalonDock.Layout;

namespace AvalonDock.Controls
{
    public abstract class LayoutFloatingWindowControl : Window, ILayoutControl
    {
        static LayoutFloatingWindowControl()
        {
            LayoutFloatingWindowControl.ContentProperty.OverrideMetadata(typeof(LayoutFloatingWindowControl), new FrameworkPropertyMetadata(null, null, new CoerceValueCallback(CoerceContentValue)));
            AllowsTransparencyProperty.OverrideMetadata(typeof(LayoutFloatingWindowControl), new FrameworkPropertyMetadata(false));
            ShowInTaskbarProperty.OverrideMetadata(typeof(LayoutFloatingWindowControl), new FrameworkPropertyMetadata(false));
        } 

        static object CoerceContentValue(DependencyObject sender, object content)
        {
            return new FloatingWindowContentHost(sender as LayoutFloatingWindowControl) { Content = content as UIElement };
        }


        protected class FloatingWindowContentHost : HwndHost
        {
            LayoutFloatingWindowControl _owner;
            public FloatingWindowContentHost(LayoutFloatingWindowControl owner)
            {
                _owner = owner;
                var manager = _owner.Model.Root.Manager;
            }


            HwndSource _wpfContentHost = null;
            Border _rootPresenter = null;

            protected override System.Runtime.InteropServices.HandleRef BuildWindowCore(System.Runtime.InteropServices.HandleRef hwndParent)
            {
                _wpfContentHost = new HwndSource(new HwndSourceParameters()
                {
                    ParentWindow = hwndParent.Handle,
                    WindowStyle = Win32Helper.WS_CHILD | Win32Helper.WS_VISIBLE | Win32Helper.WS_CLIPSIBLINGS | Win32Helper.WS_CLIPCHILDREN,
                    Width = 1,
                    Height = 1
                });

                _rootPresenter = new Border() { Child = Content };
                _rootPresenter.SetBinding(Border.BackgroundProperty, new Binding("DataContext.Background"));
                _wpfContentHost.RootVisual = _rootPresenter;
                _wpfContentHost.SizeToContent = SizeToContent.Manual;
                var manager = _owner.Model.Root.Manager;
                manager.InternalAddLogicalChild(_rootPresenter);

                return new HandleRef(this, _wpfContentHost.Handle);
            }

            protected override void DestroyWindowCore(HandleRef hwnd)
            {
                var manager = _owner.Model.Root.Manager;
                manager.InternalRemoveLogicalChild(_rootPresenter);
                Win32Helper.DestroyWindow(hwnd.Handle);
            }

            public Visual RootVisual
            {
                get { return _rootPresenter; }
            }

            protected override Size MeasureOverride(Size constraint)
            {
                if (Content == null)
                    return base.MeasureOverride(constraint);

                Content.Measure(constraint);
                return Content.DesiredSize;
            }

            #region Content

            /// <summary>
            /// Content Dependency Property
            /// </summary>
            public static readonly DependencyProperty ContentProperty =
                DependencyProperty.Register("Content", typeof(UIElement), typeof(FloatingWindowContentHost),
                    new FrameworkPropertyMetadata((UIElement)null,
                        new PropertyChangedCallback(OnContentChanged)));

            /// <summary>
            /// Gets or sets the Content property.  This dependency property 
            /// indicates ....
            /// </summary>
            public UIElement Content
            {
                get { return (UIElement)GetValue(ContentProperty); }
                set { SetValue(ContentProperty, value); }
            }

            /// <summary>
            /// Handles changes to the Content property.
            /// </summary>
            private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                ((FloatingWindowContentHost)d).OnContentChanged(e);
            }

            /// <summary>
            /// Provides derived classes an opportunity to handle changes to the Content property.
            /// </summary>
            protected virtual void OnContentChanged(DependencyPropertyChangedEventArgs e)
            {
                if (_rootPresenter != null)
                    _rootPresenter.Child = Content;
            }

            #endregion
        }

        protected LayoutFloatingWindowControl(ILayoutElement model)
        {
            Model = model;
            this.Loaded += new RoutedEventHandler(OnLoaded);
            this.SizeChanged += new SizeChangedEventHandler(OnSizeChanged);
        }

        void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            //UpdatePositionAndSizeOfPanes();
        }

        protected override void OnClosed(EventArgs e)
        {
            var host = Content as FloatingWindowContentHost;
            host.Dispose();

            _hwndSrc.RemoveHook(_hwndSrcHook);
            _hwndSrc.Dispose();

            Model.Root.Manager.RemoveFloatingWindow(this);

            base.OnClosed(e);
        }

        bool _attachDrag = false;
        internal void AttachDrag()
        {
            _attachDrag = true;
            this.Activated += new EventHandler(OnActivated);
            
        }

        HwndSource _hwndSrc;
        HwndSourceHook _hwndSrcHook;

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= new RoutedEventHandler(OnLoaded);

            _hwndSrc = HwndSource.FromDependencyObject(this) as HwndSource;
            _hwndSrcHook = new HwndSourceHook(FilterMessage);
            _hwndSrc.AddHook(_hwndSrcHook);            

        }


        void OnActivated(object sender, EventArgs e)
        {
            this.Activated -= new EventHandler(OnActivated);

            if (_attachDrag && Mouse.LeftButton == MouseButtonState.Pressed)
            {
                IntPtr windowHandle = new WindowInteropHelper(this).Handle;
                var mousePosition = this.PointToScreenDPI(Mouse.GetPosition(this));
                var clientArea = Win32Helper.GetClientRect(windowHandle);
                var windowArea = Win32Helper.GetWindowRect(windowHandle);

                Left = mousePosition.X - windowArea.Width / 2.0;
                Top = mousePosition.Y - (windowArea.Height - clientArea.Height) / 2.0;
                _attachDrag = false;

                int lParam = ((int)mousePosition.X & (int)0xFFFF) | (((int)mousePosition.Y) << 16);
                Win32Helper.SendMessage(windowHandle, Win32Helper.WM_NCLBUTTONDOWN, Win32Helper.HT_CAPTION, lParam);
            }
        }
        

        public ILayoutElement Model
        {
            get;
            private set;
        }


        #region IsDragging

        /// <summary>
        /// IsDragging Read-Only Dependency Property
        /// </summary>
        private static readonly DependencyPropertyKey IsDraggingPropertyKey
            = DependencyProperty.RegisterReadOnly("IsDragging", typeof(bool), typeof(LayoutFloatingWindowControl),
                new FrameworkPropertyMetadata((bool)false,
                    new PropertyChangedCallback(OnIsDraggingChanged)));

        public static readonly DependencyProperty IsDraggingProperty
            = IsDraggingPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the IsDragging property.  This dependency property 
        /// indicates that this floating window is being dragged.
        /// </summary>
        public bool IsDragging
        {
            get { return (bool)GetValue(IsDraggingProperty); }
        }

        /// <summary>
        /// Provides a secure method for setting the IsDragging property.  
        /// This dependency property indicates that this floating window is being dragged.
        /// </summary>
        /// <param name="value">The new value for the property.</param>
        protected void SetIsDragging(bool value)
        {
            SetValue(IsDraggingPropertyKey, value);
        }

        /// <summary>
        /// Handles changes to the IsDragging property.
        /// </summary>
        private static void OnIsDraggingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutFloatingWindowControl)d).OnIsDraggingChanged(e);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the IsDragging property.
        /// </summary>
        protected virtual void OnIsDraggingChanged(DependencyPropertyChangedEventArgs e)
        {
            Console.WriteLine("IsDragging={0}", e.NewValue);
        }

        #endregion



        DragService _dragService = null;
        Vector _dragOffest;
        Point _dragClickPoint;

        protected override void OnLocationChanged(EventArgs e)
        {
            //UpdatePositionAndSizeOfPanes();

          
            base.OnLocationChanged(e);
        }


        void UpdatePositionAndSizeOfPanes()
        {
            var rootVisual = (Content as FloatingWindowContentHost).RootVisual;
            foreach (var pane in rootVisual.FindVisualChildren<LayoutAnchorablePaneControl>())
            {
                var paneModelAsPositionableElement = pane.Model as ILayoutPositionableElement;
                paneModelAsPositionableElement.FloatingLeft = Left;
                paneModelAsPositionableElement.FloatingTop = Top;
                paneModelAsPositionableElement.FloatingWidth = Width;
                paneModelAsPositionableElement.FloatingHeight = Height;
            }
            foreach (var pane in rootVisual.FindVisualChildren<LayoutDocumentPaneControl>())
            {
                var paneModelAsPositionableElement = pane.Model as ILayoutPositionableElement;
                paneModelAsPositionableElement.FloatingLeft = Left;
                paneModelAsPositionableElement.FloatingTop = Top;
                paneModelAsPositionableElement.FloatingWidth = Width;
                paneModelAsPositionableElement.FloatingHeight = Height;
            }
        }

        private IntPtr FilterMessage(
            IntPtr hwnd,
            int msg,
            IntPtr wParam,
            IntPtr lParam,
            ref bool handled
            )
        {
            handled = false;

            switch (msg)
            {
                case Win32Helper.WM_NCRBUTTONDOWN: //Right button click on title area -> show context menu
                    //if (e.WParam.ToInt32() == HTCAPTION)
                    //{
                    //    short x = (short)((e.LParam.ToInt32() & 0xFFFF));
                    //    short y = (short)((e.LParam.ToInt32() >> 16));
                    //    OpenContextMenu(null, new Point(x, y));
                    //    e.Handled = true;
                    //}
                    break;
                case Win32Helper.WM_NCLBUTTONDOWN: //Left button down on title -> start dragging over docking manager
                    if (wParam.ToInt32() == Win32Helper.HT_CAPTION)
                    {
                        short x = (short)((lParam.ToInt32() & 0xFFFF));
                        short y = (short)((lParam.ToInt32() >> 16));

                        Point clickPoint = this.TransformToDeviceDPI(new Point(x, y));

                        _dragOffest = clickPoint -
                             this.TransformToDeviceDPI(new Point(Left, Top));

                        _dragClickPoint = clickPoint;
                    }
                    break;

                //case Win32Helper.WM_ENTERSIZEMOVE:
                //    {
                //        if (_dragService == null)
                //            _dragService = new DragService(this);
                //        SetIsDragging(true);
                //        _dragService.UpdateMouseLocation(_dragClickPoint.X, _dragClickPoint.Y);
                //    }
                //    break;

                case Win32Helper.WM_EXITSIZEMOVE:
                    if (_dragService != null)
                    {
                        bool dropFlag;
                        _dragService.Drop(Left + _dragOffest.X, Top + _dragOffest.Y, out dropFlag);
                        _dragService = null;
                        SetIsDragging(false);

                        if (dropFlag)
                            InternalClose();
                    }
                    
                    UpdatePositionAndSizeOfPanes();

                    break;
                case Win32Helper.WM_MOVING:
                    {
                        if (_dragService == null)
                        {
                            _dragService = new DragService(this);
                            SetIsDragging(true);
                        }
                        var windowRect = (Win32Helper.RECT)Marshal.PtrToStructure(lParam, typeof(Win32Helper.RECT));
                        _dragService.UpdateMouseLocation(windowRect.Left + _dragOffest.X, windowRect.Top + _dragOffest.Y);
                    }
                    break;
                case Win32Helper.WM_LBUTTONUP: //set as handled right button click on title area (after showing context menu)
                    if (_dragService != null && Mouse.LeftButton == MouseButtonState.Released)
                    {
                        _dragService.Abort();
                        _dragService = null;
                        SetIsDragging(false);
                    }
                    break;
            }

             

            return IntPtr.Zero;
        }

        bool _internalCloseFlag = false;
        internal void InternalClose()
        {
            _internalCloseFlag = true;
            Close();
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
        }
    }
}
