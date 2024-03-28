using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Shining_BeautifulGirls
{
    class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnManyChanged(string[] propertyNames)
        {
            foreach (var name in propertyNames)
                OnPropertyChanged(name);
        }
    }
}
