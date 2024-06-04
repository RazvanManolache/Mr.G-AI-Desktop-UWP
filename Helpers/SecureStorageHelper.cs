using MrG.AI.Client.Data;
using MrG.AI.Client.Enum;
using MrG.AI.Client.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace MrG.Desktop.Helpers
{
    internal class SecureStorageHelper : ISecureStorageHelper
    {
        ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public Task<bool> WriteVariableAsync(string key, string value)
        {
            //write to localsettings
            localSettings.Values[key] = value;
            return Task.FromResult(true);
        }

        public Task<string> ReadVariableAsync(string key)
        {
            //read from localsettings
            if (localSettings.Values.ContainsKey(key))
            {
                return Task.FromResult(localSettings.Values[key].ToString());
            }
            return Task.FromResult<string>(null);
        }

        public Task<string> ReadName()
        {
            return ReadVariableAsync("ServerName");

        }

        public Task<string> ReadToken()
        {
            return ReadVariableAsync("Token");
        }

        public Task<string> ReadIP()
        {
            return ReadVariableAsync("IP");
        }

        public Task<Server> GetServer()
        {
            var type = ReadVariableAsync("ServerType").Result;
            if (type == null)
            {
                return Task.FromResult<Server>(null);
            }
            if (!Enum.TryParse<ServerType>(type, out var serverType))
            {
                return Task.FromResult<Server>(null);
            }
            if (serverType == ServerType.Local)
            {
                var name = ReadName().Result;
                var ip = ReadIP().Result;
                if (name == null || ip == null)
                {
                    return Task.FromResult<Server>(null);
                }
                return Task.FromResult<Server>(new Server(serverType, name, ip));
            }
            if (serverType == ServerType.Online)
            {
                var name = ReadName().Result;
                var token = ReadToken().Result;
                if (name == null || token == null)
                {
                    return null;
                }
                return Task.FromResult<Server>(new Server(serverType, name, token));
            }
            return null;

        }

        public bool DeleteVariable(string key)
        {
            if (localSettings.Values.ContainsKey(key))
            {
                localSettings.Values.Remove(key);
                return true;
            }
            return false;
        }

        public async Task<bool> WriteServer(Server server)
        {
            if (server == null)
            {
                return false;
            }
            await WriteVariableAsync("ServerType", server.ServerType.ToString());
            await WriteVariableAsync("ServerName", server.Name);
            if (server.ServerType == ServerType.Local)
            {
                await WriteVariableAsync("IP", server.Ip);
                return true;
            }
            if (server.ServerType == ServerType.Online)
            {
                WriteVariableAsync("Token", server.Token).Wait();
                return true;
            }
            return false;
            
        }

    }
}
