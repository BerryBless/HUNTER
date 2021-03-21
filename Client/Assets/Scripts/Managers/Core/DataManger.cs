using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 요거 상속받아서 만들기
public interface ILoader <Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}
public class DataManger 
{
    //EX)
    //public Dictionary<int, Data.Skill> SkillDict { get; private set; } = new Dictionary<int, Data.Skill>();
    public void Init()
    {
        //SkillDict = LoadJson<Data.SkillData, int, Data.Skill>("StatData").MakeDict();
    }

    Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");
        return JsonUtility.FromJson<Loader>(textAsset.text);
    }
}

