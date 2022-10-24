namespace Infrastructure.Consul
{
    public class ConsulServiceOptions
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// 服务IP
        /// </summary>
        public string ServiceIP { get; set; }

        /// <summary>
        /// 服务端口
        /// </summary>
        public int ServicePort { get; set; }

        /// <summary>
        /// http/https
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        /// 服务健康检查
        /// </summary>
        public string ServiceHealthCheck { get; set; }

        /// <summary>
        /// Consul地址
        /// </summary>
        public string ConsulAddress { get; set; }
    }
}