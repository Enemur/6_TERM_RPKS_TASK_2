using System;

namespace Lab2RPKS.Model
{
    public class Gf256
    {
        private uint _polynom;

        public uint Polynom
        {
            get { return _polynom; }
            set { _polynom = value; }
        }

        public override string ToString()
        {
            string str = "";
            Console.WriteLine();
            Console.WriteLine();
            for (int i = 7; i >=0; i--)
            {
             
                if ((_polynom&(1<<i))>0)
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
        public static Gf256 Multiplication(Gf256 gf256_1, Gf256 gf256_2)
        {
            Gf256 answer=new Gf256();
            answer.Polynom = 0;
            uint a = gf256_1.Polynom;
            uint b = gf256_2.Polynom;
            while (a!=0 && b!=0)
            {
                if ((b & 1)==1) // *если b нечетно, то добавить соответствующий a к p(конечный продукт = сумма всех a, соответствующих нечетным b) * /
                    answer.Polynom ^= a;  // *так как мы находимся в GF(2 ^ m), добавление - это XOR * /

                if (a >=128 ) // *GF по модулю: если a> = 128, то при смещении влево он будет переполнен, поэтому уменьшите * /
                    a = (byte)((a << 1) ^  0x11b); // *XOR с примитивным полиномом x ^ 8 + x ^ 4 + x ^ 3 + x + 1(0b1_0001_1011) - вы можете изменить его, но оно должно быть неприводимым * /
                else 
                    a <<= 1;  // *эквивалентно a * 2 * /
                b >>= 1;  // *эквивалентно b // 2 * / 

            }
            return answer;
        }

        public Gf256 MultiplicativeInverse()
        {
            return Pow(this, 254);
        }
        private Gf256 Pow(Gf256 a, int n)
        {
            // Быстрое возведение в степень.
            if (n == 0)
            {
                Gf256 temp=new Gf256();
                temp.Polynom = 1;
                return temp;
            }
            else if (n % 2 == 0)
            {
                return Pow(Multiplication(a, a), n / 2); // (a*a)^(n/2)
            }
            else
            {
                Gf256 square = Multiplication(a, a);
                return Multiplication(Pow(square, n / 2), a); // a * (a*a)^[n/2] 
            }
        }
        public static Gf256 Multiplicatian2Number(Gf256 gf256_1, Gf256 gf256_2,int number1,int number2)
        {
            Gf256 a=new Gf256();
            Gf256 b=new Gf256();
            a.Polynom = (gf256_1.Polynom & ((uint)1 << number1));
            b.Polynom = (gf256_2.Polynom & ((uint)1 << number2));
            return Multiplication(a, b);
        }

    }
}
