using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic.CompilerServices;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/student")]
    [ApiController]
    public class StudentController : ControllerBase
    {

        [HttpGet]
        public IActionResult GetStudents()
        {
            var db = new s16985Context();

            var res = db.Student.ToList();

            return Ok(res);
        }


        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            var db = new s16985Context();
            student.Salt = "VDIPYS6X9MSB8EPMNZTB";
            db.Student.Add(student);

            db.SaveChanges();

            return Ok("Poprawnie dodano studenta: " + student.IndexNumber);
        }

        [HttpPut("update/{id}")]
        public IActionResult UpDateStudent(string id, Student student)
        {
            var db = new s16985Context();

            var st = db.Student.Where(stud => stud.IndexNumber.Equals(id)).First();

            if (student.FirstName != null) {
                st.FirstName = student.FirstName;
            }
            if (student.LastName != null)
            {
                st.LastName = student.LastName;
            }
            if (student.BirthDate != null)
            {
                st.BirthDate = student.BirthDate;
            }
            if (student.IndexNumber != null)
            {
                st.IndexNumber = student.IndexNumber;
            }


            db.SaveChanges();
            return Ok("Aktualizacja dokończona");
        }

        [HttpDelete("delete/{id}")]
        public IActionResult DeleteStudent(string id)
        {
            var db = new s16985Context();

            var st = db.Student.Where(stud => stud.IndexNumber.Equals(id)).First();

           // db.Attach(st);
            db.Remove(st);
            db.SaveChanges();
            return Ok("Usuwanie ukończone");
        }
    }
}