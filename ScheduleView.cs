using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class ScheduleView
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public List<Week> Weeks { get; set; }
        public string Kinds { get; set; }
    }
	
    public class Week
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DateStart { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DateEnd { get; set; }
        public string WeekNum { get; set; }
        public List<Day> Days { get; set; }
        public bool LecWeek { get; set; }
    }
	
    public class Day
    {
        public string DayName { get; set; }
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime Date { get; set; }
        [BsonIgnoreIfNull]
        public bool Celebr { get; set; }
        [BsonIgnoreIfNull]
        public List<Subject> Subjects { get; set; }
    }
	
    public class Subject
    {
        public Subject()
        {
            SubGroup = new List<bool>() { false, false, false };
        }
        [Display(Name = "Наименование дисциплины")]
        public string SubjectName { get; set; }
        [Display(Name = "Учебная группа")]
        public string GroupStud { get; set; }
        [Display(Name = "Вид занятия")]
        public string KindOfSubject { get; set; }
        [Display(Name = "Подгруппы")]
        [BsonIgnoreIfNull]
        public List<bool> SubGroup { get; set; }

        [Display(Name = "Аудитория")]
        public string Audit { get; set; }
        [BsonRepresentation(BsonType.DateTime)]
        [Display(Name = "Время начала")]
        public DateTime TimeStart { get; set; }
        [BsonRepresentation(BsonType.DateTime)]
        [Display(Name = "Время окончания")]
        public DateTime TimeEnd { get; set; }
        [Display(Name = "Отменено")]
        public bool Cancel { get; set; }        
    }
}