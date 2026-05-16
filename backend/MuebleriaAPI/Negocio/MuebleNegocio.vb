Imports MuebleriaAPI.Entidades
Imports MuebleriaAPI.Datos
Imports System.Collections.Generic
Imports System

Namespace Negocio
    Public Class MuebleNegocio
        Private ReadOnly _datos As New MuebleDatos()

        Public Function ObtenerMuebles() As List(Of Mueble)
            Return _datos.ConsultarMuebles()
        End Function

        Public Function ObtenerDetalleMueble(idMueble As Integer) As Object
            Return _datos.ObtenerDetalleMueble(idMueble)
        End Function

        Public Function RegistrarNuevoMueble(obj As Mueble) As String
            Try
                _datos.InsertarMueble(obj)
                Return "¡Mueble registrado exitosamente en Oracle!"
            Catch ex As Exception
                Return "Error en BD: " & ex.Message
            End Try
        End Function

        Public Function BuscarMueblesAdmin(criterio As String, tipo As String) As List(Of Dictionary(Of String, Object))
            Return _datos.BuscarMueblesAdmin(criterio, tipo)
        End Function

        Public Sub ModificarMueble(idMueble As Integer, referencia As String, nombre As String,
                                   descripcion As String, tipo As String, idCategoria As Integer,
                                   material As String, altoCm As Decimal, anchoCm As Decimal,
                                   profundidadCm As Decimal, color As String, pesoGramos As Decimal, fotoUrl As String)
            _datos.ModificarMueble(idMueble, referencia, nombre, descripcion, tipo, idCategoria,
                                   material, altoCm, anchoCm, profundidadCm, color, pesoGramos, fotoUrl)
        End Sub

        Public Sub ActualizarPrecioStock(idMueble As Integer, precio As Decimal, stock As Integer)
            _datos.ActualizarPrecioStock(idMueble, precio, stock)
        End Sub

        Public Sub EliminarMueble(idMueble As Integer)
            _datos.EliminarMueble(idMueble)
        End Sub

        Public Function ListarCategorias() As List(Of Dictionary(Of String, Object))
            Return _datos.ListarCategorias()
        End Function
    End Class
End Namespace