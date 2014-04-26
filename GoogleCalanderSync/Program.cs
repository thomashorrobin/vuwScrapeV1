using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.GData.Client;
using VictoriaUniversity;

namespace GoogleCalanderSync
{
    class Program : IDebugable
    {
        private GoogleLoginWrapper googleLoginWrapper = new GoogleLoginWrapper();
        bool debug = false;
        Course2 crs;
        List<LectureTime> lectureTimes = new List<LectureTime>();
        List<Lecture> lectures = new List<Lecture>();

        static void Main(string[] args)
        {
            Program p = new Program();
            p.MainMenu();
        }

        private void MainMenu()
        {
            Console.WriteLine("");
                Console.WriteLine("Main menu:");
                Console.WriteLine("Login");
                Console.WriteLine("LoginTom");
                Console.WriteLine("Debug");
                Console.WriteLine("ViewLectureTimes");
                Console.WriteLine("ViewLectures");
                Console.WriteLine("CalculateLectures");
                Console.WriteLine("ClearLectureTimes");
                Console.WriteLine("Exit");
                Console.WriteLine("");
                string cmd = Console.ReadLine();
            if(Course.CheckCourseCode(cmd.ToUpper()))
            {
                try
                {
                    crs = new Course2(cmd.ToUpper(), debug);
                    lectureTimes.AddRange(crs.GetLectureTimes());
                }
                catch (CourseNotValid ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
                switch(cmd)
                {
                    case "Debug":
                        SetToDebug();
                        break;
                    case "ClearLectureTimes":
                        lectureTimes.Clear();
                        break;
                    case "ViewLectureTimes":
                        foreach(LectureTime lt in lectureTimes)
                        {
                            Console.WriteLine(lt.ToString());
                        }
                        break;
                    case "CalculateLectures":
                        List<Lecture> tempLectures = new List<Lecture>();
                        foreach (LectureTime lt in lectureTimes)
                        {
                            tempLectures.AddRange(lt.CalculateLectureDates());
                        }
                        lectures.AddRange(tempLectures);
                        foreach (Lecture l in tempLectures)
                        {
                            Console.WriteLine("Added " + l.ToString());
                        }
                        lectureTimes.Clear();
                        break;
                    case "ViewLectures":
                        foreach (Lecture ltur in lectures.OrderBy(oo => oo.GetStartDateTime()))
                        {
                            Console.WriteLine(ltur.ToString());
                        }
                        break;
                    case "LoginTom":
                        googleLoginWrapper = new GoogleLoginWrapper();
                        break;
                    case "Login":
                        Console.WriteLine("Display Name");
                        string displayName = Console.ReadLine();
                        Console.WriteLine("Email");
                        string email = Console.ReadLine();
                        Console.WriteLine("Password");
                        string password = Console.ReadLine();
                        googleLoginWrapper = new GoogleLoginWrapper(displayName, email, password);
                        break;
                }
                if(cmd != "Exit")
                {
                    MainMenu();
                }
            }

        public void SetToDebug()
        {
            debug = true;
        }
    }
}
