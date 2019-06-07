using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace WebApp.Models
{
    public class ScheduleContext
    {
        IMongoDatabase database;
        IMongoDatabase database2;
        IMongoDatabase database3;

        public ScheduleContext()
        {
            string conStr = "mongodb://localhost/ucheba";
            var conn = new MongoUrlBuilder(conStr);
            MongoClient client = new MongoClient(conStr);
            database = client.GetDatabase("schedulesTeach");
            database2 = client.GetDatabase("Users");
            database3 = client.GetDatabase("Grids");
        }

        public ScheduleView GetSchedule(string UserName)
        {
            IMongoCollection<ScheduleView> schedule = database.GetCollection<ScheduleView>(UserName);
            var t = schedule.Find(r => r.Kinds == "schedule").ToList();
            return t.First();
        }

        public void AddSubject(ScheduleView sche, string UserName)
        {
            IMongoCollection<ScheduleView> schedule = database.GetCollection<ScheduleView>(UserName);
            for (int i = 0; i < 17; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (sche.Weeks[i].Days[j].Subjects != null)
                    {
                        for (int k = 0; k < sche.Weeks[i].Days[j].Subjects.Count; k++)
                        {
                            sche.Weeks[i].Days[j].Subjects[k].TimeStart.ToLocalTime();
                            sche.Weeks[i].Days[j].Subjects[k].TimeEnd.ToLocalTime();
                            
                        }
                        if (sche.Weeks[i].Days[j].Subjects.Count > 1)
                        {
                            sche.Weeks[i].Days[j].Subjects.Sort(delegate (Subject sub1, Subject sub2) { return sub1.TimeStart.CompareTo(sub2.TimeStart); });
                        }
                    }
                };
            };
            schedule.FindOneAndReplace(t => t.Id == sche.Id, sche); 
        }
		
        public void SaveSche(ScheduleView sche, string UserName)
        {
            IMongoCollection<ScheduleView> schedule = database.GetCollection<ScheduleView>(UserName);
            schedule.FindOneAndReplace(t => t.Id == sche.Id, sche);
        }

        public List<EditUserViewModel> GetStudents()
        {
            IMongoCollection<EditUserViewModel> ListSt = database2.GetCollection<EditUserViewModel>("Students");
            return ListSt.Find(_ => true).ToList();
        }
		
        public List<EditUserViewModel> GetStudentsGroup(string group)
        {
            IMongoCollection<EditUserViewModel> ListSt = database2.GetCollection<EditUserViewModel>("Students");
            return ListSt.Find(t => t.Group == group).ToList(); 
        }
		
        public void SaveSubgroups(SubgroupView subgroups, string usern)
        {
            IMongoCollection<SubgroupView> ListSt = database.GetCollection<SubgroupView>(usern);
            UpdateOptions options = new UpdateOptions
            {
                IsUpsert = true
            };
            ListSt.ReplaceOne((t=>t.GroupStud==subgroups.GroupStud&&t.SubjectName==subgroups.SubjectName), subgroups, options);
        }
		
        public bool ExistSubg(string group, string subname, string usern)
        {
            IMongoCollection<SubgroupView> ListSt = database.GetCollection<SubgroupView>(usern);
            List<SubgroupView> ls = ListSt.Find((t => t.GroupStud == group && t.SubjectName == subname)).ToList();
            if (ls.Count != 0)
            {
                return true;
            }
            else return false;
        }

        public List<SubgroupView> GetSubgroup(string group, string subname, string usern)
        {
            IMongoCollection<SubgroupView> ListSt = database.GetCollection<SubgroupView>(usern);
            return ListSt.Find((t => t.GroupStud == group && t.SubjectName == subname)).ToList();
        }
    }
}
