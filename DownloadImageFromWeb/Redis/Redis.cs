using System;
using ServiceStack.Redis;

namespace DownloadImageFromWeb.Redis
{
    public class Redis : IRedis
    {
        string host = "112.74.23.60";
        int port = 6379;
        string password = "******";

        public bool Add(string key, string value)
        {
            using (RedisClient redisClient = new RedisClient(host, port, password))
            {
                if (redisClient.Get<string>(key) == null)
                {
                    // save value in cache
                    bool result = redisClient.Set(key, value);
                    return result;
                }
                else return false;
            }
        }

        public string Get(string key)
        {
            using (RedisClient redisClient = new RedisClient(host, port, password))
            {
                // get value from the cache by key
                string message = redisClient.Get<string>(key);
                return message;
                /*
                 “ServiceStack.LicenseException”类型的异常在 ServiceStack.Redis.dll 中发生，但未在用户代码中进行处理

                  其他信息: The free-quota limit on '6000 Redis requests per hour' has been reached. Please see https://servicestack.net to upgrade to a commercial license or visit 
                 */
            }
        }

        public long GetCount()
        {
            using (RedisClient redisClient = new RedisClient(host, port, password))
            {
                long message = redisClient.DbSize;
                return message;
            }
        }
    }
}
