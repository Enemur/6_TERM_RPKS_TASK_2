using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Numerics;
using System.Security.Cryptography;

namespace Lab2RPKS.Model.EncryptionAlgorithm
{
    public class RSA : EncryptionAlgorithm
    {
        public RSA(ref int currentProgress, BackgroundWorker worker, Action<string> onPropertyChanged) : base(
            ref currentProgress, worker, onPropertyChanged)
        {
        }


        public string Encode(string inputFileName, string outputFileName, BigInteger p, BigInteger q)//возвращает секретные ключ в виде 2 чисел
        {
            if (!IsTheNumberSimple(p) || !IsTheNumberSimple(q))
            {
                throw new Exception("p или q - не простые числа!");
            }

            BigInteger n = p * q;
            BigInteger m = (p - 1) * (q - 1);
            BigInteger d = Calculate_d(m);
            BigInteger e_ = Calculate_e(d, m);
            using (FileStream fsread = new FileStream(inputFileName, FileMode.Open, FileAccess.Read))
            {
                using (StreamWriter fswrite = new StreamWriter(outputFileName))
                {


                    var OneTick = fsread.Length / 100;
                    if (fsread.Length == 0) throw new Exception("Пустой файл");

                    fswrite.WriteLine(fsread.Length);


                    for (long i = 0; i < fsread.Length; i++)
                    {
                        if (_worker != null)
                            if (i % OneTick == 0 && i != 0)
                            {
                                _worker.ReportProgress(_currentProgress);
                                Thread.Sleep(1);
                                _currentProgress = _currentProgress + 1;
                                _onPropertyChanged("CurrentProgress");

                            }

                        int readByte = fsread.ReadByte();

                        BigInteger result = RSAByteEncryption((uint)readByte, d, n);

                        fswrite.WriteLine(result);
                    }
                }
            }
            return $"{d} {n}";


        }

        public void Decipher(string inputFileName, string outputFileName, BigInteger d, BigInteger n)
        {


            using (StreamReader fsread = new StreamReader(inputFileName))
            {
                using (FileStream fswrite = new FileStream(outputFileName, FileMode.Create, FileAccess.Write))
                {



                    long length = Convert.ToInt64(fsread.ReadLine());
                    var OneTick = length / 100;

                    if (length == 0)
                        throw new Exception("Пустой файл");
                    for (long i = 0; i < length; i++)
                    {
                        uint readByte = Convert.ToUInt32(fsread.ReadLine());


                        uint result = (uint)RSAByteEncryption(readByte, d, n);

                        fswrite.WriteByte(BitConverter.GetBytes(result)[0]);

                        if (_worker != null)
                            if (i % OneTick == 0 && i != 0)
                            {
                                _worker.ReportProgress(_currentProgress);
                                Thread.Sleep(1);
                                _currentProgress = _currentProgress + 1;
                                _onPropertyChanged("CurrentProgress");

                            }
                    }



                }
            }




        }

        private BigInteger RSAByteEncryption(uint data, BigInteger d, BigInteger n)
        {
            string result = "";

            BigInteger bi;


            bi = new BigInteger(data);
            bi = BigInteger.Pow(bi, (int)d);

            BigInteger n_ = new BigInteger((int)n);

            bi = bi % n_;
            if (bi > n_)
            {
                throw new Exception("Введите числа больше");
            }
            return bi;

        }

        public bool MillerRabinTest(BigInteger n, int k)
        {
            // если n == 2 или n == 3 - эти числа простые, возвращаем true
            if (n == 2 || n == 3)
                return true;

            // если n < 2 или n четное - возвращаем false
            if (n < 2 || n % 2 == 0)
                return false;

            // представим n − 1 в виде (2^s)·t, где t нечётно, это можно сделать последовательным делением n - 1 на 2
            BigInteger t = n - 1;

            int s = 0;

            while (t % 2 == 0)
            {
                t /= 2;
                s += 1;
            }

            // повторить k раз
            for (int i = 0; i < k; i++)
            {
                // выберем случайное целое число a в отрезке [2, n − 2]
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

                byte[] _a = new byte[n.ToByteArray().LongLength];

                BigInteger a;

                do
                {
                    rng.GetBytes(_a);
                    a = new BigInteger(_a);
                }
                while (a < 2 || a >= n - 2);

                // x ← a^t mod n, вычислим с помощью возведения в степень по модулю
                BigInteger x = BigInteger.ModPow(a, t, n);

                // если x == 1 или x == n − 1, то перейти на следующую итерацию цикла
                if (x == 1 || x == n - 1)
                    continue;

                // повторить s − 1 раз
                for (int r = 1; r < s; r++)
                {
                    // x ← x^2 mod n
                    x = BigInteger.ModPow(x, 2, n);

                    // если x == 1, то вернуть "составное"
                    if (x == 1)
                        return false;

                    // если x == n − 1, то перейти на следующую итерацию внешнего цикла
                    if (x == n - 1)
                        break;
                }

                if (x != n - 1)
                    return false;
            }

            // вернуть "вероятно простое"
            return true;
        }

        private bool IsTheNumberSimple(BigInteger n)
        {
            // ПРОВЕРКА НЕ НА 2х ОСНОВАНИЯХ 'a', а на 10! Т.е. чем больше оснований - тем точнее проверка числа на простоту long[] massA = new long [11];
            // Разные основания 'а' от 2 до 10
            for (int a = 2; a < n && a < 11; a++)
            {
                // Запускаем тест
                bool b = MillerRabinTest(n, a);// Передаем число и проверяем
                if (!b)
                {
                    return false;
                }
            }
            return true;

           
        }


        private BigInteger Calculate_d(BigInteger m)
        {
            BigInteger d = m - 1;

            while (true)
            {
                bool Nod = NOD(m, d)>1;
                if (Nod)
                {
                    d--;
                }
                else
                {
                    break;
                }
                
            }
           

            return d;
        }
        private static BigInteger NOD(BigInteger a, BigInteger b)
        {
            BigInteger nod = 1;
            BigInteger tmp;
            if (a == 0)
                return b;
            if (b == 0)
                return a;
            if (a == b)
                return a;
            if (a == 1 || b == 1)
                return 1;
            while (a != 0 && b != 0)
            {
                if (((a & 1) | (b & 1)) == 0)
                {
                    nod <<= 1;
                    a >>= 1;
                    b >>= 1;
                    continue;
                }
                if (((a & 1) == 0) && (b & 1)>0)
                {
                    a >>= 1;
                    continue;
                }
                if ((a & 1)>0 && ((b & 1) == 0))
                {
                    b >>= 1;
                    continue;
                }
                if (a > b)
                {
                    tmp = a;
                    a = b;
                    b = tmp;
                }
                tmp = a;
                a = (b - a) >> 1;
                b = tmp;
            }
            if (a == 0)
                return nod * b;
            else
                return nod * a;
        }

        private BigInteger Calculate_e(BigInteger d, BigInteger m)
        {
            BigInteger e = 10;

            while (true)
            {
                if ((e * d) % m == 1)
                    break;
                else
                    e++;
            }

            return e;
        }






    }
}
