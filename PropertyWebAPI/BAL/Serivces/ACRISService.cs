using ACRISDB.Serivces;
using PropertyWebAPI.BAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PropertyWebAPI.BAL.Serivces
{
    public class ACRISService
    {
        private IACRISContext _context;

        public ACRISService(IACRISContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all update ACRIS BBLE list in date range
        /// </summary>
        /// <param name="from">beginning date to get properties  </param>
        /// <param name="to">End date property</param>
        /// <returns>
        /// List of BBLE and date range queried in data
        /// </returns>
        public PropertyUpdateTransaction GetTransaction(DateTime from, DateTime? to = null)
        {

            try
            {

                var transactions = _context.vwUpdateTrancations
                                      .Where(t => t.DateTimeProcessed > from)
                                      .Where(t => to != null ? t.DateTimeProcessed < to : true);
                if (transactions.Count() > 0)
                {
                    List<string> bbles = transactions
                                         .GroupBy(t => t.BBL)
                                         .Select(g => g.Key)
                                         .ToList();

                    return new PropertyUpdateTransaction()
                    {
                        Data = bbles,
                        Beginning = transactions.Min(t => t.DateTimeProcessed),
                        Count = bbles.Count(),
                        End = transactions.Max(t => t.DateTimeProcessed)
                    };
                }
                return null;

            }
            catch (Exception e)
            {
                Common.Logs.log().Error(string.Format("Error reading AreaAbstract{0}", Common.Logs.FormatException(e)));
                return null;
            }


        }
    }
}