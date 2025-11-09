using Microsoft.Win32;

namespace Utilities
{
    public class RegReader
    {
        public static string read(string reg)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\CustomSoft\Data");

            string response = "";
            if (key != null)
            {
                response = key.GetValue(reg).ToString();
                key.Close();
            }

            return response;
        }

    }
}
