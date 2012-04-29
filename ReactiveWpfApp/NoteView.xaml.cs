using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ReactiveWpfApp
{
    public partial class NoteView : UserControl
    {
        public NoteView()
        {
            InitializeComponent();
        }

        protected NoteViewModel Model { get { return (NoteViewModel) DataContext; } }

        private void OnNewTag(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Key == Key.Return)
            {
                Model.AddNewTag(newTagInput.Text);
                newTagInput.Text = "";
            }
        }
    }
}
