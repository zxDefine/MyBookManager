using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace MyBookManager
{
    /// <summary>
    /// Item的背景色可以交替设置不一样的颜色
    /// </summary>
    public sealed class AlternatingRowListView : ListView
    {
        public static readonly DependencyProperty EvenColorProperty = 
            DependencyProperty.Register("EvenColor", typeof(SolidColorBrush), typeof(AlternatingRowListView), new PropertyMetadata(0));
        public SolidColorBrush EvenColor
        {
            get { return (SolidColorBrush)GetValue(EvenColorProperty); }
            set { SetValue(EvenColorProperty, value); }
        }

        public static readonly DependencyProperty OddColorProperty =
            DependencyProperty.Register("OddColor", typeof(SolidColorBrush), typeof(AlternatingRowListView), new PropertyMetadata(0));
        public SolidColorBrush OddColor
        {
            get { return (SolidColorBrush) GetValue(OddColorProperty); }
            set { SetValue (OddColorProperty, value); }
        }

        public AlternatingRowListView()
        {
            this.DefaultStyleKey = typeof(ListView);
            this.Items.VectorChanged += OnItemsVectorChanged;
        }

        private void OnItemsVectorChanged(IObservableVector<object> sender, IVectorChangedEventArgs args)
        {
            if(args.CollectionChange == CollectionChange.ItemRemoved)
            {
                var removedItemIndex = (int)args.Index;
                for( var i = removedItemIndex; i < this.Items.Count; i++)
                {
                    if(ContainerFromIndex(i) is ListViewItem listViewItem)
                    {
                        listViewItem.Background = i % 2 == 0 ? EvenColor : OddColor;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            if(element is ListViewItem listViewItem)
            {
                var i = IndexFromContainer(element);
                listViewItem.Background = i % 2 == 0 ? EvenColor : OddColor;
            }
        }
    }
}
