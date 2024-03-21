using ExamenANBS.Contratos;
using ExamenANBS.Modelo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace ExamenANBS.EndPoints
{
    public class ProveedorFunction
    {
        private readonly ILogger<ProveedorFunction> _logger;
        private readonly IProveedorRepositorio repos;
        public ProveedorFunction(ILogger<ProveedorFunction> logger, IProveedorRepositorio repos)
        {
            _logger = logger;
            this.repos = repos;
        }

        [Function("InsertarProveedor")]
        public async Task<HttpResponseData> InsertarProveedor([HttpTrigger(AuthorizationLevel.Function, "post", Route = "insertarproveedor")] HttpRequestData req)
        {
            HttpResponseData respuesta;
            try
            {
                var proveedor = await req.ReadFromJsonAsync<Proveedor>() ?? throw new Exception("Debe ingresar una Proveedor con todos sus datos");
                proveedor.RowKey = Guid.NewGuid().ToString();
                proveedor.Timestamp = DateTime.UtcNow;
                bool seGuardo = await repos.Create(proveedor);

                if (!seGuardo) return req.CreateResponse(HttpStatusCode.BadRequest);

                respuesta = req.CreateResponse(HttpStatusCode.OK);

                return respuesta;
            }
            catch (Exception)
            {
                respuesta = req.CreateResponse(HttpStatusCode.InternalServerError);
                return respuesta;
            }
        }
        [Function("ListarProveedor")]
        public async Task<HttpResponseData> ListarProveedor([HttpTrigger(AuthorizationLevel.Function, "get", Route = "listarproveedor")] HttpRequestData req)
        {
            HttpResponseData respuesta;
            try
            {
                var proveedores = await repos.GetAll();
                respuesta = req.CreateResponse(HttpStatusCode.OK);
                await respuesta.WriteAsJsonAsync(proveedores);

                return respuesta;
            }
            catch (Exception)
            {
                respuesta = req.CreateResponse(HttpStatusCode.InternalServerError);
                return respuesta;
            }
        }
        [Function("EliminarProveedor")]
        public async Task<HttpResponseData> EliminarProveedor([HttpTrigger(AuthorizationLevel.Function, "get", Route = "eliminarproveedor")] HttpRequestData req)
        {
            HttpResponseData respuesta;
            try
            {
                var partitionkey = req.Query["partitionkey"];
                var rowkey = req.Query["rowkey"];
                bool validate = await repos.Delete(partitionkey, rowkey);
                if (validate)
                {
                    respuesta = req.CreateResponse(HttpStatusCode.OK);
                    return respuesta;
                }
                else
                {
                    respuesta = req.CreateResponse(HttpStatusCode.BadRequest);
                    return respuesta;
                }
            }
            catch (Exception)
            {
                respuesta = req.CreateResponse(HttpStatusCode.InternalServerError);
                return respuesta;
            }
        }
        [Function("ObtenerProveedor")]
        public async Task<HttpResponseData> ObtenerProveedor([HttpTrigger(AuthorizationLevel.Function, "get", Route = "obtenerproveedor")] HttpRequestData req)
        {
            HttpResponseData respuesta;
            try
            {
                var ID = req.Query["idProveedor"];

               
                var proveedor = await repos.Get(ID);

                respuesta = req.CreateResponse(HttpStatusCode.OK);

                await respuesta.WriteAsJsonAsync(proveedor);

                return respuesta;
            }
            catch (Exception)
            {
                respuesta = req.CreateResponse(HttpStatusCode.InternalServerError);
                return respuesta;
            }
        }
        [Function("EditarProveedor")]
        public async Task<HttpResponseData> EditarProveedor([HttpTrigger(AuthorizationLevel.Function, "post", Route = "editarproveedor")] HttpRequestData req)
        {
            HttpResponseData respuesta;
            try
            {
                var proveedor = await req.ReadFromJsonAsync<Proveedor>() ?? throw new Exception("Debe ingresar una Proveedor con todos sus datos");

                bool sw = await repos.Update(proveedor);
                if (sw)
                {
                    respuesta = req.CreateResponse(HttpStatusCode.OK);
                    return respuesta;
                }
                else
                {
                    respuesta = req.CreateResponse(HttpStatusCode.BadRequest);
                    return respuesta;
                }
            }
            catch (Exception)
            {
                respuesta = req.CreateResponse(HttpStatusCode.InternalServerError);
                return respuesta;
            }
        }

    }
}
