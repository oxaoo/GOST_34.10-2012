/*
 * Описание класса цифровой подписи.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DS_GOST_34._10_2012
{
    class CDS
    {
        private BigInteger a = new BigInteger();
        private BigInteger b = new BigInteger();
        private BigInteger n = new BigInteger();
        private BigInteger p = new BigInteger();
        private byte[] xG;
        private CECPoint G = new CECPoint();
        
        public CDS(BigInteger p, BigInteger a, BigInteger b, BigInteger n, byte[] xG)
        {
            this.a = a;
            this.b = b;
            this.n = n;
            this.p = p;
            this.xG = xG;
        }

        //генерация секретного ключа заданной длины.
        public BigInteger genPrivateKey(int BitSize)
        {
            BigInteger d = new BigInteger();
            do
            {
                d.genRandomBits(BitSize, new Random());
            } while ((d < 0) || (d > n));
            return d;
        }

        //генерация публичного ключа (с помощью секретного).
        public CECPoint genPublicKey(BigInteger d)
        {
            CECPoint G=gDecompression();
            CECPoint Q = CECPoint.multiply(G, d);
            return Q;
        }

        //восстановление координат Y из координаты X и бита четности Y.
        private CECPoint gDecompression()
        {
            byte y = xG[0];
            byte[] x=new byte[xG.Length-1];
            Array.Copy(xG, 1, x, 0, xG.Length - 1);
            BigInteger Xcord = new BigInteger(x);
            BigInteger temp = (Xcord * Xcord * Xcord + a * Xcord + b) % p;
            BigInteger beta = modSqrt(temp, p);
            BigInteger Ycord = new BigInteger();
            if ((beta % 2) == (y % 2))
                Ycord = beta;
            else
                Ycord = p - beta;
            CECPoint G = new CECPoint();
            G.a = a;
            G.b = b;
            G.fieldChar = p;
            G.x = Xcord;
            G.y = Ycord;
            this.G = G;
            return G;
        }

        //вычисление квадратоного корня по модулю простого числа q.
        public BigInteger modSqrt(BigInteger a, BigInteger q)
        {
            BigInteger b = new BigInteger();
            do
            {
                b.genRandomBits(255, new Random());
            } 
            while (legendre(b, q) == 1);
            
            BigInteger s = 0;
            BigInteger t = q - 1;
            while ((t & 1) != 1)
            {
                s++;
                t = t >> 1;
            }

            BigInteger InvA = a.modInverse(q);
            BigInteger c = b.modPow(t, q);
            BigInteger r = a.modPow(((t + 1) / 2), q);
            BigInteger d = new BigInteger();
            for (int i = 1; i < s; i++)
            {
                BigInteger temp = 2;
                temp = temp.modPow((s - i - 1), q);
                d = (r.modPow(2, q) * InvA).modPow(temp, q);
                if (d == (q - 1))
                    r = (r * c) % q;
                c = c.modPow(2, q);
            }
            return r;
        }

        //вычисление символа Лежандра.
        public BigInteger legendre(BigInteger a, BigInteger q)
        {
            return a.modPow((q - 1) / 2, q);
        }

        //формирование цифровой подписи.
        public string genDS(byte[] h, BigInteger d)
        {
            BigInteger a = new BigInteger(h);
            BigInteger e = a % n;
            if (e == 0)
                e = 1;
            BigInteger k = new BigInteger();
            CECPoint C=new CECPoint();
            BigInteger r=new BigInteger();
            BigInteger s = new BigInteger();
            do
            {
                do
                {
                    k.genRandomBits(n.bitCount(), new Random());
                } 
                while ((k < 0) || (k > n));

                C = CECPoint.multiply(G, k);
                r = C.x % n;
                s = ((r * d) + (k * e)) % n;
            } 
            while ((r == 0)||(s==0));

            string Rvector = padding(r.ToHexString(), n.bitCount() / 4);
            string Svector = padding(s.ToHexString(), n.bitCount() / 4);
            return Rvector + Svector;
        }

        //проверка цифровой подписи.
        public bool verifDS(byte[] H, string sign, CECPoint Q)
        {
            string Rvector = sign.Substring(0, n.bitCount() / 4);
            string Svector = sign.Substring(n.bitCount() / 4, n.bitCount() / 4);
            BigInteger r = new BigInteger(Rvector, 16);
            BigInteger s = new BigInteger(Svector, 16);

            if ((r < 1) || (r > (n - 1)) || (s < 1) || (s > (n - 1)))
                return (false);

            BigInteger a = new BigInteger(H);
            BigInteger e = a % n;
            if (e == 0)
                e = 1;

            BigInteger v = e.modInverse(n);
            BigInteger z1 = (s * v) % n;
            BigInteger z2 = n + ((-(r * v)) % n);
            this.G = gDecompression();
            CECPoint A = CECPoint.multiply(G, z1);
            CECPoint B = CECPoint.multiply(Q, z2);
            CECPoint C = A + B;
            BigInteger R = C.x % n;
            if (R == r)
                return (true);
            else
                return (false);
        }

        //дополнить подпись нулями слева до длины n, 
        // где n - длина модуля в битах.
        private string padding(string input, int size)
        {
            if (input.Length < size)
            {
                do
                {
                    input = "0" + input;
                } 
                while (input.Length < size);
            }
            return (input);
        }
    }
}
