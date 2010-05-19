//Copyright (c) 2007-2010, Adolfo Marinucci
//All rights reserved.

//Redistribution and use in source and binary forms, with or without modification, 
//are permitted provided that the following conditions are met:
//
//* Redistributions of source code must retain the above copyright notice, 
//  this list of conditions and the following disclaimer.
//* Redistributions in binary form must reproduce the above copyright notice, 
//  this list of conditions and the following disclaimer in the documentation 
//  and/or other materials provided with the distribution.
//* Neither the name of Adolfo Marinucci nor the names of its contributors may 
//  be used to endorse or promote products derived from this software without 
//  specific prior written permission.
//
//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
//AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
//IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, 
//INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, 
//PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
//HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
//OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, 
//EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 


using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;
using System.ComponentModel;

namespace AvalonDock
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class DocumentTabPanel : PaneTabPanel
    {
        public static bool GetIsHeaderVisible(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsHeaderVisibleProperty);
        }

        public static void SetIsHeaderVisible(DependencyObject obj, bool value)
        {
            obj.SetValue(IsHeaderVisibleProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsHeaderVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsHeaderVisibleProperty =
            DependencyProperty.RegisterAttached("IsHeaderVisible", typeof(bool), typeof(DocumentTabPanel), new UIPropertyMetadata(false));


        protected override Size MeasureOverride(Size availableSize)
        {
            Size desideredSize = new Size();
            int i = 1;

            foreach (ManagedContent child in Children)
            {
                Panel.SetZIndex(child, Selector.GetIsSelected(child)?1:-i);
                i++;
                child.Width = double.NaN;
                child.Height = double.NaN;
                child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                desideredSize.Width += child.DesiredSize.Width;
                desideredSize.Height = Math.Max(desideredSize.Height, child.DesiredSize.Height);
            }

            return new Size(Math.Min(desideredSize.Width, availableSize.Width), desideredSize.Height);
            //return base.MeasureOverride(availableSize);
            //return desideredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double offset = Padding.Left;
            bool skipAllOthers = false;
            foreach (ManagedContent doc in Children)
            {
                if (skipAllOthers || offset + doc.DesiredSize.Width > finalSize.Width)
                {
                    SetIsHeaderVisible(doc, false);
                    doc.Arrange(new Rect());
                    skipAllOthers = true;
                }
                else
                {
                    SetIsHeaderVisible(doc, true);
                    doc.Arrange(new Rect(offset, Padding.Top, doc.DesiredSize.Width, finalSize.Height));
                    offset += doc.ActualWidth;
                }
            }

            return finalSize;

        }

        #region Padding

        /// <summary>
        /// Padding Dependency Property
        /// </summary>
        public static readonly DependencyProperty PaddingProperty =
            DependencyProperty.Register("Padding", typeof(Thickness), typeof(DocumentTabPanel),
                new FrameworkPropertyMetadata((Thickness)new Thickness(),
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets the Padding property.  This dependency property 
        /// indicates internal padding of the panel.
        /// </summary>
        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        #endregion


    }
}