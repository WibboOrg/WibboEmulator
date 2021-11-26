using Butterfly.Game.Rooms;
using Butterfly.Game.Users;
using System.Collections.Generic;

namespace Butterfly.Game.Moderation
{
    public class SupportTicket
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public int Category { get; set; }
        public double Timestamp { get; set; }
        public int Priority { get; set; }
        public bool Answered { get; set; }
        public User Sender { get; set; }
        public User Reported { get; set; }
        public User Moderator { get; set; }
        public string Issue { get; set; }
        public RoomData Room { get; set; }

        public List<string> ReportedChats;

        public SupportTicket(int id, int type, int category, double timestamp, int priority, User sender, User reported, string issue, RoomData room, List<string> reportedChats)
        {
            Id = id;
            Type = type;
            Category = category;
            Timestamp = timestamp;
            Priority = priority;
            Sender = sender;
            Reported = reported;
            Moderator = null;
            Issue = issue;
            Room = room;
            Answered = false;
            ReportedChats = reportedChats;
        }

        public int GetStatus(int Id)
        {
            if (Moderator == null)
                return 1;
            else if (Moderator.Id == Id && !Answered)
                return 2;
            else if (Answered)
                return 3;
            else
                return 3;
        }
    }
}
