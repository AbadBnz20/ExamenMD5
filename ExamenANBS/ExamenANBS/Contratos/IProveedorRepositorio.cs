using ExamenANBS.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamenANBS.Contratos
{
    public interface IProveedorRepositorio
    {
        public Task<bool> Create(Proveedor proveedor);
        public Task<bool> Update(Proveedor proveedor);
        public Task<bool> Delete(string partitionkey, string rowkey);
        public Task<Proveedor> Get(string id);
        public Task<List<Proveedor>> GetAll();
    }
}
