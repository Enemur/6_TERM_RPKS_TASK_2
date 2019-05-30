using System;
using System.Windows;
using System.Windows.Input;
using Essy.Tools.InputBox;
using Lab2RPKS.Model;

namespace Lab2RPKS.ApplicationViewModel
{
    class ApplicationViewModelGf256:ApplicationViewModel_Encrypthion
    {
        private Gf256 _polynom1;
        private Gf256 _polynom2 ;
        public ApplicationViewModelGf256()
        {
            _polynom1=new Gf256();
            _polynom1.Polynom = 0;

            _polynom2 = new Gf256();
            _polynom2.Polynom = 0;
        }

        private void UpdateTextBoxs()
        {
            OnPropertyChanged($"BinaryPolynomialEquation1");
            OnPropertyChanged($"BinaryPolynomialEquation2");

            OnPropertyChanged($"BinaryPolynomialMultiplicationValue");
            OnPropertyChanged($"BinaryPolynomialMultiplicationEquation");

            OnPropertyChanged($"MultiplicativeInverse1");
            OnPropertyChanged($"MultiplicativeInverse2");
        }

        public String BinaryPolynomialValue1
        {
            get
            {
                if (_polynom1.Polynom == 0) return "";
                return _polynom1.Polynom.ToString();
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _polynom1.Polynom = Convert.ToByte(0);
                    UpdateTextBoxs();
                }
                else
                {
                    try
                    {
                        _polynom1.Polynom = Convert.ToByte(value);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                  
                    UpdateTextBoxs();
                }
            }
        }

        public String BinaryPolynomialValue2
        {
            get
            {
                if (_polynom2.Polynom == 0) return "";
                return _polynom2.Polynom.ToString();
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _polynom2.Polynom = Convert.ToByte(0);
                    UpdateTextBoxs();
                }
                else
                {
                    try
                    {
                        _polynom2.Polynom = Convert.ToByte(value);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                    UpdateTextBoxs();
                }
            }
        }
     

        public String BinaryPolynomialEquation1 { get=>_polynom1.ToString();}
    public String BinaryPolynomialEquation2 { get => _polynom2.ToString(); }
        public String BinaryPolynomialMultiplicationValue
        {
            get {return Gf256.Multiplication(_polynom1, _polynom2).Polynom.ToString(); }
        }

        public String BinaryPolynomialMultiplicationEquation
        {
            get {
                return Gf256.Multiplication(_polynom1, _polynom2).ToString();
            }
        }

        public String MultiplicativeInverse1
        {
            get
            {
                return _polynom1.MultiplicativeInverse().ToString();
            }
        }
        public String MultiplicativeInverse2
        {
            get
            {
                return _polynom2.MultiplicativeInverse().ToString();
            }
        }

        private ICommand _multiplicatian2NumberCommand;
        public ICommand Multiplicatian2NumberCommand
        {
            get
            {
                return _multiplicatian2NumberCommand ?? (_multiplicatian2NumberCommand = new RelayCommand(x =>
                           {
                               string answer=InputBox.ShowInputBox("Введите номера элементов из GF1 и GF2 через пробел (индексация с 0)");
                               if (string.IsNullOrEmpty(answer))
                               {
                                   MessageBox.Show("Введена пустая строка");
                                   return;
                               }

                               var str = answer.Split(' ');
                               try
                               {
                                   answer=Gf256.Multiplicatian2Number(_polynom1, _polynom2, Convert.ToInt32(str[0]),
                                       Convert.ToInt32(str[1])).ToString();
                                   MessageBox.Show(answer);
                               }
                               catch (Exception e)
                               {
                                   MessageBox.Show(e.Message);
                               }
                              
                           }));
            }
        }
    }
          
}
