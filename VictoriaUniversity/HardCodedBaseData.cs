using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VictoriaUniversity
{
    public partial class UniversityYear
    {
        private static void Build2014(UniversityYear universityYear)
        {
            List<DateTime> startDays = new List<DateTime>();
            startDays.Add(new DateTime(2014,3,3));
            startDays.Add(new DateTime(2014, 3, 10));
            startDays.Add(new DateTime(2014, 3, 17));
            startDays.Add(new DateTime(2014, 3, 24));
            startDays.Add(new DateTime(2014, 3, 31));
            startDays.Add(new DateTime(2014, 4, 7));
            startDays.Add(new DateTime(2014, 4, 14));
            new UniversityTerm(universityYear, new DateTime(2014, 3, 3), 1,startDays);
            startDays.Clear();
            startDays.Add(new DateTime(2014, 5, 5));
            startDays.Add(new DateTime(2014, 5, 12));
            startDays.Add(new DateTime(2014, 5, 19));
            startDays.Add(new DateTime(2014, 5, 26));
            startDays.Add(new DateTime(2014, 6, 2));
            new UniversityTerm(universityYear, new DateTime(2014, 5, 5), 2, startDays);
            startDays.Clear();
            startDays.Add(new DateTime(2014, 7, 14));
            startDays.Add(new DateTime(2014, 7, 21));
            startDays.Add(new DateTime(2014, 7, 28));
            startDays.Add(new DateTime(2014, 8, 4));
            startDays.Add(new DateTime(2014, 8, 11));
            startDays.Add(new DateTime(2014, 8, 18));
            new UniversityTerm(universityYear, new DateTime(2014, 7, 14), 3, startDays);
            startDays.Clear();
            startDays.Add(new DateTime(2014, 9, 8));
            startDays.Add(new DateTime(2014, 9, 15));
            startDays.Add(new DateTime(2014, 9, 22));
            startDays.Add(new DateTime(2014, 9, 29));
            startDays.Add(new DateTime(2014, 10, 6));
            startDays.Add(new DateTime(2014, 10, 13));
            new UniversityTerm(universityYear, new DateTime(2014, 9, 8), 4, startDays);
        }
    }
}