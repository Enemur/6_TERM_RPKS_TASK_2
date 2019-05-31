using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Essy.Tools.InputBox;
using Lab2RPKS.Model.GF;

namespace Lab2RPKS.ApplicationViewModel
{
    public class ApplicationViewModel_GF : ApplicationViewModelGf256
    {
        private Gf _polynom1;
        private Gf _polynom2;

        public ApplicationViewModel_GF()
        {
            _polynom1 = new Gf(1);
            _polynom1.Polynom = "0";

            _polynom2 = new Gf(1);
            _polynom2.Polynom = "0";
        }


        private void UpdateTextBoxs()
        {
            OnPropertyChanged($"Gf1");
            OnPropertyChanged($"Gf2");

            OnPropertyChanged($"GfSize");
            OnPropertyChanged($"ResultAdd");

            OnPropertyChanged($"ResultMult");
            OnPropertyChanged($"ResultDiv");
        }

        public String GfSize
        {
            get { return _polynom1.Size.ToString(); }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                   
                }
                else
                {
                    try
                    {
                        int size = Convert.ToInt32(value);
                        if (size > 0)
                        {

                            _polynom1.Size = size;
                            _polynom2.Size = size;
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }

                    UpdateTextBoxs();
                }
            }
        }

        public String Gf1
        {
            get
            {
                if (_polynom1.Polynom == "0") return "";
                return _polynom1.Polynom;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _polynom1.Polynom = "0";
                    UpdateTextBoxs();
                }
                else
                {
                    try
                    {
                        _polynom1.Polynom = value;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }

                    UpdateTextBoxs();
                }
            }
        }

        public String Gf2
        {
            get
            {
                if (_polynom2.Polynom == "0") return "";
                return _polynom2.Polynom;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _polynom2.Polynom = "0";
                    UpdateTextBoxs();
                }
                else
                {
                    try
                    {
                        _polynom2.Polynom = value;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }

                    UpdateTextBoxs();
                }
            }
        }


        public String ResultAdd
        {
            get => (_polynom1 + _polynom2).Polynom;
        }

        public String ResultMult
        {
            get => (_polynom1 * _polynom2).Polynom;
        }

        public String ResultDiv
        {
            get
            {
                try
                {
                    return (_polynom1 / _polynom2).Polynom;
                }
                catch (Exception e)
                {
                    return "";
                }
            }
        }
    }
}
