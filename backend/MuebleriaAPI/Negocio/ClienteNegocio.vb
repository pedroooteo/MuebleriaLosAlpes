Imports MuebleriaAPI.Datos
Imports System.Collections.Generic

Namespace Negocio
    Public Class ClienteNegocio
        Private ReadOnly _datos As New ClienteDatos()

        Public Function ObtenerPerfil(idCliente As Integer) As Object
            Return _datos.ObtenerPerfil(idCliente)
        End Function

        Public Sub RegistrarCliente(tipodoc As String, numDoc As String, nombre As String,
                                    telRes As String, telCel As String, direccion As String,
                                    idCiudad As Integer, email As String, profesion As String,
                                    tipoPersona As String, nit As String,
                                    username As String, passwordHash As String)
            _datos.RegistrarCliente(tipodoc, numDoc, nombre, telRes, telCel, direccion,
                                    idCiudad, email, profesion, tipoPersona, nit, username, passwordHash)
        End Sub

        Public Function BuscarClientes(criterio As String) As List(Of Dictionary(Of String, Object))
            Return _datos.BuscarClientes(criterio)
        End Function

        Public Sub EliminarCliente(idCliente As Integer)
            _datos.EliminarCliente(idCliente)
        End Sub

    End Class
End Namespace
