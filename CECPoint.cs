/*
 * Описание класса точки эллептической кривой заданной уравнением: y^2=x^3+ax+(b % p).
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS_GOST_34._10_2012
{
    class CECPoint
    {
        public BigInteger a;
        public BigInteger b;
        public BigInteger x;
        public BigInteger y;
        public BigInteger fieldChar;

        public CECPoint()
        {
            a = new BigInteger();
            b = new BigInteger();
            x = new BigInteger();
            y = new BigInteger();
            fieldChar = new BigInteger();
        }

        public CECPoint(CECPoint p)
        {
            a = p.a;
            b = p.b;
            x = p.x;
            y = p.y;
            fieldChar = p.fieldChar;
        }

        //сложение пары точек.
        public static CECPoint operator+(CECPoint p1, CECPoint p2)
        {
            CECPoint res = new CECPoint();
            res.a = p1.a;
            res.b = p1.b;
            res.fieldChar = p1.fieldChar;

            BigInteger dx = p2.x - p1.x;
            BigInteger dy = p2.y - p1.y;

            if (dx < 0)
                dx += p1.fieldChar;
            if (dy < 0)
                dy += p1.fieldChar;

            BigInteger t = (dy * dx.modInverse(p1.fieldChar)) % p1.fieldChar;

            if (t < 0)
                t += p1.fieldChar;

            res.x = (t * t - p1.x - p2.x) % p1.fieldChar;
            res.y = (t * (p1.x - res.x) - p1.y) % p1.fieldChar;

            if (res.x < 0)
                res.x += p1.fieldChar;
            if (res.y < 0)
                res.y += p1.fieldChar;

            return (res);
        }

        //удвоение точки.
        public static CECPoint doubling(CECPoint p)
        {
            CECPoint res = new CECPoint();

            res.a = p.a;
            res.b = p.b;
            res.fieldChar = p.fieldChar;

            BigInteger dx = 2 * p.y;
            BigInteger dy = 3 * p.x * p.x + p.a;

            if (dx < 0)
                dx += p.fieldChar;
            if (dy < 0)
                dy += p.fieldChar;

            BigInteger t = (dy * dx.modInverse(p.fieldChar)) % p.fieldChar;
            res.x = (t*t - p.x - p.x) % p.fieldChar;
            res.y = (t * (p.x - res.x) - p.y) % p.fieldChar;

            if (res.x < 0)
                res.x += p.fieldChar;
            if (res.y < 0)
                res.y += p.fieldChar;

            return (res);
        }

        //умножение точки на число.
        public static CECPoint multiply(CECPoint p, BigInteger c)
        {
            CECPoint res = p;
            c = c - 1;
            while(c!=0)
            {
                if ((c%2)!=0)
                {
                    if ((res.x == p.x) || (res.y == p.y))
                        res = doubling(res);
                    else
                        res = res + p;
                    c=c-1; 
                }

                c = c / 2;
                p = doubling(p);
            }

            return (res);
        }
    }
}
