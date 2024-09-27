using Dapper;
using Microsoft.Data.SqlClient;
using System.Collections.Concurrent;

namespace APIGateway.Services
{
    public class ConfigurationService
    {
        private readonly DapperContext _connectionString;
        private ConcurrentDictionary<string, string> _upstreamServices;

        public ConfigurationService(DapperContext connectionString)
        {
            _connectionString = connectionString;
            _upstreamServices = new ConcurrentDictionary<string, string>();
            LoadConfigurations();
        }

        public void LoadConfigurations()
        {
            using (var connection = _connectionString.CreateDbConnection())
            {
                var services = connection.Query("SELECT service_name, service_url FROM upstream_services WHERE is_active = 1");
                foreach (var service in services)
                {
                    _upstreamServices[service.service_name] = service.service_url;
                }
            }
        }

        public string GetServiceUrl(string serviceName)
        {
            if (_upstreamServices.TryGetValue(serviceName, out var serviceUrl))
            {
                return serviceUrl;
            }
            return null;
        }
    }
}
