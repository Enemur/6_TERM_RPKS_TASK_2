using System.Numerics;

namespace Lab2RPKS.Model.EncryptionTasks
{
    public static class Task3
    {
        #region Aliases

        // 3. Реализуйте алгоритм быстрого возведения в степень в кольце вычетов.
        public static BigInteger PowInResidueRing(BigInteger x, BigInteger n, BigInteger module)
        {
            return PowModule(x, n, module);
        }

        #endregion

        #region Public Methods

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

        #endregion
    }
}