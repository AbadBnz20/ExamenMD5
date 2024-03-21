using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamenANBS.Modelo
{
    public class Producto: ITableEntity
    {
        public string Id { get; set; }

        public string Nombre { get; set; }

        public double Precio { get; set; }

        public string ProveedorId { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
