using SachOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SachOnline.Controllers
{
    public class Connect
    {
        public dbSachOnlineDataContext data;
        public static dbSachOnlineDataContext GetConnect()
        {
            //string connectionString = "Data Source=LAPTOP-ODTA635P\\SQLEXPRESS01;Initial Catalog=dbNoiThat;Integrated Security=True";
            string connectionString = "Data Source=SQL5110.site4now.net;Initial Catalog=db_aa1bb7_login;User ID=db_aa1bb7_login_admin;Password=tungtomhum113";
            dbSachOnlineDataContext dataContext = new dbSachOnlineDataContext(connectionString);
            return dataContext;
        }
    }
}