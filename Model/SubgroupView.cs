using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
    public class SubgroupView
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public string Kinds { get; set; }
        public string GroupStud { get; set; }
        public string SubjectName { get; set; }
        public List<Subgroup> Subgroups { get; set; }
    }

    public class Subgroup
    {
        public string NumSubgroup { get; set; }
        public List<Student> Students { get; set; }
    }
	
    public class Student
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string FIO { get; set; }
        public  string Group { get; set; }
        public string UserName { get; set; }
    }
}