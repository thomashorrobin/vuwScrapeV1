using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VictoriaUniversity
{
    public partial class UniversityYear
    {
        private int year;
        private List<UniversityTerm> universityTerms = new List<UniversityTerm>();

        public override string ToString()
        {
            return year.ToString();
        }
    }

    public partial class UniversityTerm
    {
        private UniversityYear universityYear;
        private List<DateTime> startDatesOfWeeksInTerm = new List<DateTime>();
        private DateTime termStartDate;
        private int termNumber;

        public override string ToString()
        {
            return "Term" + termNumber.ToString() + " " + universityYear.ToString();
        }
    }

    public partial class Course
    {
        private UniversityYear universityYear;
        private string CourseCode;

        public string GetCourseCode()
        {
            return CourseCode;
        }
        public override string ToString()
        {
            return CourseCode + " " + universityYear.ToString();
        }
    }

    public partial class CourseStream
    {
        private Course course;
        private List<UniversityTerm> termsSpanned;
        private List<LectureTime> LectureTimes = new List<LectureTime>();
        private int CRN;

        public Course GetCourse()
        {
            return course;
        }

        public override string ToString()
        {
            return course.ToString() + " " + CRN.ToString();
        }
    }

    public partial class LectureTime
    {
        private static string[] theWeekDays = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saterday" };

        private CourseStream courseStream;
        private UniversityTerm universityTerm;
        private string roomNumber;
        private int dayOfTheWeek;
        private DateTime startTime;
        private DateTime endTime;

        public override string ToString()
        {
            return courseStream.GetCourse().GetCourseCode() + " on " + theWeekDays[dayOfTheWeek] + "s from " + startTime.ToLongTimeString() + " to " + endTime.ToLongTimeString() + " - Term" + this.universityTerm.GetTermNumber();
        }
    }

    public partial class Lecture
    {
        private CourseStream courseStream;
        private DateTime startTime;
        private DateTime endTime;
        private string roomNumber;

        public override string ToString()
        {
            return courseStream.GetCourse().GetCourseCode() + " lecture starting " + startTime.ToShortDateString() + " at " + startTime.ToLongTimeString() + " to " + endTime.ToLongTimeString() + " in " + this.roomNumber + " CRN " + this.courseStream.GetCRN().ToString();
        }
    }
}
