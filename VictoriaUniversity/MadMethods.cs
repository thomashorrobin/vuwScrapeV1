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
        public UniversityTerm GetUniTerm(int TermNumber)
        {
            return this.universityTerms.Single(oo => oo.GetTermNumber() == TermNumber);
        }
    }

    public partial class UniversityTerm
    {
        public int GetTermNumber()
        {
            return termNumber;
        }

        public List<DateTime> GetWeekStartDates()
        {
            return startDatesOfWeeksInTerm;
        }
    }

    public partial class Course
    {
        /// <summary>
        /// Takes a user input of a course code (i.e. TAXN201) and checks that it's in the right format (first 4 characters are capitilised letters 
        /// last 3 are numbers). 
        /// </summary>
        /// <remarks>COMP112 would pass but comp112 would not. Also none existant subjects and subject numbers will not be picked up. Example XXXX222 and ECON299 will pass</remarks>
        /// <param name="CourseCode">The users inputted string</param>
        /// <returns>True if okay false if not</returns>
        public static bool CheckCourseCode(string CourseCode)
        {
            Regex regularExpression = new Regex("^[A-Z][A-Z][A-Z][A-Z][1-5][0-9][0-9]$");
            return regularExpression.IsMatch(CourseCode);
        }

        public UniversityYear GetUniversityYear()
        {
            return this.universityYear;
        }
    }

    public partial class CourseStream
    {
        public List<LectureTime> GetLectureTimes()
        {
            return this.LectureTimes;
        }

        public int GetCRN()
        {
            return this.CRN;
        }
    }

    public partial class LectureTime
    {
        public List<Lecture> CalculateLectureDates()
        {
            List<Lecture> lectures = new List<Lecture>();
            foreach(DateTime dt in this.universityTerm.GetWeekStartDates())
            {
                DateTime startDateTime = dt.AddDays(this.dayOfTheWeek - 1);
                startDateTime = startDateTime.AddHours(this.startTime.Hour);
                startDateTime = startDateTime.AddMinutes(this.startTime.Minute);
                DateTime endDateTime = dt.AddDays(this.dayOfTheWeek - 1);
                endDateTime = endDateTime.AddHours(this.endTime.Hour);
                endDateTime = endDateTime.AddMinutes(this.endTime.Minute);
                lectures.Add(new Lecture(this.courseStream,this.roomNumber,startDateTime,endDateTime));
            }
            return lectures;
        }
    }

    public partial class Lecture
    {
        public DateTime GetStartDateTime()
        {
            return this.startTime;
        }

        public string GetCourseCode()
        {
            return this.courseStream.GetCourse().GetCourseCode();
        }

        public DateTime GetEndDateTime()
        {
            return this.endTime;
        }

        public string GetRoomNumber()
        {
            return this.roomNumber;
        }
    }
}