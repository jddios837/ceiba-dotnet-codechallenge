using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using Xunit;
using FluentAssertions;


namespace Api.Test
{

    public class PrestamoControllerTest : IntegrationTestBuilder
    {


        [Fact]
        public void GetPrestamoExito()
        {
            var solicitudPrestamo = new
            {
                TipoUsuario = TipoUsuarioPrestamo.INVITADO,
                IdentificacionUsuario = "3568778897",
                Isbn = new Guid("11f6a454-cc06-4385-8c5d-8d053449102a")
            };
            // cargamos la data a la db para poder obtener id y consultar con este si el proceso de carga fue satisfactorio
            var carga = this.TestClient.PostAsync("/api/prestamo", solicitudPrestamo, new JsonMediaTypeFormatter()).Result;
            carga.EnsureSuccessStatusCode();
            var respuestaCarga = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(carga.Content.ReadAsStringAsync().Result);
            var idPrestamo = respuestaCarga["id"];

            var c = this.TestClient.GetAsync($"api/prestamo/{idPrestamo}").Result;
            c.EnsureSuccessStatusCode();
            var response = c.Content.ReadAsStringAsync().Result;
            var respuestaConsulta = System.Text.Json.JsonSerializer.Deserialize<RespuestaConsultaDto>(response);

            respuestaConsulta.IdUsuarioPrestamoLibro.Should().Be(solicitudPrestamo.IdentificacionUsuario);
            respuestaConsulta.IsbnLibroPrestamo.Should().Be(solicitudPrestamo.Isbn);
        }


        [Fact]
        public void GetPrestamoError()
        {
            var prestamoId = Guid.NewGuid().ToString();
            HttpResponseMessage respuesta = null;
            try
            {
                respuesta = this.TestClient.GetAsync($"api/prestamo/{prestamoId}").Result;
                respuesta.EnsureSuccessStatusCode();
                Assert.True(false, "Deberia fallar");
            }
            catch (Exception)
            {
                respuesta.StatusCode.Should().Be(HttpStatusCode.NotFound);
            }
        }


        [Fact]
        public void PostPrestamoError()
        {
            var usuarioId = "1234567891";

            var errorMessage = $"El usuario con identificacion {usuarioId} ya tiene un libro prestado por lo cual no se le puede realizar otro prestamo";
            HttpResponseMessage respuesta = null;

            try
            {
                var solicitudPrestamo = new 
                {
                    TipoUsuario = TipoUsuarioPrestamo.INVITADO,
                    IdentificacionUsuario = usuarioId,
                    Isbn = new Guid("11f6a454-cc06-4385-8c5d-8d053449102a")
                };

                respuesta = this.TestClient.PostAsync("api/prestamo", solicitudPrestamo, new JsonMediaTypeFormatter()).Result;
                respuesta.EnsureSuccessStatusCode();

                respuesta = this.TestClient.PostAsync("api/prestamo", solicitudPrestamo, new JsonMediaTypeFormatter()).Result;
                respuesta.EnsureSuccessStatusCode();

                 Assert.True(false,"No deberia permitir prestar otro libro a este invitado");
            }
            catch (Exception)
            {
                var contenidoRespuesta = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(respuesta.Content.ReadAsStringAsync().Result);
                contenidoRespuesta["mensaje"].Should().Be(errorMessage);
            }
        }


        [Fact]
        public void PostPrestamoInvitadoFechaEntregaExito()
        {

            var solicitudPrestamo = new 
            {
                TipoUsuario = TipoUsuarioPrestamo.INVITADO,
                IdentificacionUsuario = "8877789999",
                Isbn = new Guid("11f6a454-cc06-4385-8c5d-8d053449102a")
            };

            var fechaEsperada = CalcularFechaEntrega(TipoUsuarioPrestamo.INVITADO).ToShortDateString();

            var respuesta = this.TestClient.PostAsync("api/prestamo", solicitudPrestamo, new JsonMediaTypeFormatter()).Result;
            respuesta.EnsureSuccessStatusCode();
            var prestamoRadicado = System.Text.Json.JsonSerializer
               .Deserialize<Dictionary<string, object>>(respuesta.Content.ReadAsStringAsync().Result);
            var fechaEntrega = DateTime.Parse(prestamoRadicado["fechaMaximaDevolucion"].ToString()).ToShortDateString();
            fechaEntrega.Should().Be(fechaEsperada);
        }

