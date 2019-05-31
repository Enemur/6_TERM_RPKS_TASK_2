using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Lab2RPKS.Model
{
    static class Operations
    {
        public static BigInteger multipclicationByModule(BigInteger firstValue, BigInteger secondValue, BigInteger module)
        {
            BigInteger result;

            result = Mod(Mod(firstValue, module) * Mod(secondValue, module), module);

            return result;
        }

        public static BigInteger subtractionByModule(BigInteger firstValue, BigInteger secondValue, BigInteger module)
        {
            BigInteger result;

            result = Mod(Mod(firstValue, module) - Mod(secondValue, module), module);

            return result;
        }

        public static BigInteger additionByModule(BigInteger firstValue, BigInteger secondValue, BigInteger module)
        {
            BigInteger result;

            result = Mod(Mod(firstValue, module) + Mod(secondValue, module), module);

            return result;
        }

        public static BigInteger Mod(BigInteger firstValue, BigInteger secondValue)
        {
            return ((firstValue % secondValue) + secondValue) % secondValue;
        }

        public static BigInteger Divide(BigInteger firstValue, BigInteger secondValue, BigInteger module)
        {
            return Mod(firstValue * BigInteger.ModPow(secondValue, module - 2, module), module);
        }
    }
}
