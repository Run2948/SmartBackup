## 在线还原数据库备份文件

##### 1.需求
###### 1.用户上传数据库备份文件（.bak）还原到指定服务器上（如果用户不上传文件，则还原默认的备份文件）
###### 2.还原文件后，创建访问该数据库的用户，登录名和密码可由用户输入（如果用户不输入登录名和密码，则生成默认的登录名和密码）

##### 2.问题
######  按照需求，很容易想打使用restore database语句，代码实现后测试发现，重复还原一个数据库文件，会提示还原失败，正在使用该文件。因为还原后在文件夹目录中会有相同的逻辑文件名称，所以会出现问题。

##### 3.解决方案
###### 使用restore database with move还原语句，将每次还原的文件放在不同的位置

##### 4.ASP.NET MVC上传大文件时的(web.config)配置：
```xml
<?xml version="1.0"?>
<configuration>
  <system.web>
    <compilation debug="true" targetFramework="4.0"/>
    <httpRuntime maxRequestLength="2147483647" appRequestQueueLimit="1200" executionTimeout="1200"/>
 </system.web>
  <system.webServer>
    <security>
      <requestFiltering >
        <requestLimits maxAllowedContentLength="2147483647" ></requestLimits>
      </requestFiltering>
    </security>

  </system.webServer>
</configuration>
```
##### 5.webuploader插件的使用
* [webuploader C# MVC版](http://blog.sina.com.cn/s/blog_15442fd010102w44e.html)
* [MVC中使用WebUploader进行图片预览上传以及编辑](https://www.cnblogs.com/cemaster/p/5604253.html)
* [Asp.Net Mvc 使用WebUploader 多图片上传](https://www.cnblogs.com/ismars/p/4176912.html)