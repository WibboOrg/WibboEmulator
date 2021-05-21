using Buttefly.Utilities;
using System;

namespace Buttefly.Communication.Encryption.KeyExchange
{
    public class DiffieHellman
    {
        public readonly int BITLENGTH = 32;

        public BigInteger Prime { get; private set; }
        public BigInteger Generator { get; private set; }

        private BigInteger PrivateKey;
        public BigInteger PublicKey { get; private set; }

        public DiffieHellman()
        {
            this.Initialize();
        }

        public DiffieHellman(int b)
        {
            this.BITLENGTH = b;

            this.Initialize();
        }

        public DiffieHellman(BigInteger prime, BigInteger generator)
        {
            this.Prime = prime;
            this.Generator = generator;

            this.Initialize(true);
        }

        private void Initialize(bool IgnoreBaseKeys = false)
        {
            this.PublicKey = 0;

            Random Rand = new Random();
            while (this.PublicKey == 0)
            {
                if (!IgnoreBaseKeys)
                {
                    this.Prime = BigInteger.genPseudoPrime(this.BITLENGTH, 10, Rand);
                    this.Generator = BigInteger.genPseudoPrime(this.BITLENGTH, 10, Rand);
                }

                byte[] bytes = new byte[this.BITLENGTH / 8];
                Randomizer.NextBytes(bytes);
                this.PrivateKey = new BigInteger(bytes);

                if (this.Generator > this.Prime)
                {
                    BigInteger temp = this.Prime;
                    this.Prime = this.Generator;
                    this.Generator = temp;
                }

                this.PublicKey = this.Generator.modPow(this.PrivateKey, this.Prime);

                if (!IgnoreBaseKeys)
                {
                    break;
                }
            }
        }

        public BigInteger CalculateSharedKey(BigInteger m)
        {
            return m.modPow(this.PrivateKey, this.Prime);
        }
    }
}

