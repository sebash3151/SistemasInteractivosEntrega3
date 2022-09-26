using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HttpManager : MonoBehaviour
{

    [SerializeField]
    private string URL;

    [Header("MenuRegistro")]
    [SerializeField] bool home = false;
    [SerializeField] InputField userfield;
    [SerializeField] InputField passfield;

    [Header("Textos")]
    [SerializeField] Text[] textos;
    [SerializeField] GameObject[] letreros;
    int contador = 0;
    private int usuarios = 0;

    private string Token;
    private string Usernamename;
    private string SuperScore;

    private void Start()
    {
        if(home == true)
        {
            Token = PlayerPrefs.GetString("token");
            Usernamename = PlayerPrefs.GetString("username");
            SuperScore = PlayerPrefs.GetString("highScore");
            Debug.Log("Token: " + Token);

            StartCoroutine(GetPerfil());
        }

    }

    public void ClickGetScores()
    {
        StartCoroutine(GetScores());
    }

    public void ClickRegistro()
    {
        DataApi data = new DataApi();

        data.username = userfield.text;
        data.password = passfield.text;
        Verificacion();
        string postData = JsonUtility.ToJson(data); 

        StartCoroutine(Registro(postData));
    }

    public void ClickIngreso()
    {
        DataApi data = new DataApi();

        data.username = userfield.text;
        data.password = passfield.text;
        Verificacion();
        string postData = JsonUtility.ToJson(data);

        StartCoroutine(Ingreso(postData));
    }

    public void SendScore()
    {
        Token = PlayerPrefs.GetString("token");

        DataUser data = new DataUser();

        data.username = PlayerPrefs.GetString("username");
        data.score = PlayerPrefs.GetInt("highScore");
             

        string postData = JsonUtility.ToJson(data);

        StartCoroutine(PatchScore(postData));
    }

    IEnumerator GetScores()
    {
        string url = URL + "/api/usuarios" + "?limit=5&sort=true";
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.method = "GET";
        Token = PlayerPrefs.GetString("token");
        Debug.Log(Token);
        www.SetRequestHeader("content-type", "application/json");
        www.SetRequestHeader("x-token", Token);

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if (www.responseCode == 200)
        {
            //Debug.Log(www.downloadHandler.text);
            Scores resData = JsonUtility.FromJson<Scores>(www.downloadHandler.text);

            for (int i = 0; i < 6; i++)
            {
                Debug.Log(resData.usuarios[i].username + " , " + resData.usuarios[i].score);
                textos[i].text = resData.usuarios[i].username + " : " + resData.usuarios[i].score;
            }

            
        }
        else
        {
            Debug.Log(www.error);
        }
    }

    IEnumerator Registro(string postData)
    {
        Debug.Log("Registro: " + postData);
       
        string url = URL + "/api/usuarios";
        UnityWebRequest www = UnityWebRequest.Put(url, postData);
        www.method = "POST";
        www.SetRequestHeader("content-type", "application/json");


        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if (www.responseCode == 200)
        {
            //Debug.Log(www.downloadHandler.text);
            DataApi resData = JsonUtility.FromJson<DataApi>(www.downloadHandler.text);

            Debug.Log("Registrado " + resData.usuario.username + ", id: " + resData.usuario._id);

            StartCoroutine(Ingreso(postData));
            letreros[0].SetActive(true);
        }
        else
        {
            Debug.Log(www.error);
            ErrorBox();
        }
    }

    IEnumerator Ingreso(string postData)
    {
        Debug.Log("Ingreso: " + postData);

        string url = URL + "/api/auth/login";
        UnityWebRequest www = UnityWebRequest.Put(url, postData);
        www.method = "POST";
        www.SetRequestHeader("content-type", "application/json");


        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if (www.responseCode == 200)
        {
            //Debug.Log(www.downloadHandler.text);
            DataApi resData = JsonUtility.FromJson<DataApi>(www.downloadHandler.text);

            Debug.Log("Autenticado " + resData.usuario.username + ", id: " + resData.usuario._id + " y score: " + resData.usuario.score);
            Debug.Log("Token:" + resData.token);
            Debug.Log("ActualScore: " + resData.usuario.score);
            PlayerPrefs.SetString("token", resData.token);
            PlayerPrefs.SetString("username", resData.usuario.username);
            PlayerPrefs.SetInt("highScore", resData.usuario.score);
            letreros[1].SetActive(true);
        }
        else
        {
            Debug.Log(www.error);
            ErrorBox();
        }
    }

    IEnumerator GetPerfil()
    {
        string url = URL + "/api/usuarios/"+Usernamename;
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("x-token", Token);

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if (www.responseCode == 200)
        {
            //Debug.Log(www.downloadHandler.text);
            DataApi resData = JsonUtility.FromJson<DataApi>(www.downloadHandler.text);
            PlayerPrefs.SetInt("highScore", resData.usuario.score);
            Debug.Log("Token valido " + resData.usuario.username + ", id: " + resData.usuario._id + " y score: " + resData.usuario.score);
            PlayGame();
        }
        else
        {
            Debug.Log(www.error);
            ErrorBox();
        }
    }

    IEnumerator PatchScore(string postData)
    {
        Debug.Log("Patch scorre: ");

        string url = URL + "/api/usuarios/";
        UnityWebRequest www = UnityWebRequest.Put(url, postData);
        www.method = "PATCH";
        www.SetRequestHeader("content-type", "application/json");
        www.SetRequestHeader("x-token", Token);

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if (www.responseCode == 200)
        {
            //Debug.Log(www.downloadHandler.text);
            DataApi resData = JsonUtility.FromJson<DataApi>(www.downloadHandler.text);

            Debug.Log("Score Actualziado " + resData.usuario.username + ", score: " + resData.usuario.score);
            letreros[3].SetActive(true);
            
        }
        else
        {
            Debug.Log(www.error);
            ErrorBox();
        }
    }


    public void Verificacion()
    {
        if(userfield.text == null || passfield == null)
        {
            ErrorBox();
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Principal");
    }

    public void ErrorBox()
    {
        if (letreros[2] == null)
        {
            
        }
        else
        {
            letreros[2].SetActive(true);
        }

    }

    public void Reseteo()
    {
        contador = 0;
        usuarios = 0;
    }

}


[System.Serializable]
public class ScoreData
{
    public int user_id;
    public int value;
    public string username;

}

[System.Serializable]
public class Scores
{
    public DataUser[] usuarios;
}

[System.Serializable]
public class DataApi
{
    public string username;
    public string password;
    public DataUser usuario;
    public string token;
}

[System.Serializable]
public class DataUser
{
    public string _id;
    public string username;
    public bool estado;
    public int score;
}
