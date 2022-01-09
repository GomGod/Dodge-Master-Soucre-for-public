using System;
using System.IO;
using System.Text;
using PlayNANOO;
using UnityEngine;


public class UserDataManager
{
    public static void SetNewUserData(string uid)
    {
        var newUdata = new UserData()
        {
            maxScore = 0,
            stackedScore = 0,
            noAds = false,
            controls = GlobalConst.Controls[0],
            bgm = GlobalConst.OnOff[0],
            sfx = GlobalConst.OnOff[0]
        };

        GameManager.Instance.SetUserNickname(uid);
        GameManager.Instance.SetUserData(newUdata);
    }

    public static bool LoadUserData()
    {
        FileStream fStream;
        try
        {
            fStream = new FileStream( GlobalConst.FilePathUserData, FileMode.Open);
        }
        catch (FileNotFoundException)
        {
            return false;
        }

        var data = new byte[fStream.Length];
        fStream.Read(data, 0, data.Length);
        fStream.Close();

        var jsonData = Encoding.UTF8.GetString(data);
        var jsonUdataFormat = JsonUtility.FromJson<UserData>(jsonData);
 
        var uData = new UserData()
        {
            maxScore = jsonUdataFormat.maxScore,
            stackedScore = jsonUdataFormat.stackedScore,
            noAds = jsonUdataFormat.noAds,
            controls = jsonUdataFormat.controls,
            bgm = jsonUdataFormat.bgm,
            sfx = jsonUdataFormat.sfx
        };

        if (uData.controls == string.Empty)
            uData.controls = GlobalConst.Controls[0];
        if (uData.bgm == string.Empty)
            uData.controls = GlobalConst.OnOff[0];
        if (uData.sfx == string.Empty)
            uData.controls = GlobalConst.OnOff[0];
        
        GameManager.Instance.SetUserData(uData);
        return true;
    }

    public static void SaveUserData()
    {
        var fStream = new FileStream(GlobalConst.FilePathUserData, FileMode.Create);
        var jsonData = JsonUtility.ToJson(GameManager.Instance.GetUserData());
        var data = Encoding.UTF8.GetBytes(jsonData);
        
        fStream.Write(data,0,data.Length);
        fStream.Close();
    }
}

[Serializable]
public struct UserData
{
    public int maxScore;
    public int stackedScore;
    public bool noAds;

    #region Player Configs
    public string controls;
    public string bgm;
    public string sfx;
    #endregion
} 