        [Fact]
        public void PostPrestamoEmpleadoFechaEntregaExito()
        {

            var solicitudPrestamo = new 
            {
                TipoUsuario = TipoUsuarioPrestamo.EMPLEADO,
                IdentificacionUsuario = "2336788956",
                Isbn = new Guid("977ef918-f8dc-492c-98d6-66dfe90e8947")
            };

            var fechaEsperadaEmpleado = CalcularFechaEntrega(TipoUsuarioPrestamo.EMPLEADO).ToShortDateString();

            var respuesta = this.TestClient.PostAsync("api/prestamo", solicitudPrestamo, new JsonMediaTypeFormatter()).Result;
            respuesta.EnsureSuccessStatusCode();
            var prestamoRadicado = System.Text.Json.JsonSerializer
                .Deserialize<Dictionary<string, object>>(respuesta.Content.ReadAsStringAsync().Result);
            var fechaEntrega = DateTime.Parse(prestamoRadicado["fechaMaximaDevolucion"].ToString()).ToShortDateString();
            fechaEntrega.Should().Be(fechaEsperadaEmpleado);

        }

        [Fact]
        public void PostPrestamoAfiliadoFechaEntregaExito()
        {           
            var solicitudPrestamo = new 
            {
                TipoUsuario = TipoUsuarioPrestamo.AFILIADO,
                IdentificacionUsuario = "2345678543",
                Isbn = new Guid("977ef918-f8dc-492c-98d6-66dfe90e8947")
            };

            var fechaEsperadaAfiliado = CalcularFechaEntrega(TipoUsuarioPrestamo.AFILIADO).ToShortDateString();

            var respuesta = this.TestClient.PostAsync("api/prestamo", solicitudPrestamo, new JsonMediaTypeFormatter()).Result;
            respuesta.EnsureSuccessStatusCode();
            var prestamoRadicado = System.Text.Json.JsonSerializer
                .Deserialize<Dictionary<string, object>>(respuesta.Content.ReadAsStringAsync().Result);
            var fechaEntrega = DateTime.Parse(prestamoRadicado["fechaMaximaDevolucion"].ToString()).ToShortDateString();
            fechaEntrega.Should().Be(fechaEsperadaAfiliado);
        }

        [Fact]
        public void PostPrestamoAfiliadoIsbnError()
        {
            HttpResponseMessage respuesta = null;
            try
            {
                var solicitudPrestamo = new
                {
                    TipoUsuario = TipoUsuarioPrestamo.AFILIADO,
                    IdentificacionUsuario = Guid.NewGuid().ToString(),
                    Isbn = "ASDFG123456789"
                };

                var fechaEsperadaAfiliado = CalcularFechaEntrega(TipoUsuarioPrestamo.AFILIADO).ToShortDateString();

                respuesta = this.TestClient.PostAsync("api/prestamo", solicitudPrestamo, new JsonMediaTypeFormatter()).Result;
                respuesta.EnsureSuccessStatusCode();
                Assert.True(false,"No deberia responder con exito, mal payload");
            }
            catch (Exception)
            {                
                respuesta.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            }
        }

        [Fact]
        public void PostPrestamoAfiliadoTipoUsuarioError()
        {
            HttpResponseMessage respuesta = null;
            try
            {
                var solicitudPrestamo = new
                {
                    TipoUsuario = 5,
                    IdentificacionUsuario = Guid.NewGuid().ToString(),
                    Isbn = Guid.NewGuid().ToString()
                };

                var fechaEsperadaAfiliado = CalcularFechaEntrega(TipoUsuarioPrestamo.AFILIADO).ToShortDateString();

                respuesta = this.TestClient.PostAsync("api/prestamo", solicitudPrestamo, new JsonMediaTypeFormatter()).Result;
                respuesta.EnsureSuccessStatusCode();
               Assert.True(false,"No deberia responder con exito, mal payload");
            }
            catch (Exception)
            {               
                respuesta.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            }
        }       

    }

}
