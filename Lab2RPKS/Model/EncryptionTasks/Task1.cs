using System;
using System.Collections.Generic;
using System.Numerics;

namespace Lab2RPKS.Model.EncryptionTasks
{
    public static class CryptoAlgorithmsTask1
    {
        #region Additional types
        
        public struct BezoutResult
        {
            public BigInteger X { get; set; }
            public BigInteger Y { get; set; }
            public BigInteger GCD { get; set; }
        }

        public struct Monom
        {
            public BigInteger Coefficient { get; set; }
            public BigInteger Degree { get; set; }
        }

        #endregion

        #region Aliases

        // 1. Напишите программу, выводящую все простые числа, которые меньше m
        public static List<BigInteger> Task1(BigInteger m)
        {
            return getSimpleNumberLessThan(m);
        }

        // 2. Выведите на экран приведенную систему вычетов по модулю m
        public static List<BigInteger> Task2(BigInteger m)
        {
            return getReducedSystemOfResiduesByModule(m);
        }

        // 3. Напишите функцию, вычисляющую значение f(m), где f(m) − функция Эйлера.
        public static BigInteger Task3(BigInteger m)
        {
            return getEuler(m);
        }

        // 4. Напишите программу, представляющую число M в каноническом разложении по степеням простых чисел
        public static List<Monom> Task4(BigInteger m)
        {
            return GetCanonicalExpansionInPowersOfPrimes(m);
        }

        #endregion

        #region Public methods

        // Расширенный алгоритм Евклида - соотношение Безу
        public static BezoutResult GetBezout(BigInteger a, BigInteger b)
        {
            // An implementation of extended Euclidean algorithm.
            // Returns integer x, y and gcd(a, b) for Bezout equation:
            // ax + by = gcd(a, b).
            // gcd - Наибольший общий делитель

            var x = new BigInteger(1);
            var xx = new BigInteger(0);
            var y = new BigInteger(0);
            var yy = new BigInteger(1);

            while (b > 0)
            {
                var q = a / b;

                var tmpA = b;
                var tmpB = a % b;

                a = tmpA;
                b = tmpB;

                var tmpX = xx;
                var tmpXx = x - xx * q;
                
                x = tmpX;
                xx = tmpXx;

                var tmpY = yy;
                var tmpYy = y - yy * q;

                y = tmpY;
                yy = tmpYy;
            }

            return new BezoutResult
            {
                X = x,
                Y = y,
                GCD = a,
            };
        }

        // Наибольший общий делитель
        public static BigInteger GetGCD(BigInteger a, BigInteger b)
        {
            var bezout = GetBezout(a, b);
            return bezout.GCD;
        }

        // Быстрое возведение в степень по модулю
        public static BigInteger PowModule(BigInteger x, BigInteger n, BigInteger module)
        {
            var result = new BigInteger(1);

            while (n > 0)
            {
                if (n % 2 != 0)
                {
                    result *= n;
                    result %= module;
                    n -= 1;
                }
                else
                {
                    x *= x;
                    x %= module;
                    n /= 2;
                }
            }

            return result;
        }
        
        // Проверяет, простое число или нет
        public static bool IsSimple(BigInteger n)
        {
            if (n >= 1 && n <= 3)
            {
                return true;
            }

            var maxNum = getNumberMoreThanSqrt(n);

            for (var i = new BigInteger(2); i < maxNum; i++)
            {
                if (n % i == 0)
                {
                    return false;
                }
            }

            return n > 1;
        }

        // Проверяет, взаимно простые ли числа
        public static bool isMutuallySimple(BigInteger a, BigInteger b)
        {
            // Натуральные числа a и b называют взаимно простыми, если их наибольший общий делитель равен 1
            var gcd = GetGCD(a, b);
            return gcd == 1;
        }

        // Все простые числа, которые меньше m
        public static List<BigInteger> getSimpleNumberLessThan(BigInteger m)
        {
            var result = new List<BigInteger>();

            for (var num = new BigInteger(1); num < m; num++)
            {
                if (IsSimple(num))
                {
                    result.Add(num);
                }
            }

            return result;
        }

        // Приведенная система вычетов по модулю m
        public static List<BigInteger> getReducedSystemOfResiduesByModule(BigInteger m)
        {
            // Определение. Числа a_1,a_2, ..., a_k образуют приведенную систему вычетов по модулю m,
            // если они взаимно просты с m и любое целое число, взаимно простое с m,
            // сравнимо с одним и только одним из этих чисел по модулю m.
            // 
            // Пример. Приведенная система вычетов по модулю 10: 1,3,7,9.

            var result = new List<BigInteger>
            {
                new BigInteger(1),
            };

            if (m == 1)
            {
                return result;
            }

            for (var num = new BigInteger(2); num < m; num++)
            {
                if (isMutuallySimple(num, m))
                {
                    result.Add(num);
                }
            }

            return result;
        }

        // функция, вычисляющая значение функции Эйлера
        public static BigInteger getEuler(BigInteger m)
        {
            // Функция, равная количеству натуральных чисел, меньших m и взаимно простых с ним
            // При это f(1) = 1

            if (m == 1)
            {
                return new BigInteger(1);
            }

            var result = new BigInteger(1);

            for (var num = new BigInteger(2); num < m; num++)
            {
                if (isMutuallySimple(num, m))
                {
                    result += 1;
                }
            }

            return result;
        }
        
        // получает следующее простое число, больше переданного
        public static BigInteger GetNextPrimeNumber(BigInteger m)
        {
            do
            {
                m++;
            } while (!IsSimple(m));

            return m;
        }

        // TODO: потестить
        // получает каноническое разложение числа по степеням простых чисел
        public static List<Monom> GetCanonicalExpansionInPowersOfPrimes(BigInteger m)
        {
            if (m <= 0)
            {
                throw new ArgumentException("m = 0");
            }

            var coefficientToDegree = new Dictionary<BigInteger, BigInteger>();

            var currentPrime = new BigInteger(2);

            while (!IsSimple(m))
            {
                var divideResult = m / currentPrime;

                if (divideResult * currentPrime != m)
                {
                    currentPrime = GetNextPrimeNumber(currentPrime);
                }
                else
                {
                    if (coefficientToDegree.ContainsKey(currentPrime))
                    {
                        coefficientToDegree[currentPrime]++;
                    }
                    else
                    {
                        coefficientToDegree.Add(currentPrime, 1);
                    }
                    
                    m = divideResult;
                }
            }

            if (coefficientToDegree.ContainsKey(m))
            {
                coefficientToDegree[m]++;
            }
            else
            {
                coefficientToDegree.Add(m, 1);
            }

            var result = new List<Monom>();

            foreach (var keyValuePair in coefficientToDegree)
            {
                result.Add(new Monom
                {
                    Coefficient = keyValuePair.Key,
                    Degree = keyValuePair.Value,
                });
            }

            return result;
        }

        #endregion

        #region Private methods

        // получает число чуть больше чем квадратный корень
        private static BigInteger getNumberMoreThanSqrt(BigInteger x)
        {
            var result = x;
            if (result <= 0)
            {
                return new BigInteger(1);
            }

            while (result * result >= x)
            {
                result /= 2;
            }

            return (result * 2) + 1;
        }

        #endregion
    }
}
