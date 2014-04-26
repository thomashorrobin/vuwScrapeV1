using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net;
using System.Text.RegularExpressions;
using VictoriaUniversity;
using Google.GData.Client;

namespace GoogleCalanderSync
{
    public struct ScreenScrapedInfo
    {
        public int lineNumber;
        public string lineString;

        public ScreenScrapedInfo(int LineNumber, string LineString)
        {
            lineNumber = LineNumber;
            lineString = LineString;
        }

        public override string ToString()
        {
            return lineNumber.ToString() + " " + lineString;
        }
    }

    public class CourseNotValid : Exception
    {
        public override string Message
        {
            get
            {
                return "This course code isn't valid";
            }
        }
    }

    public class SchoolTerm : IDebugable
    {
        public int lineNumberStartPossition = 0;
        public int lineNumberEndPossition = 0;

        public List<ScreenScrapedInfo> ReturnScreenScrapedInfoInTermRange(List<ScreenScrapedInfo> InfoToFilter)
        {
            return InfoToFilter.Where(oo => oo.lineNumber > this.lineNumberStartPossition && oo.lineNumber < this.lineNumberEndPossition).ToList();
        }

        public void SetLineNumberRange(List<ScreenScrapedInfo> HtmlExtract, int StartLineNumber, int EndLineNumber)
        {
            if(debug)
            {
                Console.WriteLine("Calculating Term" + this.TermNumber.ToString());
            }
            string DateString = "";
            string NextDateString = "";
            switch(termEnum)
            {
                case Course2.Terms.Term1:
                    DateString = "3rd March 2014";
                    break;
                case Course2.Terms.Term2:
                    DateString = "5th May 2014";
                    break;
                case Course2.Terms.Term3:
                    DateString = "14th July 2014";
                    break;
                case Course2.Terms.Term4:
                    DateString = "8th September 2014";
                    break;
            }
            switch (termEnum)
            {
                case Course2.Terms.Term1:
                    NextDateString = "5th May 2014";
                    break;
                case Course2.Terms.Term2:
                    NextDateString = "14th July 2014";
                    break;
                case Course2.Terms.Term3:
                    NextDateString = "8th September 2014";
                    break;
            }
            foreach(ScreenScrapedInfo ssi in HtmlExtract.Where(oo => oo.lineNumber > StartLineNumber && oo.lineNumber < EndLineNumber).OrderBy(oo => oo.lineNumber))
            {
                if(ssi.lineString.Substring(15).StartsWith(DateString))
                {
                    lineNumberStartPossition = ssi.lineNumber;
                    lineNumberEndPossition = 1000000;
                    if(debug)
                    {
                        Console.WriteLine(ssi.lineString + " set as start line");
                    }
                }
                else if (ssi.lineString.Substring(15).StartsWith(NextDateString) && NextDateString != "")
                {
                    lineNumberEndPossition = ssi.lineNumber;
                    if (debug)
                    {
                        Console.WriteLine(ssi.lineString + " set as end line");
                    }
                }
            }
        }

