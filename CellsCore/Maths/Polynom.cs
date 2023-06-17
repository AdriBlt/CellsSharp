using System.Collections.Generic;

namespace CellsCore.Maths
{
    class Polynom
    {
        private int degre;
        private readonly IDictionary<int, Complex> coefs;
        private IList<Complex> roots;

        public Polynom(params Complex[] coefficents)
        {
            this.coefs = new Dictionary<int, Complex>();
            this.degre = -1;
            this.roots = null;
            var lenght = coefficents.Length;
            for (int i = 0; i < lenght; i++)
            {
                this.Add(lenght - 1 - i, coefficents[i]);
            }
        }

        public void AddRoot(Complex root) {
            if (this.roots == null) {
                this.roots = this.GetRoots();
            }

            if (this.degre < 0) {
                this.AddCoef(1, new Complex(1));
                this.AddCoef(0, Complex.GetProduct(-1, root));
            } else {
                var oldCoef = this.coefs;
                var oldDegre = this.degre;
                this.coefs.Clear();
                this.degre = -1;
                for (int degree = 0; degree <= oldDegre; degree++)
                {
                    var value = oldCoef[degree];
                    if (value != null)
                    {
                        this.AddCoef(degree + 1, value);
                        this.AddCoef(degree, Complex.GetProduct(-1, value, root));
                    }
                }
            }

            if (this.roots != null) {
                this.roots.Add(root);
            }
        }

        public void Add(int index, Complex coef) {
            this.AddCoef(index, coef);
            this.roots = null;
        }

        public void Set(int index, Complex coef) {
            if (coef == null || coef == this.coefs[index]) {
                return;
            }

            this.coefs[index] = coef;
            if (index > this.degre && !coef.IsNull) {
                this.degre = index;
            } else if (index == this.degre && coef.IsNull) {
                this.UpdateDegre();
            }

            this.roots = null;
        }

        public Complex GetCoef(int deg) {
            var c = this.coefs[deg];
            if (c != null) {
                return c;
            }
            return Complex.ZERO;
        }

        public Polynom derive() {
            var deriv = new Polynom();
            for (int index = 1; index <= this.degre; index++)
            {
                var value = this.coefs[index];
                if (value != null)
                {
                    var coef = value.MultiplyByReal(index);
                    deriv.Add(index - 1, coef);
                }
            }

            return deriv;
        }

        public Polynom Integre(Complex value0)
        {
            var prim = new Polynom(value0);
            for (int index = 0; index <= this.degre; index++)
            {
                var value = this.coefs[index];
                if (value != null)
                {
                    var coef = value.DivideByReal(index + 1);
                    prim.Add(index + 1, coef);
                }
            }

            return prim;
        }

        public Complex getValue(Complex z)
        {
            var output = Complex.ZERO;

            // Complex zPow = new Complex(1);
            // for (int i = 0; i <= this.degre; i++) {
            // if (!this.getCoef(i).isNul()) {
            // sortie = sortie.add(this.getCoef(i).multiply(zPow));
            // }
            // zPow = zPow.multiply(z);
            // }

            // HORNER METHOD
            for (int i = this.degre; i >= 0; i--)
            {
                if (!this.GetCoef(i).IsNull)
                {
                    output = output.Add(this.GetCoef(i));
                }

                if (i > 0)
                {
                    output = output.Multiply(z);
                }
            }

            return output;
        }

        public override string ToString()
        {
            if (this.degre < 0)
            {
                return "0";
            }

            var str = "";
            var empty = true;
            var one = new Complex(1);
            for (int i = this.degre; i >= 0; i--)
            {
                var coef = this.GetCoef(i);
                if (!coef.IsNull)
                {
                    if (empty)
                    {
                        empty = false;
                    }
                    else
                    {
                        str += " + ";
                    }
                    if (i == 0)
                    {
                        str += coef.ToString();
                    }
                    else
                    {
                        if (!coef.IsReal && !coef.IsPureImaginary)
                        {
                            str += "(" + coef.ToString() + ") ";
                        }
                        else if (one != coef)
                        {
                            str += coef.ToString() + " ";
                        }

                        str += "z";
                        if (i > 1)
                        {
                            str += i;
                        }
                    }
                }
            }

            return str;
        }

        public IList<Complex> GetRoots() {
            if (this.roots == null)
            {
                this.roots = this.ComputeRoots();
            }

            return this.roots;
        }

        private void AddCoef(int index, Complex coef)
        {
            if (coef.IsNull)
            {
                return;
            }

            var value = this.coefs[index];
            if (value != null)
            {
                var newCoef = value.Add(coef);
                this.coefs[index] = newCoef;
            }
            else
            {
                this.coefs[index] = coef;
            }
            if (index > this.degre)
            {
                this.degre = index;
            }
            else if (index == this.degre)
            {
                this.UpdateDegre();
            }
        }

        private void UpdateDegre() {
            while (this.degre >= 0)
            {
                if (!this.GetCoef(this.degre).IsNull)
                {
                    return;
                }
                this.degre--;
            }
        }

        private IList<Complex> ComputeRoots() {
            var MAX_DEGREE = 4;
            if (this.degre > MAX_DEGREE)
            {
                return null;
            }

            var c0 = this.GetCoef(0);
            var c1 = this.GetCoef(1);
            var c2 = this.GetCoef(2);
            var c3 = this.GetCoef(3);
            var c4 = this.GetCoef(4);
            return PolynomHelpers.GetRoots4(c4, c3, c2, c1, c0);
        }
    }
}
