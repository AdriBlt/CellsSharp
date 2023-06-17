using System;

namespace CellsCore.Maths
{
    public class Complex
    {
        public static Complex ZERO = new Complex();
        public static Complex I = new Complex(0, 1);
        public static Complex J = new Complex(-0.5, Math.Sqrt(3) / 2);
        public static Complex J2 = new Complex(-0.5, -Math.Sqrt(3) / 2);

        public static Complex FromAngle(double angle, double magnitude = 1)
        {
            return new Complex(
              magnitude * Math.Cos(angle),
              magnitude * Math.Sin(angle)
            );
        }

        public static Complex GetSum(double coef, params Complex[] complexes)
        {
            var sum = new Complex(coef);
            foreach (var c in complexes)
            {
                sum = sum.Add(c);
            }

            return sum;
        }

        public static Complex GetProduct(double coef, params Complex[] complexes)
        {
            var product = new Complex(coef);
            foreach (var c in complexes)
            {
                product = product.Multiply(c);
            }

            return product;
        }

        private readonly double re;
        private readonly double im;

        public Complex(double re = 0, double im = 0)
        {
            this.re = re;
            this.im = im;
        }

        public double X { get { return this.re; } }
        public double Y { get { return this.im; } }
        public double Re { get { return this.re; } }
        public double Im { get { return this.im; } }

        public bool IsNull
        {
            get { return this.re == 0.0 && this.im == 0.0; }
        }

        public bool IsReal
        {
            get { return this.im == 0.0; }
        }

        public bool IsPureImaginary
        {
            get { return this.re == 0.0; }
        }

        public double GetSquareModule()
        {
            return this.re * this.re + this.im * this.im;
        }

        public double GetModule()
        {
            return Math.Sqrt(this.GetSquareModule());
        }

        public Complex AddRe(double d)
        {
            return new Complex(this.re + d, this.im);
        }

        public Complex AddIm(double d)
        {
            return new Complex(this.re, this.im + d);
        }

        public Complex Add(Complex c)
        {
            return new Complex(this.re + c.re, this.im + c.im);
        }

        public Complex Minus(Complex c)
        {
            return new Complex(this.re - c.re, this.im - c.im);
        }

        public Complex Multiply(Complex c)
        {
            return new Complex(
              this.re * c.re - this.im * c.im,
              this.im * c.re + this.re * c.im
            );
        }

        public Complex GetSquare()
        {
            return new Complex(
              this.re * this.re - this.im * this.im,
              2 * this.im * this.re
            );
        }

        public Complex GetSquareRoot()
        {
            var module = this.GetModule();
            return new Complex(
              Math.Sqrt((module + this.re) / 2),
              Math.Sqrt((module - this.re) / 2)
            );
        }

        public Complex GetCube()
        {
            return new Complex(
              this.re * this.re * this.re - 3 * this.re * this.im * this.im,
              3 * this.re * this.re * this.im - this.im * this.im * this.im
            );
        }

        public Complex GetCubeRoot()
        {
            if (this.IsReal)
            {
                return new Complex(Math.Pow(this.re, 1 / 3));
            }

            if (this.IsPureImaginary)
            {
                return new Complex(0, -Math.Pow(this.im, 1 / 3));
            }

            var module = this.GetModule();
            var theta = this.GetAngle();
            var re = Math.Pow(module, 1 / 3) * Math.Cos(theta / 3);
            var im = Math.Pow(module, 1 / 3) * Math.Sin(theta / 3);
            return new Complex(re, im);
        }

        public Complex GetPow(double puiss)
        {
            var module = this.GetModule();
            var theta = this.GetAngle();
            var re = Math.Pow(module, puiss) * Math.Cos(theta * puiss);
            var im = Math.Pow(module, puiss) * Math.Sin(theta * puiss);
            return new Complex(re, im);
        }

        public double GetAngle()
        {
            return Math.Atan2(this.im, this.re);
        }

        public double GetAngleCosinus()
        {
            return this.IsNull ? 0 : this.re / this.GetModule();
        }

        public double GetAngleSinus()
        {
            return this.IsNull ? 0 : this.im / this.GetModule();
        }

        public Complex Divide(Complex c)
        {
            var denum = c.GetSquareModule();
            if (denum == 0.0)
            {
                throw new DivideByZeroException("Division by zero");
            }

            return new Complex(
              (this.re * c.re + this.im * c.im) / denum,
              (-this.re * c.im + this.im * c.re) / denum
            );
        }

        public double GetSquareDistanceFrom(Complex c)
        {
            return (
              (this.re - c.re) * (this.re - c.re) +
              (this.im - c.im) * (this.im - c.im)
            );
        }

        public bool IsEqual(Complex c)
        {
            return this.re == c.re && this.im == c.im;
        }

        public Complex MultiplyByReal(double d)
        {
            return new Complex(d * this.re, d * this.im);
        }

        public Complex DivideByReal(double d)
        {
            if (d == 0.0)
            {
                throw new DivideByZeroException("Division by zero");
            }

            return new Complex(this.re / d, this.im / d);
        }

        public override string ToString()
        {
            var re = this.re.ToString();
            var im = this.im.ToString();

            if (this.IsReal)
            {
                return re;
            }

            if (this.IsPureImaginary)
            {
                if (this.im == 1)
                {
                    return "i";
                }

                return im + "i";
            }

            if (this.im < 0)
            {
                return re + im + "i";
            }

            if (this.im == 1)
            {
                return re + "+i";
            }

            return re + "+" + im + "i";
        }

        public Complex Clone()
        {
            return new Complex(this.re, this.im);
        }

        public override bool Equals(object obj)
        {
            return obj is Complex complex &&
                   this.IsEqual(complex);
        }

        public override int GetHashCode()
        {
            return -1274390064 + this.re.GetHashCode() + 31 * this.im.GetHashCode();
        }
    }
}
