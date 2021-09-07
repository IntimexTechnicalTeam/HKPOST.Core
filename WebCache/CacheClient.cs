using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCache
{
    public class CacheClient
    {
        /// <summary>
        /// 异步设置缓存并返回对应缓存数据  (Hash)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="getDataAsync"></param>
        /// <param name="timeOutSecond">key_sign过期时间（秒）</param>
        /// <returns></returns>
        public static async Task<T> CacheShellAsync<T>(string key, string field, Func<Task<T>> getDataAsync, int timeOutSecond = 3600)
        {
            string cacheSign = $"{key}_sign";                                                   //使用sign来防缓存雪崩
            var sign = await RedisHelper.GetAsync(cacheSign);
            if (sign != null)                           //未过期，直接返回。
            {
                var cacheValue = await RedisHelper.HGetAsync(key, field);
                if (cacheValue != null)
                {
                    return JsonConvert.DeserializeObject<T>(cacheValue);
                }
                var result = await getDataAsync();
                await RedisHelper.HSetAsync(key, field, result);               //不用管ret是不是空值，防止缓存穿透
                return result;
            }
            else
            {
                var result = await getDataAsync();
                await RedisHelper.SetAsync(cacheSign, "Just a key sign", timeOutSecond);      //设置缓存标签和过期时间       
                await RedisHelper.HSetAsync(key, field, result);                        //不用管ret是不是空值,直接放在缓存中,防止缓存穿透
                return result;
            }
        }

        /// <summary>
        /// 设置缓存并返回对应缓存数据  (Hash)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="getDataAsync"></param>
        /// <param name="timeOutSecond">key_sign过期时间（秒）</param>
        /// <returns></returns>
        public static T CacheShell<T>(string key, string field, T getDataAsync, int timeOutSecond = 3600)
        {
            string cacheSign = $"{key}_sign";                                                   //使用sign来防缓存雪崩
            var sign = RedisHelper.Get(cacheSign);
            if (sign != null)                   //未过期，直接返回。
            {
                var cacheValue = RedisHelper.HGet(key, field);
                return JsonConvert.DeserializeObject<T>(cacheValue);
            }
            else
            {
                var result = getDataAsync;
                RedisHelper.Set(cacheSign, "Just a key sign", timeOutSecond);                            //设置缓存标签和过期时间
                RedisHelper.HSet(key, field, result);                                               //不用管ret是不是空值,直接放在缓存中,防止缓存穿透
                return result;
            }
        }

    }
}
