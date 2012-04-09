﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using AvalonDock.Layout;

namespace AvalonDock.MVVMTestApp
{
    class DocumentTemplateSelector : DataTemplateSelector
    {
        public DocumentTemplateSelector()
        {
        
        }


        public DataTemplate FileViewTemplate
        {
            get;
            set;
        }

        public DataTemplate FileStatsViewTemplate
        {
            get;
            set;
        }

        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            var itemAsLayoutContent = item as LayoutContent;

            if (item is FileViewModel || 
                (itemAsLayoutContent != null && itemAsLayoutContent.Content is FileViewModel))
                return FileViewTemplate;

            if (item is FileStatsViewModel ||
                (itemAsLayoutContent != null && itemAsLayoutContent.Content is FileStatsViewModel))
                return FileStatsViewTemplate;

            return base.SelectTemplate(item, container);
        }
    }
}
