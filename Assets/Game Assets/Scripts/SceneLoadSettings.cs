using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoadSettings : MonoBehaviour {
    private static List<StringItem> stringCollections = new List<StringItem>();

    public static void SetString(string key, string value)
    {
        foreach (StringItem stringItem in stringCollections)
        {
            if (stringItem.key == key)
            {
                stringCollections.Remove(stringItem);
                break;
            }
        }

        stringCollections.Add(new StringItem(key, value));
    }

    public static string GetString(string key)
    {
        string value = null;

        foreach (StringItem stringItem in stringCollections)
        {
            if (stringItem.key == key)
            {
                value = stringItem.value;
                break;
            }
        }

        return value;
    }

    private class ObjectItem
    {
        private string _key;

        public ObjectItem(string key)
        {
            this._key = key;
        }

        public string key
        {
            get { return _key; }
        }
    }

    private class StringItem : ObjectItem
    {
        private string _value;

        public StringItem(string key, string value) : base(key)
        {
            this._value = value;
        }

        public string value
        {
            get { return _value; }
        }
    }

}
