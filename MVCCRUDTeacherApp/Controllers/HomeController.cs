using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using CRUDADO.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
namespace CRUDADO.Controllers
{
    public class HomeController : Controller
    {
        public IConfiguration Configuration { get; }
        public HomeController(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        public IActionResult Index()
{
    List<Teacher> teacherList = new List<Teacher>();
    string connectionString = Configuration["ConnectionStrings:DefaultConnection"];    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        //SqlDataReader
        connection.Open();
        
        string sql = "Select * From Teacher";SqlCommand command = new SqlCommand(sql, connection);
        using (SqlDataReader dataReader = command.ExecuteReader())
        {
            while (dataReader.Read())
            {
                Teacher teacher = new Teacher();
                teacher.Id = Convert.ToInt32(dataReader["Id"]);
                teacher.Name = Convert.ToString(dataReader["Name"]);
                teacher.Skills = Convert.ToString(dataReader["Skills"]);                teacher.TotalStudents = Convert.ToInt32(dataReader["TotalStudents"]);
                teacher.Salary = Convert.ToDecimal(dataReader["Salary"]);
                teacher.AddedOn = Convert.ToDateTime(dataReader["AddedOn"]);                teacherList.Add(teacher);
            }
        }
        connection.Close();
    }
    return View(teacherList);
}
        /* When the user clicks on the "Create" button in index.cshtml, this Method gets called.
         * This Method returns the view create.cshtml used to present the user a view ie form
         * which allows him to fill in the information to add a new Teacher to the database
         * invoked when user goes clicks on the asp-action Create button in index.cshtml
         * <a asp-action="Create" class="btn btn-sm btn-secondary">Create</a></h3>
         * Notice this Method is of type HttpGet not HttpPost ! */
        
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        /* When the user clicks on the "Create" button in index.cshtml, this Method gets called.
       * This Method returns the view create.cshtml used to present the user a view ie form
       * which allows him to fill in the information to add a new Teacher to the database
       * invoked when user goes clicks on the asp-action Create button in index.cshtml
       * <a asp-action="Create" class="btn btn-sm btn-secondary">Create</a></h3>
       * Notice this Method is of type HttpGet not HttpPost ! */
        [HttpPost]
        /* when the method name is Create, the matching view View()
         * will automatically go inside /Home/Create.cshtml
         * when the method does not match the name of the view
         * ie we are using the method name Create_Post, we have to specify
         * a matching view ie //return View("~/Views/Home/Create.cshtml");
         */
        //public IActionResult Create(Teacher teacher)
        public IActionResult Create_Post(Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sql = $"Insert Into Teacher (Name, Skills, TotalStudents, Salary) Values ('{teacher.Name}', '{teacher.Skills}','{teacher.TotalStudents}','{teacher.Salary}')"; using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.CommandType = CommandType.Text;
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                    return RedirectToAction("Index");
                }
            }
            else
                // Since the method name Create_Post is different from the view name Create.cshtml,
                // we need to explicitly state the Path name for that view
                return View("~/Views/Home/Create.cshtml");
                //return View();
        }

        /* There is an Update button for each record in Index.cshtml. When the user
         * clicks on the Update button this Method gets called
        * <a asp-action="Update" asp-route-id="@p.Id">Update</a></td>
        * the asp-route-id sents an additional paramter the id of the record to the Method
        * that gets invoked. After the user clicks on Update he is redirected to the view Update.cshtml
        * 
        * The purpose of this method is to provide a intermediary form to the user where he can make 
        * changes to update a particular teacher and capture it in a stored object 
        * before sending it to the database. The method fills out the form with existing values 
        * based on the id of the route ie localhost:xxxx/home/update/1 when the user clicks on Update 
        * by running the query  string sql = $"Select * From Teacher Where Id='{id}'";        * 
        */
        public IActionResult Update(int id)
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            /* Create a teacher model object which is used to store the information coming from the 
             * database for the particular teacher onto the form */
            Teacher teacher = new Teacher(); 
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"Select * From Teacher Where Id='{id}'";
                SqlCommand command = new SqlCommand(sql, connection);
                connection.Open(); 
                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        /* Grab the information provided by the dataReader from the database
                         * and store it to the teacher object */
                        teacher.Id = Convert.ToInt32(dataReader["Id"]);
                        teacher.Name = Convert.ToString(dataReader["Name"]);
                        teacher.Skills = Convert.ToString(dataReader["Skills"]); 
                        teacher.TotalStudents = Convert.ToInt32(dataReader["TotalStudents"]); 
                        teacher.Salary = Convert.ToDecimal(dataReader["Salary"]); 
                        teacher.AddedOn = Convert.ToDateTime(dataReader["AddedOn"]);
                    }
                }
                connection.Close();
            }
            // redirect to Update Form View, pass teacher object as the model to the Update Form Update.cshtml
            return View(teacher); 
        }
        /* Method which gets called after the user selects to update a particular teacher
         * by clicking on Update in Update.cshtml, updates the record by running the update query on
         * the database */
        [HttpPost]
        [ActionName("Update")]
        public IActionResult Update_Post(Teacher teacher)
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"Update Teacher SET Name='{teacher.Name}', Skills='{teacher.Skills}', TotalStudents='{teacher.TotalStudents}', Salary='{teacher.Salary}' Where Id='{teacher.Id}'"; using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
public IActionResult Delete(int id)
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"Delete From Teacher Where Id='{id}'";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        ViewBag.Result = "Operation got error:" + ex.Message;
                    }
                    connection.Close();
                }
            }
            return RedirectToAction("Index");
        }


    }
}
 