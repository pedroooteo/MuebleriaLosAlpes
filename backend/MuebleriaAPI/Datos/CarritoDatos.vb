Imports Oracle.ManagedDataAccess.Client
Imports System.Data                   
Imports System.Collections.Generic

Namespace Datos
    Public Class CarritoDatos

        Public Function ObtenerCiudades() As List(Of Object)
            Dim lista As New List(Of Object)()
            Using conn = Conexion.ObtenerConexion()
                conn.Open()
                Using cmd As New OracleCommand("PKG_CLIENTES.PR_LISTAR_CIUDADES", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    Dim pCursor As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    pCursor.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(pCursor)
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            lista.Add(New With {
                                .idCiudad = Convert.ToInt32(reader("ID_CIUDAD")),
                                .ciudad = reader("CIUDAD").ToString(),
                                .departamento = reader("DEPARTAMENTO").ToString(),
                                .pais = reader("PAIS").ToString()
                            })
                        End While
                    End Using
                End Using
            End Using
            Return lista
        End Function

        Public Sub AgregarAlCarrito(idCliente As Integer, idMueble As Integer, cantidad As Integer)
            Using conn = Conexion.ObtenerConexion()
                conn.Open()
                Using cmd As New OracleCommand("PKG_CARRITO.PR_AGREGAR_AL_CARRITO", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.Add("p_id_cliente", OracleDbType.Int32).Value = idCliente
                    cmd.Parameters.Add("p_id_mueble", OracleDbType.Int32).Value = idMueble
                    cmd.Parameters.Add("p_cantidad", OracleDbType.Int32).Value = cantidad
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        Public Function VerCarrito(idCliente As Integer) As List(Of Object)
            Dim lista As New List(Of Object)()
            Using conn = Conexion.ObtenerConexion()
                conn.Open()
                Using cmd As New OracleCommand("PKG_CARRITO.PR_VER_CARRITO", conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    cmd.Parameters.Add("p_id_cliente", OracleDbType.Int32).Value = idCliente

                    Dim pCursor As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    pCursor.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(pCursor)

                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            lista.Add(New With {
                                .idDetalle = Convert.ToInt32(reader("ID_DETALLE_CARRITO")),
                                .idMueble = Convert.ToInt32(reader("ID_MUEBLE")),
                                .referencia = reader("REFERENCIA").ToString(),
                                .nombre = reader("NOMBRE").ToString(),
                                .cantidad = Convert.ToInt32(reader("CANTIDAD")),
                                .precio = Convert.ToDecimal(reader("PRECIO_UNITARIO")),
                                .subtotal = Convert.ToDecimal(reader("SUBTOTAL"))
                            })
                        End While
                    End Using
                End Using
            End Using
            Return lista
        End Function

        Public Sub EliminarDelCarrito(idDetalle As Integer)
            Using conn = Conexion.ObtenerConexion()
                conn.Open()
                Using cmd As New OracleCommand("PKG_CARRITO.PR_ELIMINAR_DEL_CARRITO", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.Add("p_id_detalle", OracleDbType.Int32).Value = idDetalle
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        Public Function EfectuarCompra(idCliente As Integer, formaPago As String) As String
            Using conn = Conexion.ObtenerConexion()
                conn.Open()
                Using cmd As New OracleCommand("PKG_COMPRAS.PR_EFECTUAR_COMPRA", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.Add("p_id_cliente", OracleDbType.Int32).Value = idCliente
                    cmd.Parameters.Add("p_forma_pago", OracleDbType.Varchar2).Value = formaPago
                    Dim pOrden As New OracleParameter("p_numero_orden", OracleDbType.Varchar2, 50)
                    pOrden.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(pOrden)
                    cmd.ExecuteNonQuery()
                    Return pOrden.Value.ToString()
                End Using
            End Using
        End Function

        Public Function HistorialCompras(idCliente As Integer) As List(Of Object)
            Dim lista As New List(Of Object)()
            Using conn = Conexion.ObtenerConexion()
                conn.Open()
                Using cmd As New OracleCommand("PKG_COMPRAS.PR_HISTORIAL_COMPRAS", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.Add("p_id_cliente", OracleDbType.Int32).Value = idCliente
                    Dim pCursor As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    pCursor.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(pCursor)
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            lista.Add(New With {
                                .numeroOrden = reader("NUMERO_ORDEN").ToString(),
                                .fecha = reader("FECHA_COMPRA").ToString(),
                                .total = Convert.ToDecimal(reader("TOTAL")),
                                .formaPago = reader("FORMA_PAGO").ToString(),
                                .estado = reader("ESTADO").ToString()
                            })
                        End While
                    End Using
                End Using
            End Using
            Return lista
        End Function

    End Class
End Namespace