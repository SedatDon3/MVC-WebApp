using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Linq;
using WebApp.Models;
using MongoDB.Bson;

namespace WebApp.Controllers
{
    [Authorize(Roles="Преподаватель")]
    public class ScheduleController : Controller
    {
        DBOppt db = new DBOppt();
        [HttpGet]
        [Authorize(Roles = "Администратор")]
        public ActionResult ExistWeeks()
        {

            if (db.ExistWeeks() != 0)
            {
                ViewData["Ex"] = "Недели уже определены";
                Week w = db.GetWeek();
                return View(w);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Администратор")]
        public ActionResult SetWeeks(List<int> week, List<string> day, List<int> lec, string st)
        {
            CultureInfo cul = CultureInfo.CurrentCulture;
            DateTime d = Convert.ToDateTime(st);
            db.SetWeeks(d, week, day, lec);
            return RedirectToAction("ExistWeeks");
        }

        [HttpGet]
        [Authorize(Roles = "Администратор")]
        public ActionResult DelWeeks()
        {
            db.DelWeeks();
            return RedirectToAction("ExistWeeks");
        }

        public ActionResult Index(int? week)
        {
            ScheduleContext sc = new ScheduleContext();
            ScheduleView scv = sc.GetSchedule(User.Identity.Name);
            for (int i = 0; i < 17; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (scv.Weeks[i].Days[j].Subjects != null && scv.Weeks[i].Days[j].Subjects.Count > 1)
                    {
                        scv.Weeks[i].Days[j].Subjects.Sort(delegate (Subject sub1, Subject sub2) { return sub1.TimeStart.CompareTo(sub2.TimeStart); });
                    }
                };
            };
            if (week.HasValue)
            {
                TempData["w"] = week;
                TempData.Keep("w");
            }
            return PartialView(scv);
        }

        public PartialViewResult AddSubj(int w, int d, string usern)
        {
            TempData["w"] = w;
            TempData.Keep("w");
            TempData["d"] = d;
            TempData.Keep("d");
            TempData["username"] = usern;
            TempData.Keep("username");
            if (d == -1) d = 0;
            List<string> TimeDateSt = new List<string>();
            List<string> TimeDateEn = new List<string>();
            ScheduleContext sc = new ScheduleContext();
            ScheduleView scv = sc.GetSchedule(usern);            
            if (scv.Weeks[w].Days[d].Subjects != null)
            {
                foreach (var t in scv.Weeks[w].Days[d].Subjects)
                {
                    TimeDateSt.Add(t.TimeStart.ToLocalTime().ToShortTimeString());
                    TimeDateEn.Add(t.TimeEnd.ToLocalTime().ToShortTimeString());
                };
            };
            ViewBag.TimeSt = TimeDateSt;
            ViewBag.TimeEn = TimeDateEn;

            Day da = new Day();
            return PartialView("_AddSubjForm", da);
        }
		
        [Authorize]
        [HttpPost]
        public ActionResult AddSubject(Day model, string[] check, string[] dayweek)
        {
            if (ModelState.IsValid)
            {
                Subject sub = model.Subjects[0];
                ScheduleContext sc = new ScheduleContext();
                ScheduleView scv = sc.GetSchedule(User.Identity.Name);
                for (int i = 0, j = 0; i < scv.Weeks.Count; i++)
                {
                    if ((j < check.Length) && (scv.Weeks[i].WeekNum.Equals(check[j])))
                    {
                        for (int k = 0; k < dayweek.Length; k++)
                        {
                            var n = Convert.ToInt32(dayweek[k]);
                            if (scv.Weeks[i].Days[n].Subjects == null)
                                scv.Weeks[i].Days[n].Subjects = new List<Subject>();
                            DateTime ts = new DateTime(scv.Weeks[i].Days[n].Date.Year, scv.Weeks[i].Days[n].Date.Month, scv.Weeks[i].Days[n].Date.Day, sub.TimeStart.Hour, sub.TimeStart.Minute, sub.TimeStart.Second);
                            sub.TimeStart = ts;
                            DateTime te = new DateTime(scv.Weeks[i].Days[n].Date.Year, scv.Weeks[i].Days[n].Date.Month, scv.Weeks[i].Days[n].Date.Day, sub.TimeEnd.Hour, sub.TimeEnd.Minute, sub.TimeEnd.Second);
                            sub.TimeEnd = te;
                            if (scv.Weeks[i].Days[n].Celebr)
                            {
                                sub.Cancel = true;
                            }
                            else sub.Cancel = false;
                            scv.Weeks[i].Days[n].Subjects.Add(sub);
                            j++;
                        };
                    };
                };
                sc.AddSubject(scv, User.Identity.Name);
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", "Что-то пошло не так");
            return RedirectToAction("Index", "Home");
        }
		
        [Authorize]
        [HttpPost]
        public PartialViewResult ChaSubject(int w, int d, int s)
        {
            Subject sub = new Subject();
            ScheduleContext sc = new ScheduleContext();
            ScheduleView scv = sc.GetSchedule(User.Identity.Name);
            List<string> TimeDateSt = new List<string>();
            List<string> TimeDateEn = new List<string>();
            if (scv.Weeks[w].Days[d].Subjects != null)
            {
                foreach (var t in scv.Weeks[w].Days[d].Subjects)
                {
                    TimeDateSt.Add(t.TimeStart.ToLocalTime().ToShortTimeString());
                    TimeDateEn.Add(t.TimeEnd.ToLocalTime().ToShortTimeString());
                };
            };
            ViewBag.TimeSt = TimeDateSt;
            ViewBag.TimeEn = TimeDateEn;
            sub = scv.Weeks[w].Days[d].Subjects[s];
            TempData["d"] = d;
            TempData.Keep("d");
            TempData["s"] = s;
            TempData.Keep("s");
            TempData["w"] = w;
            TempData.Keep("w");
            return PartialView("_ChaSubjForm", sub);
        }
		
        [Authorize]
        public ActionResult ChanSubject(Subject sub, int d, int w, int s)
        {
            if (ModelState.IsValid)
            {
                ScheduleContext sc = new ScheduleContext();
                ScheduleView scv = sc.GetSchedule(User.Identity.Name);
                sub.TimeStart = sub.TimeStart.ToUniversalTime();
                sub.TimeEnd = sub.TimeEnd.ToUniversalTime();
                scv.Weeks[w].Days[d].Subjects[s] = sub;
                sc.AddSubject(scv, User.Identity.Name);
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", "Что-то пошло не так");
            return RedirectToAction("Index", "Home");
        }
		
        [Authorize]
        public void DelSubject(int w, int d, int s)
        {
            ScheduleContext sc = new ScheduleContext();
            ScheduleView scv = sc.GetSchedule(User.Identity.Name);
            scv.Weeks[w].Days[d].Subjects.RemoveAt(s);
            sc.AddSubject(scv, User.Identity.Name);
        }
		
        [Authorize]
        public void DelSubjects(int w, int d)
        {
            ScheduleContext sc = new ScheduleContext();
            ScheduleView scv = sc.GetSchedule(User.Identity.Name);
            scv.Weeks[w].Days[d].Subjects.Clear();
            sc.AddSubject(scv, User.Identity.Name);
        }
		
        [Authorize]
        public void ClearAll()
        {
            ScheduleContext sc = new ScheduleContext();
            ScheduleView scv = sc.GetSchedule(User.Identity.Name);
            for (int i = 0; i < 17; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (scv.Weeks[i].Days[j].Subjects != null)
                    {
                        scv.Weeks[i].Days[j].Subjects.Clear();
                    }
                };
            };
            sc.SaveSche(scv, User.Identity.Name);
        }
		
        [HttpGet]
        [Authorize]
        public ActionResult CreateSubGr()
        {
            ScheduleContext sc = new ScheduleContext();
            List<EditUserViewModel> listst = sc.GetStudents();
            var studGroups = from stud in listst group stud by stud.Group;
            List<string> groups = new List<string>();
            foreach (IGrouping<string, EditUserViewModel> g in studGroups)
            {
                groups.Add(g.Key);
            }
            ViewData["ListStud"] = new SelectList(groups);
            return View("SubGroupView");
        }
		
        [HttpPost]
        public PartialViewResult CreateSubGr(int num, string sel)
        {
            ScheduleContext sc = new ScheduleContext();
            List<EditUserViewModel> listst = sc.GetStudentsGroup(sel);
            List<string> stud = new List<string>();
            foreach(var y in listst)
            {
                stud.Add(y.FIO);
            }
            ViewData["num"] = num;
            TempData["lststud"] = listst;
            TempData.Keep("lststud");
            TempData["group"] = sel;
            TempData.Keep("group");
            ViewData["ListSt[]"] = new SelectList(stud);
            return PartialView("ListStudView");
        }
		
        public ActionResult SaveSubGr(string[] list1, string[] list2, string[] list3, string dis)
        {
            List<EditUserViewModel> listst = (List <EditUserViewModel>) TempData["lststud"];
            SubgroupView sub = new SubgroupView();
            sub.Id = ObjectId.GenerateNewId();
            sub.GroupStud = (string)TempData["group"];
            sub.Kinds = "subgroup";
            sub.SubjectName = dis;
            List<Subgroup> sg = new List<Subgroup>();
            int g = 0;
            if (list1 != null)
                g++;
            if (list2 != null)
                g++;
            if (list3 != null)
                g++;
            for(int i = 0; i < g; i++)
            {
                Subgroup subgr = new Subgroup();
                subgr.NumSubgroup = (i + 1) + "";
                List<Student> ls = new List<Student>();
                if (list1 != null && i == 0)
                {
                    for(int j = 0; j < list1.Count(); j++)
                    {
                        Student stud = new Student();
                        EditUserViewModel euv = listst.Find(t => t.FIO == list1[j]);
                        if (euv!=null) 
                        {
                            stud.FIO = list1[j];
                            stud.Group = euv.Group;
                            stud.UserName = euv.UserName;
                            stud.Id = euv.Id;
                            ls.Add(stud);
                        }
                    }                    
                }
                if (list2 != null && i == 1)
                {
                    for (int j = 0; j < list2.Count(); j++)
                    {
                        Student stud = new Student();
                        EditUserViewModel euv = listst.Find(t => t.FIO == list2[j]);
                        if (euv != null)
                        {
                            stud.FIO = list2[j];
                            stud.Group = euv.Group;
                            stud.UserName = euv.UserName;
                            stud.Id = euv.Id;
                            ls.Add(stud);
                        }
                    }
                }
                if (list3 != null && i == 2)
                {
                    for (int j = 0; j < list3.Count(); j++)
                    {
                        Student stud = new Student();
                        EditUserViewModel euv = listst.Find(t => t.FIO == list3[j]);
                        if (euv != null)
                        {
                            stud.FIO = list3[j];
                            stud.Group = euv.Group;
                            stud.UserName = euv.UserName;
                            stud.Id = euv.Id;
                            ls.Add(stud);
                        }
                    }
                }
                subgr.Students = ls;
                sg.Add(subgr);
            }
            sub.Subgroups = sg;
            ScheduleContext sc = new ScheduleContext();
            sc.SaveSubgroups(sub, User.Identity.Name);
            return RedirectToAction("CreateSubGr");
        }
		
        [Authorize]
        public PartialViewResult FormGrid(int w, int d, int s)
        {
            ScheduleContext sc = new ScheduleContext();
            ScheduleView scv = sc.GetSchedule(User.Identity.Name);
            string coursnum = scv.Weeks[w].Days[d].Subjects[s].GroupStud;
            string gc = "";
            foreach (var g in coursnum)
            {
                if (Char.IsDigit(g))
                {
                    gc = g + " курс";
                    break;
                }
            };
            Grid gr = new Grid()
            {
                Course = gc,
                Group = scv.Weeks[w].Days[d].Subjects[s].GroupStud,
                KindOfSubject = scv.Weeks[w].Days[d].Subjects[s].KindOfSubject,
                SubjectName = scv.Weeks[w].Days[d].Subjects[s].SubjectName,
                UserName = User.Identity.Name,
                Date = scv.Weeks[w].Days[d].Date,
                Audit = scv.Weeks[w].Days[d].Subjects[s].Audit,
                TimeStart = scv.Weeks[w].Days[d].Subjects[s].TimeStart,
                TimeEnd = scv.Weeks[w].Days[d].Subjects[s].TimeEnd,
            };            
            return PartialView("_SetGrid", gr);
        }
        [Authorize]
        public ActionResult ExistSubgroup(string group, string subname)
        {
            ScheduleContext sc = new ScheduleContext();
            bool t = sc.ExistSubg(group, subname, User.Identity.Name);
            return Json(new { returnvalue = t });
        }

        [Authorize]
        public async Task<ActionResult> CreateGrid(string[] marks, string[] subgro, string[] prot)
        {
            ScheduleContext sc = new ScheduleContext();
            Grid gr = (Grid)TempData["Mod"];
            NapravContext nc = new NapravContext();
            string group = gr.Group;
            string g = "";
            bool pun = false;
            int numsem = 0;
            int numgr = 0;
            int countsub = 0;
            int countsub1 = 0;
            int countsub2 = 0;
            int tekcount = 0;
            int su = 0;
            Marks m = new Marks();
            for (int i = 0; i < marks.Count(); i++)
            {
                if (marks[i] == "1")
                {
                    m.M1 = true;
                }
                if (marks[i] == "2")
                {
                    if (gr.KindOfSubject == "Лекция")
                    {
                        m.M2 = "";
                    }
                    else
                    {
                        m.M6 = true;
                        m.M7 = "";
                    }                                       
                }
                if(gr.KindOfSubject == "Практическое занятие")
                {
                    if (marks[i] == "3")
                    {
                        m.M5 = "";
                    }
                    if (marks[i] == "4")
                    {
                        m.M8 = true;
                    }
                }
                              
            }
            for(int i = 0; i < prot.Count(); i++)
            {
                if (gr.KindOfSubject == "Лабораторная работа")
                {
                    if (prot[i] == "6")
                    {
                        m.M3 = "";
                        break;
                    }
                    if (prot[i] == "7")
                    {
                        m.M4 = true;
                        break;
                    }
                }
            }
            if (subgro != null)
            {
                if (subgro[0] == "3")
                    su = 1;
                else if (subgro[0] == "4")
                    su = 2;
                else su = 3;
                gr.subgr = su + " подгруппа";
            }
            
            for (int i = 0; i < group.Count(); i++)
            {
                if (Char.IsPunctuation(group[i]))
                {
                    pun = true;
                    if (Char.IsLetter(group[i + 1]))
                    {
                        g += group[i + 1];
                    }
                }
                else if(!pun)
                {
                    g += group[i];
                }
                if (Char.IsNumber(group[i]))
                {
                    numgr = Convert.ToInt32(group[i].ToString());
                    if (gr.Date.Month <= 6)
                    {
                        numsem = numgr * 2;
                    }
                    else
                    {
                        numsem = numgr * 2 - 1;
                    }
                    break;
                }
            }
            gr.Semestr = numsem + " семестр";
            string coll = nc.GetNameColl(g);
            List<Disci> d = (List <Disci>)await nc.GetDis(coll, gr.SubjectName);
            RabProg rp = d[0].rp;
            Dictionary<int, DateTime> numDate = new Dictionary<int, DateTime>();
            ScheduleView scv = sc.GetSchedule(User.Identity.Name);
            bool se = false; bool s1s2 = false;
            foreach(var w in scv.Weeks)
            {
                foreach(var day in w.Days)
                {
                    if (day.Subjects != null && day.Subjects.Count != 0)
                    {
                        foreach (var sub in day.Subjects)
                        {
                            if (!sub.Cancel && sub.SubjectName == gr.SubjectName && sub.KindOfSubject == gr.KindOfSubject)
                            {
                                if (gr.KindOfSubject == "Лабораторная работа")
                                {
                                    if (sub.SubGroup[0] && sub.SubGroup[1])
                                    {
                                        s1s2 = true;
                                    }
                                    if (sub.SubGroup[0])
                                    {
                                        countsub++;
                                    }
                                    if (sub.SubGroup[1])
                                    {
                                        countsub1++;
                                    }
                                    if (sub.SubGroup[2])
                                    {
                                        countsub2++;
                                    }
                                }
                                else
                                    countsub++;
                            }
                        }
                    }
                    if (se=(day.Date == gr.Date))
                    {
                        if (su == 2)
                        {
                            tekcount = countsub1;
                        }
                        else if (su == 3)
                        {
                            tekcount = countsub2;
                        } else tekcount = countsub;
                        break;
                    }
                }
                if (se) break;
            }
            if (s1s2 && su!=3)
            {
                tekcount /= 2;
            }
            List<string> Them = new List<string>();
            foreach (var s in rp.Sect)
            {
                if (s.NumSem == numsem.ToString())
                {
                    foreach (var s1 in s.Sections)
                    {
                        if (gr.KindOfSubject == "Лабораторная работа")
                        {
                            foreach(var s2 in s1.SectLabs)
                            {
                                if (s2.LaborLabClass > 4)
                                {
                                    for (int i = 0; i < (s2.LaborLabClass / 4); i++)
                                    {
                                        Them.Add(s2.NameLabW);
                                    }
                                }
                                else Them.Add(s2.NameLabW);
                            }
                        }
                        else if (gr.KindOfSubject == "Практическое занятие")
                        {
                            foreach (var s2 in s1.SectPracs)
                            {
                                if (s2.LaborPracClass > 2)
                                {
                                    for (int i = 0; i < (s2.LaborPracClass / 2); i++)
                                    {
                                        Them.Add(s2.NamePracW);
                                    }
                                }
                                else Them.Add(s2.NamePracW);
                            }
                        }
                        else
                        {
                            foreach (var s2 in s1.SectLecs)
                            {
                                if (s2.LaborLecClass > 2)
                                {
                                    for (int i = 0; i < (s2.LaborLecClass / 2); i++)
                                    {
                                        Them.Add(s2.NameTheme);
                                    }
                                }
                                else Them.Add(s2.NameTheme);
                            }
                        }
                    }
                }
            }
            if (tekcount <= Them.Count)
            {
                gr.Theme = Them[tekcount - 1];
            }
            else gr.Theme = "";            
            if(gr.KindOfSubject=="Лабораторная работа")
            {
                List<SubgroupView> ls = sc.GetSubgroup(gr.Group, gr.SubjectName, User.Identity.Name);
                gr.StudMarks = new List<StudMarks>();
                foreach (var l in ls)
                {
                    foreach(var s in l.Subgroups)
                    {
                        if (su.ToString() == s.NumSubgroup)
                        {
                            foreach(var k in s.Students)
                            {
                                StudMarks sm = new StudMarks()
                                {
                                    IdStud = k.Id,
                                    FIO = k.FIO,
                                    UserNameStud = k.UserName,
                                    marks = m,
                                    listMarks = new Marks(),
                                    Comment = ""
                                };
                                gr.StudMarks.Add(sm);
                            }
                        }
                    }                    
                }
            }
            else
            {
                List<EditUserViewModel> stud = sc.GetStudentsGroup(gr.Group);
                gr.StudMarks = new List<StudMarks>();
                foreach(var s in stud)
                {
                    StudMarks sm = new StudMarks()
                    {
                        IdStud = s.Id,
                        FIO = s.FIO,
                        UserNameStud = s.UserName,
                        marks = m,
                        listMarks = new Marks(),
                        Comment = ""
                    };
                    gr.StudMarks.Add(sm);
                }
            }
            Grid kfk = (Grid)ViewData.Model;
            return View("Grid", gr);
        }
    }
}