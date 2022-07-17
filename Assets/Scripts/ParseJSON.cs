using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ParseJSON
{
    public ResponseData[] data;
}

[System.Serializable]
public class ResponseData
{
    public string name;
    public int code;
}
