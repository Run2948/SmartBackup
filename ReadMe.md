## ���߻�ԭ���ݿⱸ���ļ�

##### 1.����
###### 1.�û��ϴ����ݿⱸ���ļ���.bak����ԭ��ָ���������ϣ�����û����ϴ��ļ�����ԭĬ�ϵı����ļ���
###### 2.��ԭ�ļ��󣬴������ʸ����ݿ���û�����¼������������û����루����û��������¼�������룬������Ĭ�ϵĵ�¼�������룩

##### 2.����
######  �������󣬺��������ʹ��restore database��䣬����ʵ�ֺ���Է��֣��ظ���ԭһ�����ݿ��ļ�������ʾ��ԭʧ�ܣ�����ʹ�ø��ļ�����Ϊ��ԭ�����ļ���Ŀ¼�л�����ͬ���߼��ļ����ƣ����Ի�������⡣

##### 3.�������
###### ʹ��restore database with move��ԭ��䣬��ÿ�λ�ԭ���ļ����ڲ�ͬ��λ��

##### 4.ASP.NET MVC�ϴ����ļ�ʱ��(web.config)���ã�
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
##### 5.webuploader�����ʹ��
* [webuploader C# MVC��](http://blog.sina.com.cn/s/blog_15442fd010102w44e.html)
* [MVC��ʹ��WebUploader����ͼƬԤ���ϴ��Լ��༭](https://www.cnblogs.com/cemaster/p/5604253.html)
* [Asp.Net Mvc ʹ��WebUploader ��ͼƬ�ϴ�](https://www.cnblogs.com/ismars/p/4176912.html)