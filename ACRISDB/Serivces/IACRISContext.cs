using ACRISDB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACRISDB.Serivces
{
    public interface IACRISContext
    {
        DbSet<vwUpdateTrancation> vwUpdateTrancations { get; }
        int SaveChanges();
    }

}
