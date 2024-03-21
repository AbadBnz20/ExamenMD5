using ExamenANBS.Contratos;
using ExamenANBS.Modelo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
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
        [OpenApiOperation("Listarspec", "InsertarProveedor", Description = "Sirve para insertar Proveedor")]
        [OpenApiRequestBody("application/json", typeof(Proveedor),
           Description = "Institucion modelo")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json",
            bodyType: typeof(Proveedor),
            Description = "Mostrara  al proveedor insertado")]
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
        [OpenApiOperation("Listarspec", "ListarProveedor", Description = "Sirve para listar todas las Proveedor")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json",
            bodyType: typeof(List<Proveedor>),
            Description = "Mostrara una lista de Proveedores")]
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
        [OpenApiOperation("Listarspec", "EliminarProveedor", Description = "Sirve para eliminar  Proveedor")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json",
            bodyType: typeof(void),
            Description = " eliminar Proveedores")]
        [OpenApiParameter(name: "partitionkey", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "El partitionkey del proveedor")]
        [OpenApiParameter(name: "rowkey", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "El rowkey del proveedor")]
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
        [OpenApiOperation("Listarspec", "ListarProveedor", Description = "Sirve para obtener un  Proveedor")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json",
            bodyType: typeof(Proveedor),
            Description = "Mostrara una lista de Proveedores")]
        [OpenApiParameter(name: "idProveedor", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "El RowKey del proveedor")]
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
        [OpenApiOperation("Listarspec", "EditarProveedor", Description = "Sirve para editar Proveedor")]
        [OpenApiRequestBody("application/json", typeof(Proveedor),
           Description = "proveedor modelo")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json",
            bodyType: typeof(Proveedor),
            Description = "Mostrara  al proveedor editado")]
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
