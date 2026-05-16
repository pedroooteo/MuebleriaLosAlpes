Imports System.Net
Imports System.Text
Imports System.IO
Imports System.Text.Json
Imports MuebleriaAPI.Negocio
Imports MuebleriaAPI.Entidades

Module Program
    Sub Main()
        Dim puertoEnv = Environment.GetEnvironmentVariable("PORT")
        Dim puerto = If(puertoEnv, "8080")
        Dim servidor As New HttpListener()
        If puertoEnv IsNot Nothing Then
            servidor.Prefixes.Add($"http://*:{puerto}/")
        Else
            servidor.Prefixes.Add($"http://localhost:{puerto}/")
            servidor.Prefixes.Add($"http://127.0.0.1:{puerto}/")
        End If
        servidor.Start()

        Console.WriteLine("=== SERVIDOR API MUEBLERÍA INICIADO ===")
        Console.WriteLine($"Escuchando en http://localhost:{puerto}/")

        Dim authNegocio     As New SeguridadNegocio()
        Dim muebleNegocio   As New MuebleNegocio()
        Dim carritoNegocio  As New CarritoNegocio()
        Dim clienteNegocio  As New ClienteNegocio()
        Dim reporteNegocio  As New ReporteNegocio()

        While True
            Dim contexto = servidor.GetContext()
            Dim req = contexto.Request
            Dim res = contexto.Response

            res.AddHeader("Access-Control-Allow-Origin", "*")
            res.AddHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS")
            res.AddHeader("Access-Control-Allow-Headers", "Content-Type, ngrok-skip-browser-warning")

            If req.HttpMethod = "OPTIONS" Then
                res.StatusCode = 204
                res.Close()
                Continue While
            End If

            Dim path = req.Url.AbsolutePath.ToLower()

            ' === LOGIN ===
            If path = "/api/login" AndAlso req.HttpMethod = "POST" Then
                ManejarLogin(req, res, authNegocio)

            ' === MUEBLES PUBLICO ===
            ElseIf path = "/api/muebles" AndAlso req.HttpMethod = "GET" Then
                ManejarObtenerMuebles(req, res, muebleNegocio)

            ElseIf path = "/api/muebles" AndAlso req.HttpMethod = "POST" Then
                ManejarRegistrarMueble(req, res, muebleNegocio)

            ElseIf path = "/api/muebles/detalle" AndAlso req.HttpMethod = "GET" Then
                ManejarDetalleMueble(req, res, muebleNegocio)

            ' === ADMIN MUEBLES ===
            ElseIf path = "/api/admin/muebles" AndAlso req.HttpMethod = "GET" Then
                ManejarBuscarMueblesAdmin(req, res, muebleNegocio)

            ElseIf path = "/api/admin/muebles/modificar" AndAlso req.HttpMethod = "POST" Then
                ManejarModificarMueble(req, res, muebleNegocio)

            ElseIf path = "/api/admin/muebles/precio-stock" AndAlso req.HttpMethod = "POST" Then
                ManejarActualizarPrecioStock(req, res, muebleNegocio)

            ElseIf path = "/api/admin/muebles/eliminar" AndAlso req.HttpMethod = "POST" Then
                ManejarEliminarMueble(req, res, muebleNegocio)

            ElseIf path = "/api/admin/categorias" AndAlso req.HttpMethod = "GET" Then
                ManejarListarCategorias(req, res, muebleNegocio)

            ' === CARRITO ===
            ElseIf path = "/api/carrito/agregar" AndAlso req.HttpMethod = "POST" Then
                ManejarAgregarAlCarrito(req, res, carritoNegocio)

            ElseIf path = "/api/carrito" AndAlso req.HttpMethod = "GET" Then
                ManejarVerCarrito(req, res, carritoNegocio)

            ElseIf path = "/api/carrito/eliminar" AndAlso req.HttpMethod = "POST" Then
                ManejarEliminarDelCarrito(req, res, carritoNegocio)

            ' === COMPRA E HISTORIAL ===
            ElseIf path = "/api/compra" AndAlso req.HttpMethod = "POST" Then
                ManejarEfectuarCompra(req, res, carritoNegocio)

            ElseIf path = "/api/historial" AndAlso req.HttpMethod = "GET" Then
                ManejarHistorial(req, res, carritoNegocio)

            ' === CLIENTES ===
            ElseIf path = "/api/clientes/perfil" AndAlso req.HttpMethod = "GET" Then
                ManejarPerfil(req, res, clienteNegocio)

            ElseIf path = "/api/admin/clientes" AndAlso req.HttpMethod = "GET" Then
                ManejarBuscarClientes(req, res, clienteNegocio)

            ElseIf path = "/api/admin/clientes/eliminar" AndAlso req.HttpMethod = "POST" Then
                ManejarEliminarCliente(req, res, clienteNegocio)

            ' === REGISTRO ===
            ElseIf path = "/api/registro" AndAlso req.HttpMethod = "POST" Then
                ManejarRegistro(req, res, clienteNegocio)

            ' === CIUDADES ===
            ElseIf path = "/api/ciudades" AndAlso req.HttpMethod = "GET" Then
                ManejarCiudades(req, res, carritoNegocio)

            ' === REPORTES ===
            ElseIf path = "/api/reportes/ventas-diarias" AndAlso req.HttpMethod = "GET" Then
                ManejarVentasDiarias(req, res, reporteNegocio)

            ElseIf path = "/api/reportes/producto-top" AndAlso req.HttpMethod = "GET" Then
                ManejarProductoTop(req, res, reporteNegocio)

            ElseIf path = "/api/reportes/historial-cliente" AndAlso req.HttpMethod = "GET" Then
                ManejarHistorialClienteAdmin(req, res, reporteNegocio)

            ElseIf path = "/api/reportes/cierre-cajas" AndAlso req.HttpMethod = "GET" Then
                ManejarCierreCajas(req, res, reporteNegocio)

            ElseIf path = "/api/reportes/ltv" AndAlso req.HttpMethod = "GET" Then
                ManejarLTV(req, res, reporteNegocio)

            ElseIf path = "/api/reportes/actividad" AndAlso req.HttpMethod = "GET" Then
                ManejarActividad(req, res, reporteNegocio)

            ElseIf path = "/api/reportes/retencion" AndAlso req.HttpMethod = "GET" Then
                ManejarRetencion(req, res, reporteNegocio)

            ElseIf path = "/api/reportes/cohorte" AndAlso req.HttpMethod = "GET" Then
                ManejarCohorte(req, res, reporteNegocio)

            ElseIf path = "/api/reportes/remarketing" AndAlso req.HttpMethod = "GET" Then
                ManejarRemarketing(req, res, reporteNegocio)

            Else
                res.StatusCode = 404
                EscribirRespuesta(res, "{ ""exito"": false, ""mensaje"": ""Ruta no encontrada"" }")
            End If

            res.Close()
        End While
    End Sub

    ' ==================== TUS MÉTODOS (mantengo los que ya tenías) ====================
    Private Sub ManejarLogin(req As HttpListenerRequest, res As HttpListenerResponse, auth As SeguridadNegocio)
        Try
            Dim body = LeerBody(req)
            Dim login = JsonSerializer.Deserialize(Of UsuarioLogin)(body, New JsonSerializerOptions With {.PropertyNameCaseInsensitive = True})
            Dim valido = auth.AutenticarUsuario(login.Username, login.Password)

            If valido Then
                Dim idClienteVal As Integer = 0
                Dim idRolVal As Integer = 0
                Dim nombreRolVal As String = ""
                Try
                    Dim datos = auth.ObtenerDatosUsuario(login.Username)
                    idClienteVal = datos.idCliente
                    idRolVal = datos.idRol
                    nombreRolVal = datos.nombreRol
                Catch
                End Try
                Dim json = "{ ""exito"": true, ""username"": """ & login.Username & """, " &
                           """idCliente"": " & idClienteVal & ", " &
                           """idRol"": " & idRolVal & ", " &
                           """nombreRol"": """ & nombreRolVal & """ }"
                EscribirRespuesta(res, json)
            Else
                EscribirRespuesta(res, "{ ""exito"": false, ""mensaje"": ""Usuario o contraseña incorrectos"" }")
            End If
        Catch
            res.StatusCode = 500
            EscribirRespuesta(res, "{ ""exito"": false, ""mensaje"": ""Error interno"" }")
        End Try
    End Sub

    Private Sub ManejarObtenerMuebles(req As HttpListenerRequest, res As HttpListenerResponse, muebleNeg As MuebleNegocio)
        Try
            EscribirRespuesta(res, JsonSerializer.Serialize(muebleNeg.ObtenerMuebles()))
        Catch
            res.StatusCode = 500
            EscribirRespuesta(res, "[]")
        End Try
    End Sub

    Private Sub ManejarRegistrarMueble(req As HttpListenerRequest, res As HttpListenerResponse, muebleNeg As MuebleNegocio)
        Try
            Dim body = LeerBody(req)
            Dim mueble = JsonSerializer.Deserialize(Of Mueble)(body, New JsonSerializerOptions With {.PropertyNameCaseInsensitive = True})
            Dim mensaje = muebleNeg.RegistrarNuevoMueble(mueble)
            Dim exito = Not mensaje.Contains("Error")
            EscribirRespuesta(res, "{ ""exito"": " & exito.ToString().ToLower() & ", ""mensaje"": """ & mensaje.Replace("""", "'") & """ }")
        Catch
            res.StatusCode = 500
            EscribirRespuesta(res, "{ ""exito"": false, ""mensaje"": ""Error al registrar"" }")
        End Try
    End Sub

    Private Sub ManejarAgregarAlCarrito(req As HttpListenerRequest, res As HttpListenerResponse, carritoNeg As CarritoNegocio)
        Try
            Dim body = LeerBody(req)
            Dim d = JsonSerializer.Deserialize(Of Dictionary(Of String, JsonElement))(body)
            carritoNeg.AgregarAlCarrito(d("idCliente").GetInt32(), d("idMueble").GetInt32(), d("cantidad").GetInt32())
            EscribirRespuesta(res, "{ ""exito"": true, ""mensaje"": ""Producto agregado al carrito"" }")
        Catch ex As Exception
            EscribirRespuesta(res, "{ ""exito"": false, ""mensaje"": """ & ex.Message.Replace("""", "'") & """ }")
        End Try
    End Sub

    Private Sub ManejarVerCarrito(req As HttpListenerRequest, res As HttpListenerResponse, carritoNeg As CarritoNegocio)
        Try
            Dim idCliente = Integer.Parse(req.QueryString("idCliente"))
            EscribirRespuesta(res, JsonSerializer.Serialize(carritoNeg.VerCarrito(idCliente)))
        Catch
            EscribirRespuesta(res, "[]")
        End Try
    End Sub

    Private Sub ManejarEfectuarCompra(req As HttpListenerRequest, res As HttpListenerResponse, carritoNeg As CarritoNegocio)
        Try
            Dim body = LeerBody(req)
            Dim d = JsonSerializer.Deserialize(Of Dictionary(Of String, JsonElement))(body)
            Dim numeroOrden = carritoNeg.EfectuarCompra(d("idCliente").GetInt32(), d("formaPago").GetString())
            EscribirRespuesta(res, "{ ""exito"": true, ""numeroOrden"": """ & numeroOrden & """, ""mensaje"": ""Compra realizada con exito"" }")
        Catch ex As Exception
            EscribirRespuesta(res, "{ ""exito"": false, ""mensaje"": """ & ex.Message.Replace("""", "'") & """ }")
        End Try
    End Sub

    ' ==================== ADMIN MUEBLES ====================
    Private Sub ManejarDetalleMueble(req As HttpListenerRequest, res As HttpListenerResponse, muebleNeg As MuebleNegocio)
        Try
            Dim id = Integer.Parse(req.QueryString("id"))
            EscribirRespuesta(res, JsonSerializer.Serialize(muebleNeg.ObtenerDetalleMueble(id)))
        Catch ex As Exception
            res.StatusCode = 500
            EscribirRespuesta(res, "{ ""exito"": false, ""mensaje"": """ & ex.Message.Replace("""", "'") & """ }")
        End Try
    End Sub

    Private Sub ManejarBuscarMueblesAdmin(req As HttpListenerRequest, res As HttpListenerResponse, muebleNeg As MuebleNegocio)
        Try
            Dim criterio = If(req.QueryString("criterio"), "")
            Dim tipo     = If(req.QueryString("tipo"), "")
            EscribirRespuesta(res, JsonSerializer.Serialize(muebleNeg.BuscarMueblesAdmin(criterio, tipo)))
        Catch ex As Exception
            res.StatusCode = 500
            EscribirRespuesta(res, "[]")
        End Try
    End Sub

    Private Sub ManejarModificarMueble(req As HttpListenerRequest, res As HttpListenerResponse, muebleNeg As MuebleNegocio)
        Try
            Dim body = LeerBody(req)
            Dim d = JsonSerializer.Deserialize(Of Dictionary(Of String, JsonElement))(body)
            muebleNeg.ModificarMueble(
                d("idMueble").GetInt32(),
                d("referencia").GetString(),
                d("nombre").GetString(),
                d("descripcion").GetString(),
                d("tipo").GetString(),
                d("idCategoria").GetInt32(),
                d("material").GetString(),
                d("altoCm").GetDecimal(),
                d("anchoCm").GetDecimal(),
                d("profundidadCm").GetDecimal(),
                d("color").GetString(),
                d("pesoGramos").GetDecimal(),
                d("fotoUrl").GetString()
            )
            EscribirRespuesta(res, "{ ""exito"": true, ""mensaje"": ""Mueble actualizado correctamente"" }")
        Catch ex As Exception
            res.StatusCode = 500
            EscribirRespuesta(res, "{ ""exito"": false, ""mensaje"": """ & ex.Message.Replace("""", "'") & """ }")
        End Try
    End Sub

    Private Sub ManejarActualizarPrecioStock(req As HttpListenerRequest, res As HttpListenerResponse, muebleNeg As MuebleNegocio)
        Try
            Dim body = LeerBody(req)
            Dim d = JsonSerializer.Deserialize(Of Dictionary(Of String, JsonElement))(body)
            muebleNeg.ActualizarPrecioStock(d("idMueble").GetInt32(), d("precio").GetDecimal(), d("stock").GetInt32())
            EscribirRespuesta(res, "{ ""exito"": true, ""mensaje"": ""Precio y stock actualizados"" }")
        Catch ex As Exception
            res.StatusCode = 500
            EscribirRespuesta(res, "{ ""exito"": false, ""mensaje"": """ & ex.Message.Replace("""", "'") & """ }")
        End Try
    End Sub

    Private Sub ManejarEliminarMueble(req As HttpListenerRequest, res As HttpListenerResponse, muebleNeg As MuebleNegocio)
        Try
            Dim body = LeerBody(req)
            Dim d = JsonSerializer.Deserialize(Of Dictionary(Of String, JsonElement))(body)
            muebleNeg.EliminarMueble(d("idMueble").GetInt32())
            EscribirRespuesta(res, "{ ""exito"": true, ""mensaje"": ""Mueble eliminado correctamente"" }")
        Catch ex As Exception
            res.StatusCode = 500
            EscribirRespuesta(res, "{ ""exito"": false, ""mensaje"": """ & ex.Message.Replace("""", "'") & """ }")
        End Try
    End Sub

    Private Sub ManejarListarCategorias(req As HttpListenerRequest, res As HttpListenerResponse, muebleNeg As MuebleNegocio)
        Try
            EscribirRespuesta(res, JsonSerializer.Serialize(muebleNeg.ListarCategorias()))
        Catch
            EscribirRespuesta(res, "[]")
        End Try
    End Sub

    ' ==================== CARRITO EXTRA ====================
    Private Sub ManejarEliminarDelCarrito(req As HttpListenerRequest, res As HttpListenerResponse, carritoNeg As CarritoNegocio)
        Try
            Dim body = LeerBody(req)
            Dim d = JsonSerializer.Deserialize(Of Dictionary(Of String, JsonElement))(body)
            carritoNeg.EliminarDelCarrito(d("idDetalle").GetInt32())
            EscribirRespuesta(res, "{ ""exito"": true, ""mensaje"": ""Item eliminado"" }")
        Catch ex As Exception
            EscribirRespuesta(res, "{ ""exito"": false, ""mensaje"": """ & ex.Message.Replace("""", "'") & """ }")
        End Try
    End Sub

    Private Sub ManejarHistorial(req As HttpListenerRequest, res As HttpListenerResponse, carritoNeg As CarritoNegocio)
        Try
            Dim idCliente = Integer.Parse(req.QueryString("idCliente"))
            EscribirRespuesta(res, JsonSerializer.Serialize(carritoNeg.HistorialCompras(idCliente)))
        Catch
            EscribirRespuesta(res, "[]")
        End Try
    End Sub

    Private Sub ManejarCiudades(req As HttpListenerRequest, res As HttpListenerResponse, carritoNeg As CarritoNegocio)
        Try
            EscribirRespuesta(res, JsonSerializer.Serialize(carritoNeg.ObtenerCiudades()))
        Catch
            EscribirRespuesta(res, "[]")
        End Try
    End Sub

    ' ==================== REGISTRO ====================
    Private Sub ManejarRegistro(req As HttpListenerRequest, res As HttpListenerResponse, clienteNeg As ClienteNegocio)
        Try
            Dim body = LeerBody(req)
            Dim d = JsonSerializer.Deserialize(Of Dictionary(Of String, JsonElement))(body)
            clienteNeg.RegistrarCliente(
                d("tipoDoc").GetString(),
                d("numDoc").GetString(),
                d("nombre").GetString(),
                d("telResidencia").GetString(),
                If(d.ContainsKey("telCelular"), d("telCelular").GetString(), ""),
                d("direccion").GetString(),
                d("idCiudad").GetInt32(),
                d("email").GetString(),
                If(d.ContainsKey("profesion"), d("profesion").GetString(), ""),
                d("tipoPersona").GetString(),
                If(d.ContainsKey("nit"), d("nit").GetString(), ""),
                d("username").GetString(),
                d("password").GetString()
            )
            EscribirRespuesta(res, "{ ""exito"": true, ""mensaje"": ""Cuenta creada exitosamente"" }")
        Catch ex As Exception
            res.StatusCode = 500
            EscribirRespuesta(res, "{ ""exito"": false, ""mensaje"": """ & ex.Message.Replace("""", "'") & """ }")
        End Try
    End Sub

    ' ==================== ADMIN CLIENTES ====================
    Private Sub ManejarPerfil(req As HttpListenerRequest, res As HttpListenerResponse, clienteNeg As ClienteNegocio)
        Try
            Dim idCliente = Integer.Parse(req.QueryString("idCliente"))
            EscribirRespuesta(res, JsonSerializer.Serialize(clienteNeg.ObtenerPerfil(idCliente)))
        Catch ex As Exception
            res.StatusCode = 500
            EscribirRespuesta(res, "{ ""exito"": false, ""mensaje"": """ & ex.Message.Replace("""", "'") & """ }")
        End Try
    End Sub

    Private Sub ManejarBuscarClientes(req As HttpListenerRequest, res As HttpListenerResponse, clienteNeg As ClienteNegocio)
        Try
            Dim criterio = If(req.QueryString("criterio"), "")
            EscribirRespuesta(res, JsonSerializer.Serialize(clienteNeg.BuscarClientes(criterio)))
        Catch
            EscribirRespuesta(res, "[]")
        End Try
    End Sub

    Private Sub ManejarEliminarCliente(req As HttpListenerRequest, res As HttpListenerResponse, clienteNeg As ClienteNegocio)
        Try
            Dim body = LeerBody(req)
            Dim d = JsonSerializer.Deserialize(Of Dictionary(Of String, JsonElement))(body)
            clienteNeg.EliminarCliente(d("idCliente").GetInt32())
            EscribirRespuesta(res, "{ ""exito"": true, ""mensaje"": ""Cliente eliminado correctamente"" }")
        Catch ex As Exception
            res.StatusCode = 500
            EscribirRespuesta(res, "{ ""exito"": false, ""mensaje"": """ & ex.Message.Replace("""", "'") & """ }")
        End Try
    End Sub

    ' ==================== REPORTES ====================
    Private Sub ManejarVentasDiarias(req As HttpListenerRequest, res As HttpListenerResponse, reporteNeg As ReporteNegocio)
        Try
            Dim fechaIni = req.QueryString("fechaIni")
            Dim fechaFin = req.QueryString("fechaFin")
            Dim idCiudad = Integer.Parse(If(req.QueryString("idCiudad"), "0"))
            EscribirRespuesta(res, JsonSerializer.Serialize(reporteNeg.VentasDiarias(fechaIni, fechaFin, idCiudad)))
        Catch
            EscribirRespuesta(res, "[]")
        End Try
    End Sub

    Private Sub ManejarProductoTop(req As HttpListenerRequest, res As HttpListenerResponse, reporteNeg As ReporteNegocio)
        Try
            Dim fechaIni = req.QueryString("fechaIni")
            Dim fechaFin = req.QueryString("fechaFin")
            Dim idCiudad = Integer.Parse(If(req.QueryString("idCiudad"), "0"))
            EscribirRespuesta(res, JsonSerializer.Serialize(reporteNeg.ProductoTop(fechaIni, fechaFin, idCiudad)))
        Catch
            EscribirRespuesta(res, "[]")
        End Try
    End Sub

    Private Sub ManejarHistorialClienteAdmin(req As HttpListenerRequest, res As HttpListenerResponse, reporteNeg As ReporteNegocio)
        Try
            Dim idCliente = Integer.Parse(req.QueryString("idCliente"))
            EscribirRespuesta(res, JsonSerializer.Serialize(reporteNeg.HistorialClienteAdmin(idCliente)))
        Catch
            EscribirRespuesta(res, "[]")
        End Try
    End Sub

    Private Sub ManejarCierreCajas(req As HttpListenerRequest, res As HttpListenerResponse, reporteNeg As ReporteNegocio)
        Try
            Dim fechaIni = req.QueryString("fechaIni")
            Dim fechaFin = req.QueryString("fechaFin")
            EscribirRespuesta(res, JsonSerializer.Serialize(reporteNeg.CierreCajas(fechaIni, fechaFin)))
        Catch
            EscribirRespuesta(res, "[]")
        End Try
    End Sub

    Private Sub ManejarLTV(req As HttpListenerRequest, res As HttpListenerResponse, reporteNeg As ReporteNegocio)
        Try
            EscribirRespuesta(res, JsonSerializer.Serialize(reporteNeg.ReporteLTV()))
        Catch
            EscribirRespuesta(res, "[]")
        End Try
    End Sub

    Private Sub ManejarActividad(req As HttpListenerRequest, res As HttpListenerResponse, reporteNeg As ReporteNegocio)
        Try
            Dim fechaIni = req.QueryString("fechaIni")
            Dim fechaFin = req.QueryString("fechaFin")
            EscribirRespuesta(res, JsonSerializer.Serialize(reporteNeg.ReporteActividad(fechaIni, fechaFin)))
        Catch
            EscribirRespuesta(res, "[]")
        End Try
    End Sub

    Private Sub ManejarRetencion(req As HttpListenerRequest, res As HttpListenerResponse, reporteNeg As ReporteNegocio)
        Try
            EscribirRespuesta(res, JsonSerializer.Serialize(reporteNeg.ReporteRetencion()))
        Catch
            EscribirRespuesta(res, "[]")
        End Try
    End Sub

    Private Sub ManejarCohorte(req As HttpListenerRequest, res As HttpListenerResponse, reporteNeg As ReporteNegocio)
        Try
            EscribirRespuesta(res, JsonSerializer.Serialize(reporteNeg.ReporteCohorte()))
        Catch
            EscribirRespuesta(res, "[]")
        End Try
    End Sub

    Private Sub ManejarRemarketing(req As HttpListenerRequest, res As HttpListenerResponse, reporteNeg As ReporteNegocio)
        Try
            EscribirRespuesta(res, JsonSerializer.Serialize(reporteNeg.ReporteRemarketing()))
        Catch
            EscribirRespuesta(res, "[]")
        End Try
    End Sub

    ' ==================== HELPERS ====================
    Private Function LeerBody(req As HttpListenerRequest) As String
        Using reader As New StreamReader(req.InputStream)
            Return reader.ReadToEnd()
        End Using
    End Function

    Private Sub EscribirRespuesta(res As HttpListenerResponse, json As String)
        Dim buffer = Encoding.UTF8.GetBytes(json)
        res.ContentType = "application/json"
        res.ContentLength64 = buffer.Length
        res.OutputStream.Write(buffer, 0, buffer.Length)
    End Sub
End Module