using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineBackup.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(string id, string name, string type, string lastModifiedDate, int size, HttpPostedFileBase file)
        {
            string filePathName = string.Empty;
            string basePath = Server.MapPath("/Content/");
            if (Request.Files.Count == 0)
                return Json(new { jsonrpc = 2.0, error = new { code = 102, message = "上传文件为空！" }, id = "id" });
            try
            {
                //HttpFileCollectionBase files = Request.Files;
                //HttpPostedFileBase file = Request.Files[0];
                          
                //自行处理保存
                if (!Directory.Exists(basePath))
                    Directory.CreateDirectory(basePath);
                filePathName = basePath + "cms.bak";
                //files[0].SaveAs(filePathName);
                file.SaveAs(filePathName);
            }
            catch (Exception e)
            {
                return Json(new { jsonrpc = 2.0, error = new { code = 103, message = "保存失败:"+e.Message }, id = "id" });
            }

            return Json(new
            {
                jsonrpc = 2.0,
                id = "id",
                message = "OK",
                filePath = filePathName//返回路径以备后用
            });
        }

        [HttpPost]
        public JsonResult Restore(string name, string account, string pwd, bool isRestoreDefault)
        {
            using (SqlConnection connection = new SqlConnection($"Data Source=.;uid={Config("DBUser")};pwd={Config("DBPwd") };database={Config("DBName") };timeout=180"))
            {
                try
                {
                    connection.Open();
                    string dbNewUserName = name + "User";
                    string dbNewUserPwd = name + "User!@#";
                    if (!string.IsNullOrEmpty(account))
                    {
                        dbNewUserName = account;
                        dbNewUserPwd = pwd;
                    }

                    //备份文件获取
                    string path = Server.MapPath("/Content/" + (isRestoreDefault ? "cms" : "temp") + ".bak");
                    List<string> logicalNameList = new List<string>();

                    //还原前，获取数据库备份文件的逻辑名称
                    string cmdText = "restore filelistonly from disk='" + path + "'";
                    SqlCommand cmd = new SqlCommand(cmdText, connection);
                    cmd.CommandTimeout = Int32.MaxValue;//命令执行时间
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        logicalNameList.Add(reader["LogicalName"].ToString());
                    }
                    reader.Close();

                    //创建数据库
                    cmdText = @"restore database " + name + " from disk='" + path + "' with";
                    string dataPath = Server.MapPath("/RestoreData/");
                    cmdText += $" move {logicalNameList[0]} to {dataPath + name}.mdf',";
                    cmdText += $" move {logicalNameList[0]} to {dataPath + name}.ldf',"; ;
                    cmd = new SqlCommand(cmdText, connection);
                    cmd.ExecuteNonQuery();

                    //创建用户
                    cmd = new SqlCommand($"create login {dbNewUserName} with password={dbNewUserPwd}, default_database={name}", connection);
                    cmd.ExecuteNonQuery();
                    string dbSql = $"use {name} create user {dbNewUserName} for login {dbNewUserName} with default_schema=dbo exec sp_addrolemember 'db_owner', '{dbNewUserName}'";
                    cmd = new SqlCommand(dbSql, connection);
                    cmd.ExecuteNonQuery();

                    //删除上传的数据库文件
                    if (!isRestoreDefault)
                    {
                        System.IO.File.Delete(path);
                    }

                    return Json(new
                    {
                        msg = "还原成功"
                    });
                }
                catch (Exception e)
                {
                    return Json(new
                    {
                        msg = "还原失败：" + e.Message
                    });
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        protected string Config(string key)
        {
            switch (key)
            {
                case "DBUser":
                    return "sa";
                case "DBPwd":
                    return "sa1994sa";
                case "DBName":
                    return "ApiDemo";
                default:
                    return "";
            }
        }
    }
}