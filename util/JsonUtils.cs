using Base64ApiClient.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base64ApiClient.util
{
    public class JsonUtils
    {
        static public List<DynamicObject> DeserializeJsonArray(string jsonArray)
        {
            var dynamicObjects = new List<DynamicObject>();
            var jArray = Newtonsoft.Json.JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JArray>(jsonArray);

            foreach (var item in jArray)
            {
                dynamicObjects.Add(DeserializeJsonObject(item.ToString()));
            }

            return dynamicObjects;
        }

        static public DynamicObject DeserializeJsonObject(string jsonObject)
        {
            var dynamicObj = new DynamicObject();
            var jObject = Newtonsoft.Json.JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(jsonObject);

            foreach (var property in jObject.Properties())
            {
                if (property.Value.Type == Newtonsoft.Json.Linq.JTokenType.Object)
                {
                    dynamicObj.SetProperty(property.Name, DeserializeJsonObject(property.Value.ToString()));
                }
                else
                {
                    dynamicObj.SetProperty(property.Name, property.Value);
                }
            }

            return dynamicObj;
        }
    }
}
