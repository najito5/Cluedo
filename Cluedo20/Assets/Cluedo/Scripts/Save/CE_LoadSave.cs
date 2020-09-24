using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
public class CE_LoadSave
{
   
    public static CE_GlobalSaveData ReadSave(string _path)
    {
        // connard proof
        //if (!CE_DataPath.IsSave()) return;
        return JsonUtility.FromJson<CE_GlobalSaveData>(File.ReadAllText(_path));
    }
}
