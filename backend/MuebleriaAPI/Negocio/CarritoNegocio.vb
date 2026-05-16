Imports MuebleriaAPI.Datos
Imports MuebleriaAPI.Entidades
Imports System.Collections.Generic

Namespace Negocio
    Public Class CarritoNegocio
        Private ReadOnly _datos As New CarritoDatos()

        Public Sub AgregarAlCarrito(idCliente As Integer, idMueble As Integer, cantidad As Integer)
            _datos.AgregarAlCarrito(idCliente, idMueble, cantidad)
        End Sub

        Public Function VerCarrito(idCliente As Integer) As List(Of Object)
            Return _datos.VerCarrito(idCliente)
        End Function

        Public Sub EliminarDelCarrito(idDetalle As Integer)
            _datos.EliminarDelCarrito(idDetalle)
        End Sub

        Public Function EfectuarCompra(idCliente As Integer, formaPago As String) As String
            Return _datos.EfectuarCompra(idCliente, formaPago)
        End Function

        Public Function ObtenerCiudades() As List(Of Object)
            Return _datos.ObtenerCiudades()
        End Function

        Public Function HistorialCompras(idCliente As Integer) As List(Of Object)
            Return _datos.HistorialCompras(idCliente)
        End Function
    End Class
End Namespace