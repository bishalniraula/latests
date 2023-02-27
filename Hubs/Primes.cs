using System.Numerics;

namespace SignalRSample.Hubs
{
    public class Primes
    {
        private const int MaxValue = 25000;
        private readonly bool[] isPrime = new bool[MaxValue + 1];
        private readonly List<int> primes = new List<int>();

        internal Primes()
        {
            for (var i = 2; i <= MaxValue; i++)
            {
                if (!isPrime[i])
                {
                    primes.Add(i);
                    for (var j = i * i; j <= MaxValue; j += i)
                    {
                        isPrime[j] = true;
                    }
                }
            }
        }

        internal Key GetKey()
        {
            var end = primes.Count - 1;
            var start = end / 4;
            var random = new Random();
            var primeOne = primes[random.Next(start, end)];
            var primeTwo = primes[random.Next(start, end)];
            // var primeThree = primes[random.Next(start, end)];

            // if ((primeTwo == primeOne) | (primeThree == primeOne)) 
            // {
            //     primeOne = primes[random.Next(start, end)];
            // }else if ((primeTwo == primeOne) | (primeThree == primeTwo)) 
            // {
            //     primeTwo = primes[random.Next(start, end)];

            // }
            while (primeTwo == primeOne)
            {
                primeTwo = primes[random.Next(start, end)];
            }
            return new Key(primeOne, primeTwo, primes);
        }
    }
    internal class Key
    {
        private readonly int n;
        private readonly int e;
        private int d;

        internal Key(int primeOne, int primeTwo, List<int> primes)
        {
            n = primeOne * primeTwo;
            var phi = (primeOne - 1) * (primeTwo - 1);
            var end = primes.Count - 1;
            var start = end / 4;
            var random = new Random();
            do
            {
                do
                {
                    e = primes[random.Next(start, end)];
                } while (e == primeOne || e == primeTwo);
            } while (!IsFoundD(phi));
            Console.WriteLine("Public Key: (e, n) = (" + e + ", " + n + ")");
        }

        private bool IsFoundD(int phi)
        {
            for (var i = phi - 1; i > 1; i--)
            {
                var mul = BigInteger.Multiply(e, i);
                var result = BigInteger.Remainder(mul, phi);
                if (result.Equals(1))
                {
                    d = i;
                    Console.WriteLine("Private Key: (d, n) = (" + d + ", " + n + ")");
                    return true;
                }
            }
            return false;
        }

        internal int[] Encrypt(string message)
        {
            var charArray = message.ToCharArray();
            var array = new int[charArray.Length];
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = (int)BigInteger.ModPow(charArray[i], e, n);
            }
            return array;
        }

        internal string Decrypt(int[] cyphertext)
        {
            var array = new char[cyphertext.Length];
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = (char)BigInteger.ModPow(cyphertext[i], d, n);
            }
            return new string(array);
        }
    }
}
