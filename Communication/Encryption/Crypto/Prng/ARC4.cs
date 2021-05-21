namespace Buttefly.Communication.Encryption.Crypto.Prng
{
    public class ARC4
    {
        private int i;
        private int j;
        private readonly byte[] bytes;

        public const int POOLSIZE = 256;

        public ARC4()
        {
            this.bytes = new byte[POOLSIZE];
        }

        public ARC4(byte[] key)
        {
            this.bytes = new byte[POOLSIZE];
            this.Initialize(key);
        }

        public void Initialize(byte[] key)
        {
            this.i = 0;
            this.j = 0;

            for (this.i = 0; this.i < POOLSIZE; ++this.i)
            {
                this.bytes[this.i] = (byte)this.i;
            }

            for (this.i = 0; this.i < POOLSIZE; ++this.i)
            {
                this.j = (this.j + this.bytes[this.i] + key[this.i % key.Length]) & (POOLSIZE - 1);
                this.Swap(this.i, this.j);
            }

            this.i = 0;
            this.j = 0;
        }

        private void Swap(int a, int b)
        {
            byte t = this.bytes[a];
            this.bytes[a] = this.bytes[b];
            this.bytes[b] = t;
        }

        public byte Next()
        {
            this.i = ++this.i & (POOLSIZE - 1);
            this.j = (this.j + this.bytes[this.i]) & (POOLSIZE - 1);
            this.Swap(this.i, this.j);
            return this.bytes[(this.bytes[this.i] + this.bytes[this.j]) & (POOLSIZE - 1)];
        }

        public void Encrypt(ref byte[] src)
        {
            for (int k = 0; k < src.Length; k++)
            {
                src[k] ^= this.Next();
            }
        }

        public void Decrypt(ref byte[] src)
        {
            this.Encrypt(ref src);
        }
    }
}
