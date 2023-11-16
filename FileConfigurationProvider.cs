using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HWReflection
{
    public class FileConfigurationProvider
    {
        public object GetValue(string settingName)
        {
            if (settingName != null)
            {
                return settingName;
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        public void SetValue(string settingName, object value)
        {
            settingName = value.ToString();
        }
    }

}
