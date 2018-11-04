using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineBackup.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        ///  全局属性，用来保存上传文件的路径
        /// </summary>
        private static string _fileBak = string.Empty;

        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 上传备份文件
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="lastModifiedDate"></param>
        /// <param name="size"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Upload(string id, string name, string type, string lastModifiedDate, int size, HttpPostedFileBase file)
        {
            string basePath = Server.MapPath("/Content/");
            _fileBak = $"upload_{DateTime.Now:yyyyMMdd}.bak";
            string filePathName;
            if (Request.Files.Count == 0)
                return Json(new { jsonrpc = 2.0, error = new { code = 102, message = "上传文件为空！" }, id = "id" });
            try
            {
                //HttpFileCollectionBase files = Request.Files;
                //HttpPostedFileBase file = Request.Files[0];                         
                //自行处理保存
                if (!Directory.Exists(basePath))
                    Directory.CreateDirectory(basePath);
                filePathName = basePath + _fileBak;
                //files[0].SaveAs(filePathName);
                file.SaveAs(filePathName);
            }
            catch (Exception e)
            {
                return Json(new { jsonrpc = 2.0, error = new { code = 103, message = "保存失败:" + e.Message }, id = "id" });
            }

            return Json(new
            {
                jsonrpc = 2.0,
                id = "id",
                message = "OK",
                filePath = filePathName//返回路径以备后用
            });
        }

        /// <summary>
        /// 开始备份
        /// </summary>
        /// <param name="name"></param>
        /// <param name="account"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Backup(string name, string account = "", string pwd = "")
        {
            //备份文件获取
            string path = Server.MapPath($"/Content/backup_{DateTime.Now:yyyyMMdd}.bak");
            //还原前，获取数据库备份文件的逻辑名称
            string cmdText = $"backup database {name} to disk='{path}'";
            var (ok, msg) = BackOrRestore(cmdText,name);
            return Json(ok ? new { status = 0, message = "备份成功！" } : new { status = 0, message = msg });
        }

        /// <summary>
        /// 开始恢复
        /// </summary>
        /// <param name="name"></param>
        /// <param name="account"></param>
        /// <param name="pwd"></param>
        /// <param name="isRestoreDefault"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Restore(string name, bool isRestoreDefault, string account = "", string pwd = "")
        {
            //备份文件获取
            string path = Server.MapPath("/Content/" + (isRestoreDefault ? $"back_{ DateTime.Now:yyyyMMdd}" : $"{_fileBak}") + ".bak");
            //创建数据库
            string cmdText = $"restore database {name} from disk='{path}' with replace";

            var (ok, msg) = BackOrRestore(cmdText, name, false);
            //删除上传的数据库文件
            if (!isRestoreDefault)
            {
                System.IO.File.Delete(path);
            }
            return Json(ok ? new { status = 0, message = "还原成功！" } : new { status = 0, message = msg });
        }

        /// <summary>
        /// 对数据库的备份和恢复操作，Sql语句实现
        /// </summary>
        /// <param name="cmdText">实现备份或恢复的Sql语句</param>
        /// <param name="dbName">还原或恢复的数据库名称</param>
        /// <param name="isBak">该操作是否为备份操作，是为true否，为false</param>
        protected (bool, string) BackOrRestore(string cmdText,string dbName = "", bool isBak = true)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection($"Data Source={Config("DBServer")};Initial Catalog=master;uid={Config("DBUser")};pwd={Config("DBPwd")};timeout=180");
            try
            {
                conn.Open();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = Int32.MaxValue;//命令执行时间
                if (!isBak)     //如果是恢复操作
                {
                    string setOffline = $"Alter database {dbName} Set Offline With rollback immediate ";
                    string setOnline = $" Alter database {dbName} Set Online With Rollback immediate";
                    cmd.CommandText = setOffline + cmdText + setOnline;
                }
                else
                {
                    cmd.CommandText = cmdText;
                }
                cmd.ExecuteNonQuery();
                return !isBak ? (true, "恭喜你，数据成功恢复为所选文档的状态！") : (true, "恭喜，你已经成功备份当前数据！");
            }
            catch (SqlException sex)
            {
                return (false, "失败，可能是对数据库操作失败，原因：" + sex);
            }
            catch (Exception ex)
            {
                return (false, "对不起，操作失败，可能原因：" + ex);
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
                conn.Dispose();
            }
        }

        /// <summary>
        /// 根据key读取配置
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected string Config(string key)
        {
            switch (key)
            {
                case "DBServer":
                    return ConfigurationManager.AppSettings["DbServer"];
                case "DBUser":
                    return ConfigurationManager.AppSettings["DbUsr"];
                case "DBPwd":
                    return ConfigurationManager.AppSettings["DbPwd"];
                default:
                    throw new KeyNotFoundException($"在Web.config文件中找不到名称为“{key}”的配置项");
            }
        }
    }
}