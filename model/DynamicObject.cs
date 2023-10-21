using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base64ApiClient.model
{
    public class DynamicObject
    {
        private Dictionary<string, object> properties = new Dictionary<string, object>();

        public void SetProperty(string propertyName, object value)
        {
            properties[propertyName] = value;
        }

        public object GetProperty(string propertyName)
        {
            return properties.ContainsKey(propertyName) ? properties[propertyName] : null;
        }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, properties);
        }
    }
}
