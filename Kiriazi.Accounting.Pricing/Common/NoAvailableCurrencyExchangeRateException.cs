using Kiriazi.Accounting.Pricing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kiriazi.Accounting.Pricing.Common
{
    class NoAvailableCurrencyExchangeRateException : Exception
    {
        public NoAvailableCurrencyExchangeRateException()
            : base()
        {

        }
        public NoAvailableCurrencyExchangeRateException(string message)
            : base(message)
        {

        }
        public NoAvailableCurrencyExchangeRateException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
        public Currency FromCurrency { get; set; }
        public Currency ToCurrency { get; set; }
    }
}
