using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;


namespace WebApp.Models
{
    public class Grid
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [Display(Name ="Наименование дисциплины")]
        public string SubjectName { get; set; }
        [Display(Name = "Вид занятия")]
        public string KindOfSubject { get; set; }
        [Display(Name = "Курс")]
        public string Course { get; set; }
        [Display(Name = "Семестр")]
        public string Semestr { get; set; }
        [Display(Name = "Учебная группа")]
        public string Group { get; set; }
        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }
        [Display(Name = "Дата")]
        public DateTime Date { get; set; }
        [Display(Name = "Аудитория")]
        public string Audit { get; set; }
        [BsonRepresentation(BsonType.DateTime)]
        [Display(Name = "Время начала")]
        public DateTime TimeStart { get; set; }
        [BsonRepresentation(BsonType.DateTime)]
        [Display(Name = "Время окончания")]
        public DateTime TimeEnd { get; set; }
        public List<StudMarks> StudMarks { get; set; }
        [BsonDefaultValue("grids")]
        public string Kinds { get; set; }
        [Display(Name = "Тема")]
        public string Theme { get; set; }
        [Display(Name = "Задание")]
        public string Zadanie { get; set; }
        [BsonIgnoreIfNull]
        [Display(Name = "Подгруппа")]
        public string subgr { get; set; }
    }

    public class StudMarks
    {
        public string FIO { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string IdStud { get; set; }
        public string UserNameStud { get; set; }
        public Marks marks { get; set; }
        public Marks listMarks { get; set; }
        public string Comment { get; set; }
    }

    public class Marks
    {
        [Display(Name ="Посещаемость")]
        public bool M1 { get; set; }
        [BsonIgnoreIfNull]
        [Display(Name = "Работа на лекции")]
        public string M2 { get; set; }
        [Display(Name = "Защита отчета с оценкой")]
        [BsonIgnoreIfNull]
        public string M3 { get; set; }
        [Display(Name = "Защита отчета без оценки")]
        [BsonIgnoreIfNull]
        public bool M4 { get; set; }
        [Display(Name = "Оценка")]
        [BsonIgnoreIfNull]
        public string M5 { get; set; }
        [Display(Name = "Доклад")]
        [BsonIgnoreIfNull]
        public bool M6 { get; set; }
        [Display(Name = "Тема доклада")]
        [BsonIgnoreIfNull]
        public string M7 { get; set; }
        [Display(Name = "Выполнение задания")]
        [BsonIgnoreIfNull]
        public bool M8 { get; set; }
    }
}