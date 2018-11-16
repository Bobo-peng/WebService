using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

using System.Data.SqlClient;
using System.Data;

using System.Xml.Linq;
namespace WebServiceTest
{
    /// <summary>
    /// Summary description for WebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebService : System.Web.Services.WebService
    {
        static System.Threading.ReaderWriterLockSlim LogWriteLock = new System.Threading.ReaderWriterLockSlim();//文件写锁
        string CurrPath = System.AppDomain.CurrentDomain.BaseDirectory;//路径
        string[] e = new string[4] { "Name", "Chinese", "Math", "English" };

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
        [WebMethod]
        public int Add(int a, int b)
        {
            return a + b;
        }
        [WebMethod]
        public string GetdbData(string Name)
        {
            //数据库操作
            string strcon = "server = localhost,1433;uid = sa; pwd = 123456; database = MyDataBase";
            string strSQL = "select Name,Chinese,Math,English from Student where Name = @name";
            SqlConnection con = new SqlConnection(strcon);
            SqlCommand cmd = new SqlCommand(strSQL, con);
            cmd.Parameters.Add("@name", SqlDbType.VarChar);
            cmd.Parameters["@name"].Value = Name;
            DataSet ds = new DataSet();
            try
            {
                SqlDataAdapter DA = new SqlDataAdapter(cmd);
                DA.Fill(ds, "tb");
            }
            catch (SqlException E)
            {
                throw new Exception(E.Message);
            }
            con.Close();//关闭数据库
            //数据库操作
            //string strcon = "server = localhost,1433;uid = sa; pwd = 123456; database = MyDataBase";
            //string strSQL = "select Name,Chinese,Math,English from Student where Name = @name";


            ////先创建cmd对象
            //SqlCommand cmd = new SqlCommand(strSQL);
            //cmd.Parameters.Add("@name", SqlDbType.VarChar);
            //cmd.Parameters["@name"].Value = Name;

            ////后创建连接对象
            //SqlConnection con = new SqlConnection(strcon);
            //cmd.Connection = con;//连接对象赋值给cmd的连接对象
            //DataSet ds = new DataSet();
            //try
            //{
                
            //    SqlDataAdapter DA = new SqlDataAdapter(cmd);
            //    DA.Fill(ds, "tb");
            //}
            //catch (SqlException E)
            //{
            //    throw new Exception(E.Message);
            //}
            //con.Close();//关闭数据库
            ////数据库操作
            //string strcon = "server = localhost,1433;uid = sa; pwd = 123456; database = MyDataBase";
            //string strSQL = "select Name,Chinese,Math,English from Student where Name = '" + Name + "'";
            //SqlConnection con = new SqlConnection(strcon);
            //DataSet ds = new DataSet();
            //try
            //{
            //    SqlDataAdapter DA = new SqlDataAdapter(strSQL, con);
            //    DA.Fill(ds, "tb");
            //}
            //catch (SqlException E)
            //{
            //    throw new Exception(E.Message);
            //}
            //con.Close();//关闭数据库
            //封装xml

             XDocument xDoc = new XDocument();
             //创建一个根节点
             XElement root = new XElement("Root");
             xDoc.Add(root); //将根节点加入到XML对象中
             try
             {
                 for (int i = 0; ds.Tables.Count > 0 && i < ds.Tables[0].Rows.Count; i++)
                 {
                     //创建一个子节点
                     XElement xele = new XElement("Data" + (i + 1).ToString());
                     root.Add(xele);
                     for (int j = 0; j < e.Length; j++)
                     {
                         xele.SetElementValue(e[j], ds.Tables[0].Rows[i].ItemArray[j]);
                     }
                 }
                 //保存xml文件   
                 LogWriteLock.EnterWriteLock();
                 xDoc.Save(CurrPath + "student.xml");
                 LogWriteLock.ExitWriteLock();

             }
             catch (SqlException E)
             {
                 throw new Exception(E.Message);
             }

             return xDoc.ToString();
        
            
        }

    }
}
