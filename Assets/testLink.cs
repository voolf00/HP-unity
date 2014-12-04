using UnityEngine;
using System.Collections;
using System.Net;
using System.IO;
using System.Xml;


public class testLink : MonoBehaviour {

// ------------------------------ Login ------------------------------------

    private string username = "";
    private string password= "";

    bool start = false;




	private string text = "Now";
	private int page_id = 1;

	XmlTextReader reader;
	StringReader stringReader;
//------------------------ название страницы и url --------------------------
	string window = "main";
	string url = "http://localhost:8000/unity/";
// -------------------- Генерация ссылки на объект --------------------------
	string url_next;
	string http;
// -------------------- номера страниц --------------------------------------
	int page_up;
	int page_down;
	int page = 1; // и номер страницы

// ------------------------ положение кнопок по х и у -----------------------
	int y = 30;
	int x = 30;
// ---------------------------- другие --------------------------------------
	bool button = false;
	bool article = false;


// ------------------------------ Get впрос в Django по сслыке ---------------


    void Connect(){

        WWWForm form  = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        url_next = url+"login";
        WWW connect = new WWW (url_next, form);


        if (connect.isDone){
            print(connect.text);
        }

        else if (connect.error == null){
             Debug.Log("Error");
        }

    }

	private string HttpGET(string url){
		HttpWebRequest req = WebRequest.Create (url) as HttpWebRequest;
		string Out = null;
		HttpWebResponse resp = req.GetResponse () as HttpWebResponse;
		StreamReader sr = new StreamReader (resp.GetResponseStream());
		Out = sr.ReadToEnd ();
		sr.Close ();

         // обрабатываем его в стринг
        return Out;
	} // обращаемся к серверу и получаем все данные
	private void get_Xml(string httpVar){
		stringReader = new StringReader(httpVar); // обрабатываем его в стринг
         reader = new XmlTextReader(stringReader);
    } // переводим данные в xml, начало запроса начанаетмся отсюда, именно этот бето пишем для запроса
    // запрос записывается в "reader"

    void OnGUI() {


     if(start == false){
        url_next = url+"login";
        start = true;
        HttpGET(url_next);

     }

     username = GUI.TextField(new Rect(Screen.width/2-100, Screen.height/2-100, 200, 20), username, 12);
        //Создаём текстовое поле для ввода пароля
     password = GUI.TextField(new Rect(Screen.width/2-100, Screen.height/2-75, 200, 20), password, 12);

     if (GUI.Button(new Rect(Screen.width/2-100, Screen.height/2-25, 200, 20), "Connect"))
        {
            Connect();
        }

// -------------------------------------------- page start ----------------------------------------------------
//		if(button==false){
//        if(GUI.Button(new Rect(100, 100, 500, 500), window)){
//            url_next = url+"article";
//			http = HttpGET(url_next);
//			get_Xml(http);
//            button = true;
//            //print(reader);
//        }
//         }

//------------------------------------- Список новостей --------------------------------------
        if(button == true && article==false){
			//print(reader);
            while (reader.Read()){
                switch (reader.Name){
                    case "object":
                        string pk = reader["pk"];
                        if(pk != null){
                            url_next = url+ "article/get/"+pk;
                        }
                        break;

                    case "field":
                        string attribute = reader["name"];
                        if (attribute == "article_name")
                        {
                            if (reader.Read())
                            {
                                string name = reader.Value.Trim();
                                GUI.Label (new Rect (Screen.width / 2 - 100, y, 200, 50), name );
                                if(GUI.Button (new Rect(Screen.width / 2 - 100, y+50, 200, 25), "Read" )){

                                    article=true;
                                    http = HttpGET(url_next);
                                }
                                y = y+80;
                            }
                        }
                    break;
				}
            }
			y=30;
			get_Xml(http);
        }

// ------------------------------  открыли новость --------------------------
        if(article==true){
            while (reader.Read()){
                switch (reader.Name){
                    case "object":
                        string pk = reader["pk"];
                        if(pk != null){
                            //print("asdasd");
                             GUI.Label (new Rect (50, 50, 100, 100), "id "+pk );
                             if(GUI.Button (new Rect(Screen.width / 2 - 100, 200, 200, 25), "Back" )){
                                    article=false;
                                    url_next = url+"article";
                                    http = HttpGET(url_next);
                                }
                        }
                        break;
                        case "field":
                            string fname = reader["name"];
                            if (fname == "article_name")
                            {
                                if (reader.Read())
                                {
                                    string name = reader.Value.Trim();
                                    GUI.Label (new Rect (Screen.width / 2 - 100, 100, 200, 50), "name article: "+name );
                                }
                            }
                            if(fname == "article_text" ){
                                if (reader.Read()){
                                    string text = reader.Value.Trim();
                                    GUI.Label (new Rect (Screen.width / 2 - 100, 150, 200, 100), "Text: "+text );
                                }
                            }
                    break;
                }
            }
            get_Xml(http);
        }
    }
}