        public void SetLineNumberPossitions(int startingPossition, int endPossition)
        {
            if(startingPossition < endPossition)
            {
                lineNumberStartPossition = startingPossition;
                lineNumberEndPossition = endPossition;
            }
            else
            {
                if(debug)
                {
                    Console.WriteLine("start number higher than end number");
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("Term number " + this.TermNumber.ToString() + " starts on " + this.TermStartDate.ToLongDateString());
            sb.AppendLine();
            sb.AppendLine("Line number range " + lineNumberStartPossition.ToString() + " to " + lineNumberEndPossition.ToString());
            sb.AppendLine("Starts on these dates:");
            foreach (DateTime dt in this.universityTerm.GetWeekStartDates())
            {
                sb.AppendLine(dt.ToShortDateString());
            }
            return sb.ToString();
        }

        public static void AddTermData(ITermStorable objectToAddTo, UniversityYear universtiyYear, bool Debug)
        {
            SchoolTerm newTerm1 = new SchoolTerm(1, new DateTime(2014, 3, 3), universtiyYear, Debug);
            newTerm1.universityTerm = universtiyYear.GetUniTerm(newTerm1.TermNumber);
            objectToAddTo.AddTerm(newTerm1);
            SchoolTerm newTerm2 = new SchoolTerm(2, new DateTime(2014, 5, 5), universtiyYear, Debug);
            newTerm2.universityTerm = universtiyYear.GetUniTerm(newTerm2.TermNumber);
            objectToAddTo.AddTerm(newTerm2);
            SchoolTerm newTerm3 = new SchoolTerm(3, new DateTime(2014, 7, 14), universtiyYear, Debug);
            newTerm3.universityTerm = universtiyYear.GetUniTerm(newTerm3.TermNumber);
            objectToAddTo.AddTerm(newTerm3);
            SchoolTerm newTerm4 = new SchoolTerm(4, new DateTime(2014, 9, 8), universtiyYear, Debug);
            newTerm4.universityTerm = universtiyYear.GetUniTerm(newTerm4.TermNumber);
            objectToAddTo.AddTerm(newTerm4);
        }

        private SchoolTerm(int termNo, DateTime startD, VictoriaUniversity.UniversityYear uniYear, bool Debug)
        {
            TermNumber = termNo;
            if(Debug)
            {
                SetToDebug();
            }
            TermStartDate = startD;
            if (termNo == 1) { termEnum = Course2.Terms.Term1;}
            else if (termNo == 2) { termEnum = Course2.Terms.Term2;}
            else if (termNo == 3) { termEnum = Course2.Terms.Term3;}
            else if (termNo == 4) { termEnum = Course2.Terms.Term4;}
        }

        public UniversityTerm universityTerm;
        public Course2.Terms termEnum;
        public int TermNumber
        {
            get;
            set;
        }
        public DateTime TermStartDate
        {
            set;
            get;
        }

        public void SetToDebug()
        {
            debug = true;
        }

        private bool debug = false;
    }

    public interface IDebugable
    {
        void SetToDebug();
    }

    public static class GoogleCalanderUpload
    {
        public static void AddToGoogleCalander(Lecture l, GoogleLoginWrapper Login)
        {
            FeedQuery feedQuery = Login.GetFeedQuery();
            Google.GData.Calendar.EventEntry calEvent = new Google.GData.Calendar.EventEntry(l.GetCourseCode(), l.ToString(), l.GetRoomNumber());
            calEvent.Authors.Add(Login.Author());
            calEvent.AddExtension(new Google.GData.Extensions.When(l.GetStartDateTime(),l.GetEndDateTime()));
            Service service = Login.CurrentService();
            calEvent.Service = service;
            calEvent.Update();
            service.Query(feedQuery);
            
        }
    }

    /// <summary>
    /// This class uses publicly avalible non https pages to determine lecture times. A private list of LectureTimes will be exacutable.
    /// </summary>
    /// 
    public class Course2 : Course, ITermStorable, IDebugable
    {
        public enum Terms { Term1, Term2, Term3, Term4 }

        bool debug = false;

        public void SetToDebug()
        {
            debug = true;
        }

        CourseStream cs;
        List<LectureTime> allLectureTimes = new List<LectureTime>();

        public Course2(string CourseCode, bool Debug)
            :base(CourseCode)
        {
            if(!VictoriaUniversity.Course.CheckCourseCode(CourseCode))
            {
                throw new CourseNotValid();
            }
            if (Debug) { SetToDebug(); }
            uri = new Uri("http://www.victoria.ac.nz/courses/" + CourseCode.Substring(0,4).ToLower() + "/" + CourseCode.Substring(4,3) + "?year=2014");
            Console.WriteLine(uri);
            client.DownloadFile(uri, CourseCode + ".htm");
            doc.Load(CourseCode + ".htm");
            SchoolTerm.AddTermData(this,this.GetUniversityYear(),debug);
            cs = chooseCRN(this);
            printNodes("//section/div/h3","Trimester");
            List<ScreenScrapedInfo> tempList = printNodes("//p", "Teaching dates");
            List<ScreenScrapedInfo> LectureDatails = printNodes("//ol/li/h6/div");
            foreach(SchoolTerm st in copyOfSchoolTerms)
            {
                st.SetLineNumberRange(tempList,startingLinePosition,endingLinePosition);
                allLectureTimes.AddRange(CreateLectureTime(st.ReturnScreenScrapedInfoInTermRange(LectureDatails),st));
            }
            if(debug)
            {
                Console.WriteLine("==Debug==");
                Console.WriteLine("Print out SchoolTerms:");
                foreach (SchoolTerm st in copyOfSchoolTerms)
                {
                    Console.WriteLine(st.ToString());
                    Console.WriteLine("==" + st.TermStartDate.ToLongDateString() + "==");
                }
            }
            Console.WriteLine("Lecture times added:");
            foreach (LectureTime lt in allLectureTimes)
            {
                Console.WriteLine(lt.ToString());
            }
        }

        public List<LectureTime> GetLectureTimes()
        {
            return this.allLectureTimes;
        }

        private List<LectureTime> CreateLectureTime(List<ScreenScrapedInfo> infoToUse, SchoolTerm st)
        {
            List<LectureTime> addedLectureTimes = new List<LectureTime>();
                List<ScreenScrapedInfo> threeStringsToUse = infoToUse.Where(wh => nodeInEstablishedRange(wh.lineNumber)).ToList();
                DateTime startTime = new DateTime(2014,1,1,8,30,0);
                DateTime endTime = new DateTime(2014, 1, 1, 8, 30, 0);
                int dayOfDaWeek = 1;
                string roomName = "No room";
                foreach (ScreenScrapedInfo sci in threeStringsToUse.OrderBy(oo => oo.lineNumber)) 
                {
                    if (sci.lineString.StartsWith("Monday")) { dayOfDaWeek = 1; }
                    else if (sci.lineString.StartsWith("Tuesday")) { dayOfDaWeek = 2; }
                    else if (sci.lineString.StartsWith("Wednesday")) { dayOfDaWeek = 3; }
                    else if (sci.lineString.StartsWith("Thursday")) { dayOfDaWeek = 4; }
                    else if (sci.lineString.StartsWith("Friday")) { dayOfDaWeek = 5; }
                    else
                    {
                        try
                        {
                            int startHour = int.Parse(sci.lineString.Substring(0, 2));
                            int startMinute = int.Parse(sci.lineString.Substring(3, 2));
                            int endHour = int.Parse(sci.lineString.Substring(8, 2));
                            int endMinute = int.Parse(sci.lineString.Substring(11, 2));
                            startTime = new DateTime(2014, 1, 1, startHour, startMinute, 0);
                            endTime = new DateTime(2014, 1, 1, endHour, endMinute, 0);
                            if(debug)
                            {
                                Console.WriteLine("Lecture starts " + startTime.ToLongTimeString());
                                Console.WriteLine("Lecture ends " + endTime.ToLongTimeString());
                            }
                        }
                        catch (FormatException fx)
                        {
                            if(debug)
                            {
                                Console.WriteLine("=============");
                                Console.WriteLine(fx.Message + " " + sci.lineString);
                            }
                            roomName = sci.lineString;
                            addedLectureTimes.Add(new LectureTime(cs,st.universityTerm,roomName,dayOfDaWeek,startTime,endTime));
                        }
                    }
                    if(debug)
                    {
                        Console.WriteLine("Success on " + sci.lineString);
                    }
                    //new LectureTime(startTime, endTime, roomName, dayOfDaWeek, newCourse);
                    infoToUse.Remove(sci); 
                }
            if(debug)
            {
                Console.WriteLine("+++ print all added lecture times +++");
                foreach(LectureTime lt in addedLectureTimes)
                {
                    Console.WriteLine(lt.ToString());
                }
            }
            return addedLectureTimes;
        }

        /// <summary>
        /// Determines weather there is more than one CRN avalible and prompts the console for the user to choose if there is.
        /// </summary>
        private CourseStream chooseCRN(Course course)
        {
            if (doc.DocumentNode.SelectNodes("//section/div/h2").Count > 1)
            {
                Console.WriteLine("Choose from bellow CRNs");
                printNodes("//section/div/h2",true);
                int crnToChoose = 0;
                try
                {
                    crnToChoose = int.Parse(Console.ReadLine()) - 1;
                    if(crnToChoose ==0)
                    {
                        endingLinePosition = doc.DocumentNode.SelectNodes("//section/div/h2").ElementAt(crnToChoose + 1).LinePosition;
                    }
                    else if(crnToChoose == doc.DocumentNode.SelectNodes("//section/div/h2").Count - 1)
                    {
                        startingLinePosition = doc.DocumentNode.SelectNodes("//section/div/h2").ElementAt(crnToChoose).LinePosition;
                    }
                    else if (crnToChoose > 0 && doc.DocumentNode.SelectNodes("//section/div/h2").Count > crnToChoose)
                    {
                        endingLinePosition = doc.DocumentNode.SelectNodes("//section/div/h2").ElementAt(crnToChoose + 1).LinePosition;
                        startingLinePosition = doc.DocumentNode.SelectNodes("//section/div/h2").ElementAt(crnToChoose).LinePosition;
                    }
                    else
                    {
                        Console.WriteLine("Not correct format. Must be integar between 1 - 147.");
                        chooseCRN(course);
                    }
                    return new CourseStream(course, int.Parse(doc.DocumentNode.SelectNodes("//section/div/h2").ElementAt(crnToChoose).InnerText.Substring(4)));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Not correct format. Must be integar between 1 - 147. int.Parse threw - " + ex.Message);
                    chooseCRN(course);
                }
            }
            try
            {
                return new CourseStream(course, int.Parse(doc.DocumentNode.SelectNodes("//section/div/h2").ElementAt(0).InnerText.Substring(5)));
            }
            catch (FormatException fx)
                {
                    Console.WriteLine(fx.Message);
                    Console.WriteLine(doc.DocumentNode.SelectNodes("//section/div/h2").ElementAt(0).InnerText.Substring(5));
                    return new CourseStream(course, 1234);
                }
        }

        WebClient client = new WebClient();
        HtmlDocument doc = new HtmlDocument();
        Uri uri;
        int startingLinePosition = 0;
        int endingLinePosition = 1000000;

        /// <summary>
        /// Prints nodes to console allong with the line/column number of the node.
        /// Code will handle doc being uninitialised.
        /// </summary>
        /// <param name="XPath">The XPath query</param>
        private List<ScreenScrapedInfo> printNodes(string XPath)
        {
            List<ScreenScrapedInfo> listOfInfo = new List<ScreenScrapedInfo>();
            if(doc != null)
            {
                for (int i = 0; i < doc.DocumentNode.SelectNodes(XPath).Count; i++)
                {
                    if (nodeInEstablishedRange(doc.DocumentNode.SelectNodes(XPath).ElementAt(i).LinePosition))
                    {
                        if(debug)
                        {
                        Console.WriteLine((i + 1).ToString() + ". " + doc.DocumentNode.SelectNodes(XPath).ElementAt(i).InnerText
                            + " " + doc.DocumentNode.SelectNodes(XPath).ElementAt(i).LinePosition.ToString());
                        }
                        listOfInfo.Add(new ScreenScrapedInfo(doc.DocumentNode.SelectNodes(XPath).ElementAt(i).LinePosition, doc.DocumentNode.SelectNodes(XPath).ElementAt(i).InnerText));
                    }
                }
            }
            else
            {
                Console.WriteLine("No document loaded");
            }
            return listOfInfo;
        }


        /// <summary>
        /// Prints nodes to console allong with the line/column number of the node.
        /// Code will handle doc being uninitialised.
        /// </summary>
        /// <param name="XPath">The XPath query</param>
        private List<ScreenScrapedInfo> printNodes(string XPath, bool RunInDebugMode)
        {
            List<ScreenScrapedInfo> listOfInfo = new List<ScreenScrapedInfo>();
            if (doc != null)
            {
                for (int i = 0; i < doc.DocumentNode.SelectNodes(XPath).Count; i++)
                {
                    if (nodeInEstablishedRange(doc.DocumentNode.SelectNodes(XPath).ElementAt(i).LinePosition))
                    {
                        if (RunInDebugMode)
                        {
                            Console.WriteLine((i + 1).ToString() + ". " + doc.DocumentNode.SelectNodes(XPath).ElementAt(i).InnerText
                                + " " + doc.DocumentNode.SelectNodes(XPath).ElementAt(i).LinePosition.ToString());
                        }
                        listOfInfo.Add(new ScreenScrapedInfo(doc.DocumentNode.SelectNodes(XPath).ElementAt(i).LinePosition, doc.DocumentNode.SelectNodes(XPath).ElementAt(i).InnerText));
                    }
                }
            }
            else
            {
                Console.WriteLine("No document loaded");
            }
            return listOfInfo;
        }

        /// <summary>
        /// Checks wheather element within the CRN range the user has set
        /// </summary>
        /// <param name="nodeLinePosition">The Node.LinePosition number</param>
        /// <returns>true for in range and false for out of range</returns>
        private bool nodeInEstablishedRange(int nodeLinePosition)
        {
            if(nodeLinePosition < endingLinePosition && startingLinePosition < nodeLinePosition)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Prints nodes to console allong with the line/column number of the node.
        /// Code will handle doc being uninitialised.
        /// </summary>
        /// <param name="XPath">The XPath query</param>
        /// <param name="startsWith">The match to the innerText property</param>
        private List<ScreenScrapedInfo> printNodes(string XPath, string startsWith)
        {
            List<ScreenScrapedInfo> listOfInfo = new List<ScreenScrapedInfo>();
            if (doc != null)
            {
                for (int i = 0; i < doc.DocumentNode.SelectNodes(XPath).Count; i++)
                {
                    if (doc.DocumentNode.SelectNodes(XPath).ElementAt(i).InnerText.StartsWith(startsWith) && nodeInEstablishedRange(doc.DocumentNode.SelectNodes(XPath).ElementAt(i).LinePosition))
                    {
                        if(debug)
                        {

                            Console.WriteLine((i + 1).ToString() + ". " + doc.DocumentNode.SelectNodes(XPath).ElementAt(i).InnerText
                                + " " + doc.DocumentNode.SelectNodes(XPath).ElementAt(i).LinePosition.ToString());
                        }
                        listOfInfo.Add(new ScreenScrapedInfo(doc.DocumentNode.SelectNodes(XPath).ElementAt(i).LinePosition, doc.DocumentNode.SelectNodes(XPath).ElementAt(i).InnerText));
                    }
                }
            }
            else
            {
                Console.WriteLine("No document loaded");
            }
            return listOfInfo;
        }

        public List<SchoolTerm> copyOfSchoolTerms = new List<SchoolTerm>();

        public void AddTerm(SchoolTerm schoolTerm)
        {
            copyOfSchoolTerms.Add(schoolTerm);
        }
    }

    public interface ITermStorable
    {
        void AddTerm(SchoolTerm schoolTerm);
    }
}
