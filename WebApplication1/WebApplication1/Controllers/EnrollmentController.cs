using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTOs.Requests;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/enrollment")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {

        [HttpPost]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            int semestr = 1;
            var db = new s16985Context();
            int idStudy;
            int idEnroll;
            try
            {
                idStudy = db.Studies.Where(study => study.Name.Equals(request.Studies)).Select(s => s.IdStudy).First();
            }
            catch(Exception e)
            {
                return BadRequest("Podane studia nie istnieją!");
            }

            try
            {
                idEnroll = db.Enrollment.Where(idEnroll => idEnroll.Semester.Equals(semestr) && idEnroll.IdStudy.Equals(idStudy)).Select(e => e.IdEnrollment).First();
            }
            catch (Exception e)
            {
                var enrollment = new Enrollment();
                idEnroll = db.Enrollment.Max(e => e.IdEnrollment) + 1;
                enrollment.IdEnrollment = idEnroll;
                enrollment.IdStudy = idStudy;
                enrollment.Semester = semestr;
                enrollment.StartDate = DateTime.Today;

                db.Enrollment.Add(enrollment);
            }
            


            if(db.Student.Where(s => s.IndexNumber.Equals(request.IndexNumber)).Count() > 0)
            {
                return BadRequest("Podany numer indeksu już istnieje!");
            }

            Student student = new Student();
            student.IndexNumber = request.IndexNumber;
            student.FirstName = request.FirstName;
            student.LastName = request.LastName;
            student.BirthDate = request.BirthDate;
            student.IdEnrollment = idEnroll;
            student.Salt = "VDIPYS6X9MSB8EPMNZTB";

            db.Student.Add(student);
            db.SaveChanges();
            return Ok("Pomyślnie dodano studenta");
        }

        [HttpPost("promotions")]
        public IActionResult PromoteStudents(PromoteStudentRequest request)
        {
            int idStudy;
            int semester;
            var db = new s16985Context();
            int idEnrollOld;
            int idEnrollNew;

            try
            {
                idStudy = db.Studies.Where(st => st.Name.Equals(request.Studies)).Select(st => st.IdStudy).First();
                semester = db.Enrollment.Where(enroll => enroll.IdStudy.Equals(idStudy)).Select(enroll => enroll.Semester).First();
                if (!semester.Equals(request.Semester))
                {
                    return BadRequest("Semestr tych studiów nie istnieje!");
                }
            }
            catch (Exception e)
            {
                return BadRequest("Podane studia nie istnieją!");
            }

            try
            {
                idEnrollNew = db.Enrollment.Where(enroll => enroll.Semester.Equals(semester + 1) && enroll.IdStudy.Equals(idStudy)).Select(e => e.IdEnrollment).First();
            }
            catch (Exception e)
            {
                var enrollment = new Enrollment();
                idEnrollNew = db.Enrollment.Max(e => e.IdEnrollment) + 1;
                enrollment.IdEnrollment = idEnrollNew;
                enrollment.IdStudy = idStudy;
                enrollment.Semester = semester + 1;
                enrollment.StartDate = DateTime.Today;

                db.Enrollment.Add(enrollment);
            }
            idEnrollOld = db.Enrollment.Where(enroll => enroll.IdStudy.Equals(idStudy) && enroll.Semester.Equals(semester)).Select(enroll => enroll.IdEnrollment).First();

            foreach(Student student in db.Student){
                if (student.IdEnrollment.Equals(idEnrollOld))
                {
                    student.IdEnrollment = idEnrollNew;
                }
            }

            return Ok("Promocja zakończona sukcesem");
        }

    }
}