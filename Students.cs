using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Stud
{
    class Program
    {
        static void Main(string[] args)
        {
            string yesNo = string.Empty;
            do
            {
                yesNo = string.Empty;
                ShowOption();
                Console.WriteLine("Kindly enter the option:");
                string strOpt = Console.ReadLine();
                Option option = (Option)Enum.Parse(typeof(Option), strOpt);
                switch (option)
                {
                    case Option.ListAll:
                        ListAll();
                        break;
                    case Option.ListSelected:
                        ListSelected();
                        break;
                    case Option.MoveStudent:
                        MoveStudent();
                        break;
                    case Option.AddStudent:
                        AddStudent();
                        break;
                    default:
                        Console.WriteLine("Exit");
                        return;
                }
                Console.WriteLine("Do you want to continue (y/n):");
                yesNo = Console.ReadLine();
            } while (yesNo.ToUpper() == "Y");

        }

        private static void AddStudent()
        {
            Console.WriteLine("Kindly enter the name of the student:");
            string newstudentName = Console.ReadLine();
            int newStudId = DBData.Reference.AddStudent(newstudentName);
            Console.WriteLine("Kindly enter the numbers of courses want to join:");
            int noofCourses = Convert.ToInt32(Console.ReadLine());
             
            for (int i = 0; i < noofCourses; i++)
            {
                Console.WriteLine("Kindly enter the courseid:");
                int courseId = Convert.ToInt32(Console.ReadLine());
                DBData.Reference.AddStudentCourse(newStudId, courseId);
            }
        }


        private static void ShowOption()
        {
            Console.WriteLine("1. List of all Students, Courses and Professor");
            Console.WriteLine("2. List of all Students through ProfessorId");
            Console.WriteLine("3. Move Courses");
            Console.WriteLine("4. Add Student");
            Console.WriteLine("5. Exit");
        }

        static void ListAll()
        {
            var studs = DBData.Reference.GetAllStudent();
            if (studs.Length == 0)
            {
                Console.WriteLine("No students available.");
            }
            else
            {
                Console.WriteLine("StudentId\t\tStudent Name");
                Console.WriteLine("---------\t\t------------");
                for (int i = 0; i < studs.Length; i++)
                {
                    Console.WriteLine(studs[i].StudentId + "\t\t" + studs[i].Name);
                }
            }
            var cources = DBData.Reference.GetAllCourses();
            if (cources.Length == 0)
            {
                Console.WriteLine("No cources available.");
            }
            else
            {
                Console.WriteLine("CoursesId\t\tCourses Name");
                Console.WriteLine("---------\t\t------------");
                for (int i = 0; i < cources.Length; i++)
                {
                    Console.WriteLine(cources[i].CoursesId + "\t\t" + cources[i].Name);
                }
            }
            var professor = DBData.Reference.GetAllProfessor();
            if (professor.Length == 0)
            {
                Console.WriteLine("No Professor available.");
            }
            else
            {
                Console.WriteLine("Professor Name\t\tCourses Id");
                Console.WriteLine("--------------\t\t----------");
                for (int i = 0; i < professor.Length; i++)
                {
                    Console.WriteLine(professor[i].Name + "\t\t" + professor[i].CoursesId);
                }
            }
        }

        static void ListSelected()
        {
            Console.WriteLine("Kindly enter the Professor Name:");
            string strOpt = Console.ReadLine();
            var studs = DBData.Reference.GetStudents(strOpt);
            if (studs.Length == 0)
            {
                Console.WriteLine("No students available of professor " + strOpt);
            }
            else
            {
                Console.WriteLine("StudentId\t\tStudent Name");
                Console.WriteLine("---------\t\t------------");
                for (int i = 0; i < studs.Length; i++)
                {
                    Console.WriteLine(studs[i].StudentId + "\t\t" + studs[i].Name);
                }
            }
        }

        static void MoveStudent()
        {
            Console.WriteLine("Enter the StudentId:");
            int studentId = Convert.ToInt32(Console.ReadLine());
            var studCources = DBData.Reference.GetStudentEnroll(studentId);
            if (studCources.Length == 0)
            {
                Console.WriteLine("No cources mapped.");
            }
            else
            {
                Console.WriteLine("StudentId\t\tStudent Name\t\tCoursesId\t\tCourses Name");
                Console.WriteLine("---------\t\t------------\t\t---------\t\t------------");
                for (int i = 0; i < studCources.Length; i++)
                {
                    Console.WriteLine(studCources[i].StudentData.StudentId + "\t\t" + studCources[i].StudentData.Name + "\t\t" +
                        studCources[i].CoursesId + "\t\t" + studCources[i].CoursesName);
                }
                Console.WriteLine("Enter new Course Id:");
                int courseId = Convert.ToInt32(Console.ReadLine());
                bool retVal = DBData.Reference.UpdateCourse(studentId, courseId);
                if (retVal)
                    Console.WriteLine("Mapped Sucesfully");
                else
                    Console.WriteLine("Problem in Mapping");
            }
        }
    }

    enum Option
    {
        ListAll = 1,
        ListSelected,
        MoveStudent,
        AddStudent
    }

    class DBData
    {
        static DBData reference = new DBData();

        List<Student> lstStudent = new List<Student>();
        List<Professor> lstProfessor = new List<Professor>();
        List<Courses> lstCourses = new List<Courses>();

        private DBData()
        {
            LoadStudents();
            LoadProfessor();
            LoadCourses();
        }

        public static DBData Reference
        {
            get { return reference; }
        }

        void LoadStudents()
        {
            try
            {
                SqlConnection con = new SqlConnection("Data Source=192.168.81.29,5633;Initial Catalog=TestDB;User ID=sa;Password=auto$tgr56@33");
                con.Open();
                SqlCommand cmd = new SqlCommand("Select * from Student", con);
                cmd.CommandType = System.Data.CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();

                Student student = null;
                while (dr.Read())
                {
                    student = new Student();
                    student.StudentId = dr.GetInt32(dr.GetOrdinal("StudentId"));
                    student.Name = dr.GetString(dr.GetOrdinal("StudentName"));
                    lstStudent.Add(student);
                }
                con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }
        void LoadProfessor()
        {
            SqlConnection con = new SqlConnection("Data Source=192.168.81.29,5633;Initial Catalog=TestDB;User ID=sa;Password=auto$tgr56@33");
            con.Open();
            SqlCommand cmd = new SqlCommand("Select * from Professors", con);
            cmd.CommandType = System.Data.CommandType.Text;
            SqlDataReader dr = cmd.ExecuteReader();

            Professor professor = null;
            while (dr.Read())
            {
                professor = new Professor();
                professor.Name = dr.GetString(dr.GetOrdinal("ProfessorName"));
                professor.CoursesId = dr.GetInt32(dr.GetOrdinal("CourseId"));
                lstProfessor.Add(professor);
            }
            con.Close();
        }
        void LoadCourses()
        {
            SqlConnection con = new SqlConnection("Data Source=192.168.81.29,5633;Initial Catalog=TestDB;User ID=sa;Password=auto$tgr56@33");
            con.Open();
            SqlCommand cmd = new SqlCommand("Select * from Courses", con);
            cmd.CommandType = System.Data.CommandType.Text;
            SqlDataReader dr = cmd.ExecuteReader();

            Courses courses = null;
            while (dr.Read())
            {
                courses = new Courses();
                courses.CoursesId = dr.GetInt32(dr.GetOrdinal("CourseId"));
                courses.Name = dr.GetString(dr.GetOrdinal("CourseName"));
                lstCourses.Add(courses);
            }
            con.Close();
        }
        internal bool UpdateCourse(int studentId, int courseId)
        {

            SqlConnection con = new SqlConnection("Data Source=192.168.81.29,5633;Initial Catalog=TestDB;User ID=sa;Password=auto$tgr56@33");
            con.Open();
            SqlCommand cmd = new SqlCommand("update StudentEnrollment Set CourseId=@CourseId where StudentId=@StudentId", con);
            cmd.Parameters.Add("@StudentId", System.Data.SqlDbType.Int).Value = studentId;
            cmd.Parameters.Add("@CourseId", System.Data.SqlDbType.Int).Value = courseId;
            cmd.CommandType = System.Data.CommandType.Text;

            int affectedRows = cmd.ExecuteNonQuery();

            con.Close();
            return affectedRows > 0;
        }

        internal bool AddCourse(int studentId, int courseId)  
        {

            SqlConnection con = new SqlConnection("Data Source=192.168.81.29,5633;Initial Catalog=TestDB;User ID=sa;Password=auto$tgr56@33");
            con.Open();
            SqlCommand cmd = new SqlCommand("update StudentEnrollment Set CourseId=@CourseId where StudentId=@StudentId", con);
            cmd.Parameters.Add("@StudentId", System.Data.SqlDbType.Int).Value = studentId;
            cmd.Parameters.Add("@CourseId", System.Data.SqlDbType.Int).Value = courseId;
            cmd.CommandType = System.Data.CommandType.Text;

            int affectedRows = cmd.ExecuteNonQuery();

            con.Close();
            return affectedRows > 0;
        }

        internal int AddStudent(string studentName)   
        {
            
            SqlConnection con = new SqlConnection("Data Source=192.168.81.29,5633;Initial Catalog=TestDB;User ID=sa;Password=auto$tgr56@33");
            con.Open();
            SqlCommand cmd = new SqlCommand("sp_AddSudent", con);
            cmd.Parameters.Add("@StudentName", System.Data.SqlDbType.VarChar, 50).Value = studentName;
            cmd.Parameters.Add("@StudId", System.Data.SqlDbType.VarChar, 50).Value = 0;
            cmd.Parameters["@StudId"].Direction = System.Data.ParameterDirection.Output;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            cmd.ExecuteNonQuery();
            con.Close();
            return Convert.ToInt32(cmd.Parameters["@StudId"].Value);
        }

        internal  int AddStudentCourse(int studentId, int courseId)   
        {

            SqlConnection con = new SqlConnection("Data Source=192.168.81.29,5633;Initial Catalog=TestDB;User ID=sa;Password=auto$tgr56@33");
            con.Open();
            SqlCommand cmd = new SqlCommand("sp_AddCourse", con);
            cmd.Parameters.Add("@StudentId", System.Data.SqlDbType.Int).Value = studentId;
            cmd.Parameters.Add("@CourseId", System.Data.SqlDbType.Int).Value = courseId;
            
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            int affectrow = cmd.ExecuteNonQuery();
            con.Close();
            return affectrow;
            
        }
        
        internal StudentEnroll[] GetStudentEnroll(int studentId)
        {
            List<StudentEnroll> lst = new List<StudentEnroll>();
            SqlConnection con = new SqlConnection("Data Source=192.168.81.29,5633;Initial Catalog=TestDB;User ID=sa;Password=auto$tgr56@33");
            con.Open();
            SqlCommand cmd = new SqlCommand("Select * from [StudentEnrollment] where StudentId=" + studentId, con);
            cmd.CommandType = System.Data.CommandType.Text;
            SqlDataReader dr = cmd.ExecuteReader();

            StudentEnroll studentEnroll = null;
            while (dr.Read())
            {
                studentEnroll = new StudentEnroll();
                studentEnroll.StudentData = GetStudent(dr.GetInt32(dr.GetOrdinal("StudentId")));
                studentEnroll.CoursesId = dr.GetInt32(dr.GetOrdinal("CourseId"));
                studentEnroll.CoursesName = GetCourses(studentEnroll.CoursesId).Name;
                lst.Add(studentEnroll);
            }
            con.Close();
            return lst.ToArray();
        }
        internal Student[] GetStudents(string professorName)
        {
            List<Student> lst = new List<Student>();
            SqlConnection con = new SqlConnection("Data Source=192.168.81.29,5633;Initial Catalog=TestDB;User ID=sa;Password=auto$tgr56@33");
            con.Open();
            SqlCommand cmd = new SqlCommand("Select Distinct StudentId from StudentEnrollment WHere CourseId IN (Select CourseId from Professors Where ProfessorName=@ProfessorName)", con);
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.Parameters.Add("@ProfessorName", System.Data.SqlDbType.VarChar, 50).Value = professorName;
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lst.Add(GetStudent(dr.GetInt32(dr.GetOrdinal("StudentId"))));
            }
            con.Close();
            return lst.ToArray();
        }
        Student GetStudent(int studentId)
        {
            return lstStudent.FirstOrDefault(x => x.StudentId == studentId);
        }
        Courses GetCourses(int coursesId)
        {
            return lstCourses.FirstOrDefault(x => x.CoursesId == coursesId);
        }
        internal Student[] GetAllStudent()
        {
            return lstStudent.ToArray();
        }
        internal Professor[] GetAllProfessor()
        {
            return lstProfessor.ToArray();
        }
        internal Courses[] GetAllCourses()
        {
            return lstCourses.ToArray();
        }
    }

    class Student
    {
        public int StudentId { get; set; }
        public string Name { get; set; }
    }
    class Professor
    {
        public string Name { get; set; }
        public int CoursesId { get; set; }
    }
    class Courses
    {
        public int CoursesId { get; set; }
        public string Name { get; set; }
    }
    class StudentEnroll
    {
        public Student StudentData { get; set; }
        public int CoursesId { get; set; }
        public string CoursesName { get; set; }
    }

}

