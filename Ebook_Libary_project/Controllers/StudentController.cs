using Lab3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lab3.Controllers
{
    public class StudentController : Controller
    {
        // GET: Student
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ShowStudent()
        {
            Student student = new Student
            {
                FirstName = "Jane",
                LastName = "Doe",
                Phone = "0521234567",
                Email = "JaneDoe@gmail.com"
            };
            return View("student", student);
        }

        public ActionResult AddStudent()
        {
            return View();
        }

        public ActionResult SubmitStudentDetails(Student obj)
        {
            return View("student", obj);
        }
    }
}