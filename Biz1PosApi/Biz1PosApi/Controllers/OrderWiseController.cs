using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Biz1BookPOS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Biz1PosApi.Controllers
{
    [Route("api/[controller]")]
    public class OrderWiseController : Controller
    {
        private POSDbContext db;
        public IConfiguration Configuration { get; }
        public OrderWiseController(POSDbContext contextOptions, IConfiguration configuration)
        {
            db = contextOptions;
            Configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("GetRpt")]
        public IActionResult GetRpt(DateTime frmdate, DateTime todate, int Id,int compId,int sourceId)
        {
            try
            {
                //SqlConnection sqlCon = new SqlConnection("server=(LocalDb)\\MSSQLLocalDB; database=Biz1POS;Trusted_Connection=True;");
                SqlConnection sqlCon = new SqlConnection(Configuration.GetConnectionString("myconn"));
                sqlCon.Open();
                SqlCommand cmd = new SqlCommand("dbo.OrdWiseSalesRpt", sqlCon);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@fromDate", frmdate));
                cmd.Parameters.Add(new SqlParameter("@toDate", todate));
                cmd.Parameters.Add(new SqlParameter("@storeId",Id));
                cmd.Parameters.Add(new SqlParameter("@companyId", compId));
                cmd.Parameters.Add(new SqlParameter("@sourceId", sourceId));
                DataSet ds = new DataSet();
                SqlDataAdapter sqlAdp = new SqlDataAdapter(cmd);
                sqlAdp.Fill(ds);
                DataTable table = ds.Tables[0];
                DataTable table1 = ds.Tables[1];
                DataTable table2 = ds.Tables[2];
                DataTable table3 = ds.Tables[3];
                var data = new
                {
                    Order = ds.Tables[0],
                    order1 = ds.Tables[1],
                    order2 = ds.Tables[2],
                    order3 = ds.Tables[3]

                };
                sqlCon.Close();
                return Ok(data);
            }
            catch(Exception e) 
            {
                var error = new
                {
                    error = new Exception(e.Message, e.InnerException),
                    status = 0,
                    msg = "Something went wrong  Contact our service provider"
                };
                return Json(error);
            }
        }

        [HttpGet("GetStore")]
        public IActionResult GetStore()
        {
            try
            {
                var stores = db.Stores.ToList();
                return Ok(stores);
            }
            catch(Exception e)
            {
                var error = new
                {
                    error = new Exception(e.Message, e.InnerException),
                    status = 0,
                    msg = "Something went wrong  Contact our service provider"
                };
                return Json(error);
            }
            }
    }
}