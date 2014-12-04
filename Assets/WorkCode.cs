using UnityEngine;
using System.Collections;
using System.Net;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

public class WorkCode : MonoBehaviour {

     string url = "http://localhost:8000/unity";
     string next_url;
     string Out;
     string sCookies;
     string sLocation;
     bool playOne = false;
     private string username ="";
     private string password="";
     private string buildCookie="";

    void getLoginHttp(string login_url){
        HttpWebRequest myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(login_url);
        myHttpWebRequest.Proxy = new WebProxy("127.0.0.1", 8000);
        myHttpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/37.0.2062.124 YaBrowser/14.10.2062.12061 Safari/537.36";
        myHttpWebRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
        myHttpWebRequest.Headers.Add("Accept-Language", "ru,en;q=0.8");
        HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
        StreamReader myStreamReader = new StreamReader(myHttpWebResponse.GetResponseStream(), Encoding.UTF8);
        Out = myStreamReader.ReadToEnd();
        sCookies = "";
        sCookies = myHttpWebResponse.Headers["Set-Cookie"];

       // print(sCookies);
    }

    void postloginHttp(string next_url){
        //next_url = url +"/auth/login";
        HttpWebRequest myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(next_url);
        myHttpWebRequest.Proxy = new WebProxy("127.0.0.1", 8000);
        myHttpWebRequest.Method = "POST";
        myHttpWebRequest.Referer = "http://localhost:8000/unity/auth/login";
        myHttpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/37.0.2062.124 YaBrowser/14.10.2062.12061 Safari/537.36";
        myHttpWebRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
        myHttpWebRequest.Headers.Add("Accept-Language", "ru,en;q=0.8");
        myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
        myHttpWebRequest.Headers.Add(HttpRequestHeader.Cookie, sCookies);
        myHttpWebRequest.AllowAutoRedirect = false;

        string[] result = Regex.Split(sCookies, ";");
        string[] result2 = Regex.Split(result[0],"=");
        string token ="csrfmiddlewaretoken="+result2[1]+"&username="+username + "&password="+password;

        byte[] ByteArr = System.Text.Encoding.ASCII.GetBytes(token);
        myHttpWebRequest.ContentLength = ByteArr.Length;
        myHttpWebRequest.GetRequestStream().Write(ByteArr, 0, ByteArr.Length);
        HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
        //myHttpWebResponse.CookieContainer = new CookieContainer();

        sCookies = myHttpWebResponse.Headers["Set-Cookie"];
        sLocation = myHttpWebResponse.Headers["Location"];



    }


	private string HttpGET(string next_url){
	    //print(next_url);
		HttpWebRequest myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(next_url);
		myHttpWebRequest.Proxy = new WebProxy("127.0.0.1", 8000);
        myHttpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/37.0.2062.124 YaBrowser/14.10.2062.12061 Safari/537.36";
        myHttpWebRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
        myHttpWebRequest.Headers.Add("Accept-Language", "ru,en;q=0.8");


         string[] result = Regex.Split(sCookies, ";");
         string session = result[3].Replace("Path=/,","");
         session = session.Replace(" ","");
         //Path=/,
        // print(result[0]);
        // print(session);
         buildCookie = result[0]+"; "+session;
         myHttpWebRequest.Headers.Add(HttpRequestHeader.Cookie, buildCookie);


       // print(myHttpWebRequest.Headers);
        //myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
		//string Out = null;
		HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
		StreamReader myStreamReader = new StreamReader(myHttpWebResponse.GetResponseStream(), Encoding.UTF8);
        Out = myStreamReader.ReadToEnd();
		//myStreamReader.Close ();

         // обрабатываем его в стринг
        return Out;
	} // обращаемся к серверу и получаем все данные


	void Start () {

	}

	void Update () {

	}

	void OnGUI(){


// Первый запуск приложения заклюяается в том, что человек должен зарегатся
	    if (playOne == false){
	           next_url = url +"/auth/login";
	           playOne=true;
	           getLoginHttp(next_url);

	    } // это условие и выполняется, когда человек вышел из аккаути или заходит в первый раз

        if(buildCookie.IndexOf("sessionid=")==-1){

            GUI.Label(new Rect(Screen.width/2-100,150,200,20), "Введите логин и пароль");

            GUI.Label(new Rect(Screen.width/2-160,190,200,20), "Логин:");
            GUI.Label(new Rect(Screen.width/2-160,220,200,20), "Пароль:");
           // GUI.Label(new Rect(Screen.width/2-160,300,500,250), sCookies);

            username = GUI.TextField(new Rect(Screen.width/2-100,190,200,20),username,12);
            password = GUI.PasswordField(new Rect(Screen.width/2-100,220,200,20),password, "*"[0],12);

             if (GUI.Button(new Rect(Screen.width/2-100, 250, 200, 20), "Вход"))
            {
                next_url = url +"/auth/login";
                postloginHttp(next_url);
                next_url ="http://localhost:8000/unity/article/get/1";
                string test2 = HttpGET(next_url);
                print(test2);
                password="";
            }
        } // отвечает за вход и проверку входа пользователя, чтобы убрать потом таблицы логина и пороля

        else{

             if (GUI.Button(new Rect(Screen.width/2-100, 250, 200, 20), "Выход"))
             {
                 next_url ="http://localhost:8000/unity/auth/logout";
                 string test2 = HttpGET(next_url);
                 print(test2);
                 sCookies = "";
                 buildCookie = "";
                 playOne=false;


             }


        }
       // GUI.Label(new Rect(500,250, 500 ,100), sCookies);
	}
}
