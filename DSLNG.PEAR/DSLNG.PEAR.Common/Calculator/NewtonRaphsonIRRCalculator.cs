
using System;
namespace DSLNG.PEAR.Common.Calculator
{
    public class NewtonRaphsonIRRCalculator : ICalculator
    {
        private readonly double[] _cashFlows;
        private int _numberOfIterations;
        private double _result;

        public NewtonRaphsonIRRCalculator(double[] cashFlows)
        {
            _cashFlows = cashFlows;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is valid cash flows.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is valid cash flows; otherwise, <c>false</c>.
        /// </value>
        private bool IsValidCashFlows
        {
            //Cash flows for the first period must be positive
            //There should be at least two cash flow periods         
            get
            {
                const int MIN_NO_CASH_FLOW_PERIODS = 2;

                if (_cashFlows.Length < MIN_NO_CASH_FLOW_PERIODS || (_cashFlows[0] > 0))
                {
                    throw new ArgumentOutOfRangeException(
                        "Cash flow for the first period  must be negative and there should");
                }
                return true;
            }
        }

        /// <summary>
        /// Gets the initial guess.
        /// </summary>
        /// <value>The initial guess.</value>
        private double InitialGuess
        {
            get
            {
                double initialGuess = -1 * (1 + (_cashFlows[1] / _cashFlows[0]));
                return initialGuess;
            }
        }

        #region ICalculator Members

        public double Execute()
        {
            if (IsValidCashFlows)
            {
                DoNewtonRapshonCalculation(InitialGuess);

                if (_result > 1)
                    throw new IRRCalculationException(
                        "Failed to calculate the IRR for the cash flow series. Please provide a valid cash flow sequence");
            }
            return _result;
        }

        #endregion

        /// <summary>
        /// Does the newton rapshon calculation.
        /// </summary>
        /// <param name="estimatedReturn">The estimated return.</param>
        /// <returns></returns>
        private void DoNewtonRapshonCalculation(double estimatedReturn)
        {
            _numberOfIterations++;
            _result = estimatedReturn - SumOfIRRPolynomial(estimatedReturn) / IRRDerivativeSum(estimatedReturn);
            while (!HasConverged(_result) && 50000 != _numberOfIterations)
            {
                DoNewtonRapshonCalculation(_result);
            }
        }


        /// <summary>
        /// Sums the of IRR polynomial.
        /// </summary>
        /// <param name="estimatedReturnRate">The estimated return rate.</param>
        /// <returns></returns>
        private double SumOfIRRPolynomial(double estimatedReturnRate)
        {
            double sumOfPolynomial = 0;
            if (IsValidIterationBounds(estimatedReturnRate))
                for (int j = 0; j < _cashFlows.Length; j++)
                {
                    sumOfPolynomial += _cashFlows[j] / (Math.Pow((1 + estimatedReturnRate), j));
                }
            return sumOfPolynomial;
        }

        /// <summary>
        /// Determines whether the specified estimated return rate has converged.
        /// </summary>
        /// <param name="estimatedReturnRate">The estimated return rate.</param>
        /// <returns>
        /// 	<c>true</c> if the specified estimated return rate has converged; otherwise, <c>false</c>.
        /// </returns>
        private bool HasConverged(double estimatedReturnRate)
        {
            //Check that the calculated value makes the IRR polynomial zero.
            bool isWithinTolerance = Math.Abs(SumOfIRRPolynomial(estimatedReturnRate)) <= 0.00000001;
            return (isWithinTolerance) ? true : false;
        }

        /// <summary>
        /// IRRs the derivative sum.
        /// </summary>
        /// <param name="estimatedReturnRate">The estimated return rate.</param>
        /// <returns></returns>
        private double IRRDerivativeSum(double estimatedReturnRate)
        {
            double sumOfDerivative = 0;
            if (IsValidIterationBounds(estimatedReturnRate))
                for (int i = 1; i < _cashFlows.Length; i++)
                {
                    sumOfDerivative += _cashFlows[i] * (i) / Math.Pow((1 + estimatedReturnRate), i);
                }
            return sumOfDerivative * -1;
        }

        /// <summary>
        /// Determines whether [is valid iteration bounds] [the specified estimated return rate].
        /// </summary>
        /// <param name="estimatedReturnRate">The estimated return rate.</param>
        /// <returns>
        /// 	<c>true</c> if [is valid iteration bounds] [the specified estimated return rate]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsValidIterationBounds(double estimatedReturnRate)
        {
            return estimatedReturnRate != -1 && (estimatedReturnRate < int.MaxValue) &&
                   (estimatedReturnRate > int.MinValue);
        }
    }
}
