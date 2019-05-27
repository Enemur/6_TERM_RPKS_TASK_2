using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Lab2RPKS.ApplicationViewModel
{
    public abstract class BaseAplicationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        private ICommand _aboutProgramCommand;
        public ICommand AboutProgramCommand =>
            _aboutProgramCommand ??
            (_aboutProgramCommand = new RelayCommand(obj =>
            {
                MessageBox.Show("Лаба 2");
            }));
        private ICommand _aboutAuthorCommand;
        public ICommand AboutAuthorCommand =>
            _aboutAuthorCommand ??
            (_aboutAuthorCommand = new RelayCommand(obj =>
            {
                MessageBox.Show("Малахов Александр Владимирович\n8-Т3О-301Б-16");
            }));
    }
}
