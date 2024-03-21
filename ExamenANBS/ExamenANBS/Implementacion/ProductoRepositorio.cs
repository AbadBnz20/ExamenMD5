using Azure.Data.Tables;
using ExamenANBS.Contratos;
using ExamenANBS.Modelo;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamenANBS.Implementacion
{
    public class ProductoRepositorio : IProductoRepositorio
    {
        private readonly string? cadenaConexion;
        private readonly string tablaNombre;
        private readonly IConfiguration configuration;

        public ProductoRepositorio (IConfiguration conf)
        {
            configuration= conf;
            cadenaConexion = configuration.GetSection("cadenaconexion").Value;
            tablaNombre = "Producto";
        }


        public async Task<bool> Create(Producto producto)
        {
            try
            {
                var tabla = new TableClient(cadenaConexion, tablaNombre);
                await tabla.UpsertEntityAsync(producto);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> Delete(string partitionkey, string rowkey)
        {
            try
            {
                var tabla = new TableClient(cadenaConexion, tablaNombre);
                await tabla.DeleteEntityAsync(partitionkey, rowkey);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Producto> Get(string id)
        {
            var tablaCliente = new TableClient(cadenaConexion, tablaNombre);
            var filtro = $"PartitionKey eq 'Producto' and RowKey eq '{id}'";
            await foreach (Producto producto in tablaCliente.QueryAsync<Producto>(filter: filtro))
            {
                return producto;
            }
            return null;
        }

        public async Task<List<Producto>> GetAll()
        {
            List<Producto> lista = new List<Producto>();
            var tabla = new TableClient(cadenaConexion, tablaNombre);
            var filtro = $"PartitionKey eq 'Producto'";            
            await foreach (Producto producto in tabla.QueryAsync<Producto>(filter: filtro))
            {
                lista.Add(producto);
            }

            return lista;
        }

        public async Task<bool> Update(Producto producto)
        {
            try
            {
                var tabla = new TableClient(cadenaConexion, tablaNombre);
                await tabla.UpdateEntityAsync(producto, producto.ETag);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
