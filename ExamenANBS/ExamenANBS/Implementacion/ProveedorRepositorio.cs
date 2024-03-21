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
    public class ProveedorRepositorio : IProveedorRepositorio
    {
        private readonly string? cadenaConexion;
        private readonly string tablaNombre;
        private readonly IConfiguration configuration;

        public ProveedorRepositorio(IConfiguration conf)
        {
            configuration = conf;
            cadenaConexion = configuration.GetSection("cadenaconexion").Value;
            tablaNombre = "Proveedor";
        }
        public async Task<bool> Create(Proveedor proveedor)
        {
            try
            {
                var tablaClient = new TableClient(cadenaConexion, tablaNombre);
                await tablaClient.UpsertEntityAsync(proveedor);
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
                var tablaClient = new TableClient(cadenaConexion, tablaNombre);
                await tablaClient.DeleteEntityAsync(partitionkey, rowkey);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Proveedor> Get(string id)
        {
            try
            {
                var tablaClient = new TableClient(cadenaConexion, tablaNombre);
                var filtro = $"PartitionKey eq 'Proveedor' and RowKey eq '{id}'";
                await foreach (Proveedor proveedor in tablaClient.QueryAsync<Proveedor>(filter: filtro))
                {
                    return proveedor;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<Proveedor>> GetAll()
        {
            List<Proveedor> lista = new List<Proveedor>();
            var tablaClient = new TableClient(cadenaConexion, tablaNombre);
            var filtro = $"PartitionKey eq 'Proveedor'";
            await foreach (Proveedor proveedor in tablaClient.QueryAsync<Proveedor>(filter: filtro))
            {
                lista.Add(proveedor);
            }
            return lista;
        }

        public async Task<bool> Update(Proveedor proveedor)
        {
            try
            {
                var tablaClient = new TableClient(cadenaConexion, tablaNombre);
                await tablaClient.UpdateEntityAsync(proveedor, proveedor.ETag);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
