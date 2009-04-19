﻿#pragma checksum "..\..\Window2.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "A7D12E2D93E385B2E4ABB14A9D6BC9C5"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3053
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using AvalonDock;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace AvalonDockTest {
    
    
    /// <summary>
    /// Window2
    /// </summary>
    public partial class Window2 : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 7 "..\..\Window2.xaml"
        internal System.Windows.Controls.DockPanel dockPanel1;
        
        #line default
        #line hidden
        
        
        #line 8 "..\..\Window2.xaml"
        internal System.Windows.Controls.Menu MainMenu;
        
        #line default
        #line hidden
        
        
        #line 9 "..\..\Window2.xaml"
        internal System.Windows.Controls.MenuItem MainMenuFileItem;
        
        #line default
        #line hidden
        
        
        #line 11 "..\..\Window2.xaml"
        internal AvalonDock.DockingManager mainDockingManager;
        
        #line default
        #line hidden
        
        
        #line 12 "..\..\Window2.xaml"
        internal AvalonDock.ResizingPanel verticalResizingPanel;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\Window2.xaml"
        internal AvalonDock.ResizingPanel horizontalResizingPanel;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\Window2.xaml"
        internal AvalonDock.DockablePane LeftDockablePane;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\Window2.xaml"
        internal AvalonDock.DocumentPane CenterDocumentPane;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\Window2.xaml"
        internal AvalonDock.DockablePane BottomDockablePane;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/AvalonDockTest;component/window2.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\Window2.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.dockPanel1 = ((System.Windows.Controls.DockPanel)(target));
            return;
            case 2:
            this.MainMenu = ((System.Windows.Controls.Menu)(target));
            return;
            case 3:
            this.MainMenuFileItem = ((System.Windows.Controls.MenuItem)(target));
            return;
            case 4:
            this.mainDockingManager = ((AvalonDock.DockingManager)(target));
            return;
            case 5:
            this.verticalResizingPanel = ((AvalonDock.ResizingPanel)(target));
            return;
            case 6:
            this.horizontalResizingPanel = ((AvalonDock.ResizingPanel)(target));
            return;
            case 7:
            this.LeftDockablePane = ((AvalonDock.DockablePane)(target));
            return;
            case 8:
            this.CenterDocumentPane = ((AvalonDock.DocumentPane)(target));
            return;
            case 9:
            this.BottomDockablePane = ((AvalonDock.DockablePane)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
