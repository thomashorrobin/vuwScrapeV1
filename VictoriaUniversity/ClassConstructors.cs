using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VictoriaUniversity
{
    public partial class UniversityYear
    {
        public UniversityYear()
        {
            this.year = 2014;
            UniversityYear.Build2014(this);
        }

        public UniversityYear(int Year)
        {
            this.year = Year;
        }

        public void AddTerm(UniversityTerm UniversityTerm)
        {
            this.universityTerms.Add(UniversityTerm);
        }
    }

    public partial class UniversityTerm
    {
        public UniversityTerm(UniversityYear UniversityYear, DateTime TermStartDate, int TermNumber)
        {
            this.universityYear = UniversityYear;
            this.termStartDate = TermStartDate;
            if(TermNumber > 0 && TermNumber <= 4)
            {
                this.termNumber = TermNumber;
            }
            else
            {
                throw new InvalidOperationException();
            }
            universityYear.AddTerm(this);
        }

        public UniversityTerm(UniversityYear UniversityYear, DateTime TermStartDate, int TermNumber, List<DateTime> DatesToAdd)
        {
            this.universityYear = UniversityYear;
            this.termStartDate = TermStartDate;
            this.startDatesOfWeeksInTerm.AddRange(DatesToAdd);
            if (TermNumber > 0 && TermNumber <= 4)
            {
                this.termNumber = TermNumber;
            }
            else
            {
                throw new InvalidOperationException();
            }
            universityYear.AddTerm(this);
        }

        public void AddWeekStartDates(List<DateTime> DatesToAdd)
        {
            this.startDatesOfWeeksInTerm.AddRange(DatesToAdd);
        }
    }

    public partial class Course
    {
        public Course(string CourseCode, UniversityYear universityYear)
        {
            if(Course.CheckCourseCode(CourseCode))
            {
                this.CourseCode = CourseCode;
                this.universityYear = universityYear;
            }
            else
            {
                throw new InvalidOperationException("Course code not right. Needs to be in TAXN201 format");
            }
        }

        public Course(string CourseCode)
        {
            if (Course.CheckCourseCode(CourseCode))
            {
                this.CourseCode = CourseCode;
                this.universityYear = new UniversityYear();
            }
            else
            {
                throw new InvalidOperationException("Course code not right. Needs to be in TAXN201 format");
            }
        }
    }

    public partial class CourseStream
    {
        public void AddTerm(UniversityTerm Term)
        {
            this.termsSpanned.Add(Term);
        }

        public void AddLectureTime(LectureTime lt)
        {
            this.LectureTimes.Add(lt);
        }

        public CourseStream(Course Course, int CRN)
        {
            this.course = Course;
            this.CRN = CRN;
        }

        public CourseStream(Course Course, int CRN, List<UniversityTerm> UniversityTerms)
        {
            this.course = Course;
            this.CRN = CRN;
            this.termsSpanned = UniversityTerms;
        }
    }

    public partial class LectureTime
    {
        public LectureTime(CourseStream CourseStream, UniversityTerm UniversityTerm, string RoomNumber, int DayOfTheWeek, DateTime StartTime, DateTime EndTime)
        {
            this.courseStream = CourseStream;
            this.universityTerm = UniversityTerm;
            this.roomNumber = RoomNumber;
            this.dayOfTheWeek = DayOfTheWeek;
            this.startTime = StartTime;
            this.endTime = EndTime;
            courseStream.AddLectureTime(this);
        }

        public LectureTime(CourseStream CourseStream, UniversityTerm UniversityTerm, string RoomNumber, int DayOfTheWeek, DateTime StartTime, int Duration)
        {
            this.courseStream = CourseStream;
            this.universityTerm = UniversityTerm;
            this.roomNumber = RoomNumber;
            this.dayOfTheWeek = DayOfTheWeek;
            this.startTime = StartTime;
            this.endTime = StartTime.AddMinutes(Duration);
            courseStream.AddLectureTime(this);
        }
    }

    public partial class Lecture
    {
        public Lecture(CourseStream CourseStream, string RoomNumber, DateTime StartTime, DateTime EndDate)
        {
            this.roomNumber = RoomNumber;
            this.courseStream = CourseStream;
            this.startTime = StartTime;
            this.endTime = EndDate;
        }

        public Lecture(CourseStream CourseStream, string RoomNumber, DateTime StartTime, int Duration)
        {
            this.roomNumber = RoomNumber;
            this.courseStream = CourseStream;
            this.startTime = StartTime;
            this.endTime = StartTime.AddMinutes(Duration);
        }
    }
}