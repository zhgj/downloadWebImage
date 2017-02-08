using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadImageFromWeb.Redis
{
    public interface IRedis
    {
        bool Add(string key, string value);

        string Get(string key);

        long GetCount();
    }
}
