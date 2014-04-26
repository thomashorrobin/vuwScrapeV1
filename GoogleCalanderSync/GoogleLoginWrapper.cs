using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.GData.Client;
using Google.GData.Calendar;
using Google.GData.Extensions;

namespace GoogleCalanderSync
{
    public class GoogleLoginWrapper
    {
        private Uri postUri;
        private FeedQuery feedQuery = new FeedQuery();
        private Service service = new Service("cl", "tomsApplication01");
        private AtomPerson author = new AtomPerson(AtomPersonType.Author);
        private AtomFeed atomFeed;

        public AtomPerson Author()
        {
            return author;
        }

        public FeedQuery GetFeedQuery()
        {
            return feedQuery;
        }

        public Service CurrentService()
        {
            return service;
        }

        public GoogleLoginWrapper()
        {
            postUri = new Uri("http://www.google.com/calendar/feeds/thomasroberthorrobin@gmail.com/private/full");
            author.Name = "Thomas Horrobin";
            author.Email = "thomasroberthorrobin@gmail.com";
            service.setUserCredentials("thomasroberthorrobin@gmail.com", "HeavyBevi2012");
            feedQuery.Uri = postUri;
            atomFeed = service.Query(feedQuery);
        }

        public GoogleLoginWrapper(string DisplayName, string Email, string Password)
        {
            postUri = new Uri("http://www.google.com/calendar/feeds/" + Email + "/private/full");
            author.Name = DisplayName;
            author.Email = Email;
            service.setUserCredentials(Email, Password);
            feedQuery.Uri = postUri;
            atomFeed = service.Query(feedQuery);
        }
    }
}
