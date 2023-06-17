using System;
using System.Collections.Generic;
using System.Linq;

namespace CellsCore.Maths
{
    static class PolynomHelpers
    {
        public static IList<Complex> GetRoots1(Complex c1, Complex c0)
        {
            var roots = new List<Complex>();
            if (c1.IsNull)
            {
                return roots;
            }

            roots.Add(c0.MultiplyByReal(-1).Divide(c1));
            return roots;
        }

        public static IList<Complex> GetRoots2(Complex c2, Complex c1, Complex c0)
        {
            if (c2.IsNull)
            {
                return GetRoots1(c1, c0);
            }

            var roots = new List<Complex>();
            var delta = Complex.GetSum(0, c1.GetSquare(), Complex.GetProduct(-4, c2, c0));
            roots.Add(
              c1
                .Add(delta.GetSquareRoot())
                .Divide(c2)
                .MultiplyByReal(-0.5)
            );
            roots.Add(
              c1
                .Minus(delta.GetSquareRoot())
                .Divide(c2)
                .MultiplyByReal(-0.5)
            );
            return roots;
        }

        public static IList<Complex> GetRoots3(
          Complex c3,
          Complex c2,
          Complex c1,
          Complex c0)
        {
            if (c3.IsNull)
            {
                return GetRoots2(c2, c1, c0);
            }

            var roots = new List<Complex>();
            var p = Complex.GetSum(
              0,
              c1.Divide(c3),
              c2
                .GetSquare()
                .Divide(c3.GetSquare())
                .MultiplyByReal(-1.0 / 3.0)
            );
            var q = Complex.GetSum(
              0,
              c0.Divide(c3),
              c2
                .Multiply(c1)
                .Divide(c3.GetSquare())
                .MultiplyByReal(-1.0 / 3.0),
              c2
                .GetCube()
                .Divide(c3.GetCube())
                .MultiplyByReal(2.0 / 27.0)
            );

            var cardan = GetRoots2(
              new Complex(1),
              q,
              p.GetCube().MultiplyByReal(-1.0 / 27.0)
            );
            var chVar = c2.Divide(c3).MultiplyByReal(-1.0 / 3.0);
            var u = cardan[0].GetCubeRoot();
            var v = cardan[1].GetCubeRoot();
            var root = Complex.GetSum(0, chVar, u, v);
            roots.Add(root);
            //  r2 = Complex.GetSum(0, chVar, u.Multiply(Complex.J), v.Multiply(Complex.J2));
            //  r3 = Complex.GetSum(0, chVar, u.Multiply(Complex.J2), v.Multiply(Complex.J));
            // roots.Add(r2);
            // roots.Add(r3);
            var bb = c2.Add(c3.Multiply(root));
            var cc = c1.Add(bb.Multiply(root));
            roots.AddRange(GetRoots2(c3, bb, cc));
            return roots;
        }

        public static IList<Complex> GetRoots4(
          Complex c4,
          Complex c3,
          Complex c2,
          Complex c1,
          Complex c0)
        {
            if (c4.IsNull)
            {
                return GetRoots3(c3, c2, c1, c0);
            }

            var roots = new List<Complex>();
            var p = Complex.GetSum(
              0,
              c2.Divide(c4),
              c3.GetSquare().Divide(c4.GetSquare()).MultiplyByReal(-0.375)
            );
            var q = Complex.GetSum(
              0,
              c1.Divide(c4),
              c3.Multiply(c2).Divide(c4.GetSquare()).MultiplyByReal(-0.5),
              c3.GetCube().Divide(c4.GetCube()).MultiplyByReal(0.125)
            );
            var r = Complex.GetSum(
              0,
              c0.Divide(c4),
              c3.Multiply(c1).Divide(c4.GetSquare()).MultiplyByReal(-0.25),
              c3.GetSquare().Multiply(c2).Divide(c4.GetCube()).MultiplyByReal(0.0625),
              c3
                .Divide(c4)
                .GetPow(4)
                .MultiplyByReal(-3.0 / 256)
            );
            var ferrari = GetRoots3(
              new Complex(8),
              p.MultiplyByReal(-4),
              r.MultiplyByReal(-8),
              r.Multiply(p).MultiplyByReal(4).Minus(q.GetSquare())
            );

            var root = ferrari[0];
            var aa = root.MultiplyByReal(2).Minus(p).GetSquareRoot();
            var bb = aa.IsNull
              ? root.GetSquare().Minus(r).GetSquareRoot()
              : q.Divide(aa).MultiplyByReal(-0.5);

            roots.AddRange(GetRoots2(new Complex(1), aa, root.Add(bb)));
            roots.AddRange(GetRoots2(new Complex(1), aa.MultiplyByReal(-1), root.Minus(bb)));

            var chVar = c3.Divide(c4).MultiplyByReal(-0.25);
            return roots.Select((complex) => complex.Add(chVar)).ToList();
        }

        public static Polynom GetUnityRootsPolynom(int n)
        {
            if (n <= 0)
            {
                throw new ArgumentException("Invalid parameter");
            }

            var p = new Polynom();
            p.AddRoot(new Complex(1));

            if (n == 1)
            {
                return p;
            }

            if (n == 2)
            {
                p.AddRoot(new Complex(-1));
                return p;
            }

            if (n == 3)
            {
                p.AddRoot(Complex.J);
                p.AddRoot(Complex.J2);
                return p;
            }

            if (n == 4)
            {
                p.AddRoot(new Complex(-1));
                p.AddRoot(Complex.I);
                p.AddRoot(new Complex(0, -1));
                return p;
            }

            for (int k = 1; k < n; k++)
            {
                var theta = 2 * Math.PI * k / n;
                var re = Math.Cos(theta);
                var im = Math.Sin(theta);
                p.AddRoot(new Complex(re, im));
            }

            return p;
        }
    }
}
