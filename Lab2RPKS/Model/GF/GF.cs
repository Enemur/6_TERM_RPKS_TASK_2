using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Lab2RPKS.Model.GF
{
    public class Gf
    {
        private BigInteger _polynom;
        private BigInteger _polynomMax;
        private BigInteger _primitivePolynom;

        private int _size;

        public Gf(int size)
        {
            _polynomMax = (BigInteger)1 << (size - 1);
            _size = size;
            CreatePrimitivePolynom();
        }

        public int Size
        {
            get { return _size; }
            set
            {
                _polynomMax = (BigInteger)1 << (value - 1);
                _size = value;
                CreatePrimitivePolynom();
            }
           
        }
        public string Polynom
        {
            get { return _polynom.ToString(); }
            set
            {

                BigInteger polynomTemp = BigInteger.Parse(value);
                BigInteger max = (_polynomMax << 1) - 1;
                if (polynomTemp > max)
                    throw new Exception($"Данный полином выходит за границу 2^{_size}");
                else
                    _polynom = polynomTemp;

            }
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

        private bool IsPrime(BigInteger n)
        {
         
            for (int a = 2; a < n && a < 11; a++)
            {
            
                bool b = MillerRabinTest(n, a);
                if (!b)
                {
                    return false;
                }
            }
            return true;


        }
        private void CreatePrimitivePolynom()
        {
            _primitivePolynom = BigInteger.Pow(2, _size);
            while (!IsPrime(_primitivePolynom))
            {
                _primitivePolynom++;
            }
        }

        public override string ToString()
        {
            string str = "";
            Console.WriteLine();
            Console.WriteLine();
            for (int i = (_size - 1); i >= 0; i--)
            {

                if ((_polynom & (1 << i)) > 0)
                {
                    if (str.Length > 0)
                        str += " + ";
                    if (i == 0)
                        str += "1";
                    else if (i == 1)
                        str += "x";
                    else
                        str += $"x^{i}";

                }
            }

            if (str.Length == 0)
                str = "0";
            return str;
        }
        public static Gf operator +(Gf gf1, Gf gf2)
        {
            var answer = new Gf(gf1._size);
            answer._polynom = gf1._polynom ^ gf2._polynom;
            return answer;
        }
        public static Gf operator *(Gf gf1, Gf gf2)
        {
            if (gf1._size != gf2._size)
            {
                throw new Exception("Разный размер");
            }

            Gf answer = new Gf(gf1._size);
            answer.Polynom = "0";

            BigInteger a = BigInteger.Parse(gf1.Polynom);
            BigInteger b = BigInteger.Parse(gf2.Polynom);

            while (!a.IsZero && !b.IsZero)
            {
                if ((b & 1) == 1) // если b нечетно, то добавить соответствующий a к p(конечный продукт = сумма всех a, соответствующих нечетным b) 
                    answer._polynom ^= a;  // так как мы находимся в GF(2 ^ m), добавление - это XOR 

                if (a >= gf1._polynomMax) // GF по модулю: если a> = 128, то при смещении влево он будет переполнен, поэтому уменьшите 
                    a = ((a << 1) ^ gf1._primitivePolynom); // *XOR // XOR с примитивным полиномом x ^ 8 + x ^ 4 + x ^ 3 + x + 1(0b1_0001_1011) - вы можете изменить его, но оно должно быть неприводимым 
                else
                    a <<= 1;  // эквивалентно a * 2 
                b >>= 1;  // эквивалентно b // 2 

            }
            return answer;
        }
        public static Gf operator /(Gf gf1, Gf gf2)
        {
            /*
            x*y=z;
            z/x=y;
             */
            if (gf1._size != gf2._size)
            {
                throw new Exception("Разный размер");
            }

            Gf answer = new Gf(gf1._size);
            answer._polynom = 0;
            BigInteger max = (gf1._polynomMax << 1) - 1;
            while (true)
            {
                Gf temp = gf2 * answer;
                if (temp._polynom == gf1._polynom)
                {
                    break;
                }

                answer._polynom++;

                if (answer._polynom > max)
                    throw new Exception("Ошибка при делении");
            }
            return answer;
        }



    }
}
