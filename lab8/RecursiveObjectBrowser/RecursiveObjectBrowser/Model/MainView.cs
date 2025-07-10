using System.Collections.ObjectModel;
using RecursiveObjectBrowser.Model;

namespace RecursiveObjectBrowser
{
    public class MainView
    {
        public ObservableCollection<Worker> Children { get; set; } = new ObservableCollection<Worker>();

        public MainView()
        {
        }
    }
}