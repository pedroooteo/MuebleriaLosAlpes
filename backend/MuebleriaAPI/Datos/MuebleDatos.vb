Imports Oracle.ManagedDataAccess.Client
Imports System.Data
Imports System.Collections.Generic
Imports MuebleriaAPI.Entidades   

Namespace Datos
    Public Class MuebleDatos

        ' CONSULTAR MUEBLES (usando el paquete)
        Public Function ConsultarMuebles() As List(Of Mueble)
            Dim lista As New List(Of Mueble)()

            Using conn = Conexion.ObtenerConexion()
                conn.Open()
                Using cmd As New OracleCommand("PKG_MUEBLES.PR_CONSULTAR_MUEBLES", conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    cmd.Parameters.Add("p_criterio", OracleDbType.Varchar2).Value = DBNull.Value
                    cmd.Parameters.Add("p_tipo", OracleDbType.Varchar2).Value = DBNull.Value

                    Dim pCursor As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    pCursor.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(pCursor)

                    Using reader As OracleDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim m As New Mueble With {
                                .ID_Mueble = Convert.ToInt32(reader("ID_MUEBLE")),
                                .Referencia = reader("REFERENCIA").ToString(),
                                .Nombre = reader("NOMBRE").ToString(),
                                .Tipo = reader("TIPO").ToString(),
                                .Precio = Convert.ToDecimal(reader("PRECIO_VENTA")),
                                .Stock = Convert.ToInt32(reader("STOCK_DISPONIBLE")),
                                .Material = If(IsDBNull(reader("MATERIAL")), "", reader("MATERIAL").ToString()),
                                .Color = If(IsDBNull(reader("COLOR")), "", reader("COLOR").ToString()),
                                .FotoUrl = If(IsDBNull(reader("FOTO_URL")), "", reader("FOTO_URL").ToString()),
                                .Descripcion = If(IsDBNull(reader("DESCRIPCION")), "", reader("DESCRIPCION").ToString())
                            }
                            lista.Add(m)
                        End While
                    End Using
                End Using
            End Using
            Return lista
        End Function

        ' OBTENER DETALLE DE UN MUEBLE
        Public Function ObtenerDetalleMueble(idMueble As Integer) As Object
            Using conn = Conexion.ObtenerConexion()
                conn.Open()
                Using cmd As New OracleCommand("PKG_MUEBLES.PR_OBTENER_MUEBLE", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.Add("p_id_mueble", OracleDbType.Int32).Value = idMueble
                    Dim pCursor As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    pCursor.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(pCursor)
                    Using reader As OracleDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            Return New With {
                                .ID_Mueble = Convert.ToInt32(reader("ID_MUEBLE")),
                                .Referencia = reader("REFERENCIA").ToString(),
                                .Nombre = reader("NOMBRE").ToString(),
                                .Descripcion = If(IsDBNull(reader("DESCRIPCION")), "", reader("DESCRIPCION").ToString()),
                                .Tipo = reader("TIPO").ToString(),
                                .Categoria = If(IsDBNull(reader("CATEGORIA")), "", reader("CATEGORIA").ToString()),
                                .Material = If(IsDBNull(reader("MATERIAL")), "", reader("MATERIAL").ToString()),
                                .AltoCm = Convert.ToDecimal(reader("ALTO_CM")),
                                .AnchoCm = Convert.ToDecimal(reader("ANCHO_CM")),
                                .ProfundidadCm = Convert.ToDecimal(reader("PROFUNDIDAD_CM")),
                                .Color = If(IsDBNull(reader("COLOR")), "", reader("COLOR").ToString()),
                                .PesoGramos = Convert.ToDecimal(reader("PESO_GRAMOS")),
                                .FotoUrl = If(IsDBNull(reader("FOTO_URL")), "", reader("FOTO_URL").ToString()),
                                .Precio = Convert.ToDecimal(reader("PRECIO_VENTA")),
                                .Stock = Convert.ToInt32(reader("STOCK_DISPONIBLE"))
                            }
                        End If
                    End Using
                End Using
            End Using
            Return Nothing
        End Function

        ' BUSCAR MUEBLES ADMIN (incluye stock 0)
        Public Function BuscarMueblesAdmin(criterio As String, tipo As String) As List(Of Dictionary(Of String, Object))
            Dim lista As New List(Of Dictionary(Of String, Object))
            Using conn = Conexion.ObtenerConexion()
                conn.Open()
                Using cmd As New OracleCommand("PKG_MUEBLES.PR_BUSCAR_MUEBLES_ADMIN", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.Add("p_criterio", OracleDbType.Varchar2).Value = If(String.IsNullOrEmpty(criterio), DBNull.Value, CObj(criterio))
                    cmd.Parameters.Add("p_tipo", OracleDbType.Varchar2).Value = If(String.IsNullOrEmpty(tipo), DBNull.Value, CObj(tipo))
                    Dim pCursor As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    pCursor.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(pCursor)
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim fila As New Dictionary(Of String, Object)
                            For i = 0 To reader.FieldCount - 1
                                fila(reader.GetName(i)) = If(IsDBNull(reader(i)), Nothing, reader(i))
                            Next
                            lista.Add(fila)
                        End While
                    End Using
                End Using
            End Using
            Return lista
        End Function

        ' MODIFICAR MUEBLE
        Public Sub ModificarMueble(idMueble As Integer, referencia As String, nombre As String,
                                   descripcion As String, tipo As String, idCategoria As Integer,
                                   material As String, altoCm As Decimal, anchoCm As Decimal,
                                   profundidadCm As Decimal, color As String, pesoGramos As Decimal,
                                   fotoUrl As String)
            Using conn = Conexion.ObtenerConexion()
                conn.Open()
                Using cmd As New OracleCommand("PKG_MUEBLES.PR_MODIFICAR_MUEBLE", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.Add("p_id_mueble", OracleDbType.Int32).Value = idMueble
                    cmd.Parameters.Add("p_referencia", OracleDbType.Varchar2).Value = referencia
                    cmd.Parameters.Add("p_nombre", OracleDbType.Varchar2).Value = nombre
                    cmd.Parameters.Add("p_descripcion", OracleDbType.Clob).Value = If(String.IsNullOrEmpty(descripcion), DBNull.Value, CObj(descripcion))
                    cmd.Parameters.Add("p_tipo", OracleDbType.Varchar2).Value = tipo
                    cmd.Parameters.Add("p_id_categoria", OracleDbType.Int32).Value = idCategoria
                    cmd.Parameters.Add("p_material", OracleDbType.Varchar2).Value = If(String.IsNullOrEmpty(material), DBNull.Value, CObj(material))
                    cmd.Parameters.Add("p_alto_cm", OracleDbType.Decimal).Value = altoCm
                    cmd.Parameters.Add("p_ancho_cm", OracleDbType.Decimal).Value = anchoCm
                    cmd.Parameters.Add("p_profundidad_cm", OracleDbType.Decimal).Value = profundidadCm
                    cmd.Parameters.Add("p_color", OracleDbType.Varchar2).Value = If(String.IsNullOrEmpty(color), DBNull.Value, CObj(color))
                    cmd.Parameters.Add("p_peso_gramos", OracleDbType.Decimal).Value = pesoGramos
                    cmd.Parameters.Add("p_foto_url", OracleDbType.Varchar2).Value = If(String.IsNullOrEmpty(fotoUrl), DBNull.Value, CObj(fotoUrl))
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        ' ACTUALIZAR PRECIO Y STOCK
        Public Sub ActualizarPrecioStock(idMueble As Integer, precio As Decimal, stock As Integer)
            Using conn = Conexion.ObtenerConexion()
                conn.Open()
                Using cmd As New OracleCommand("PKG_MUEBLES.PR_ACTUALIZAR_PRECIO_STOCK", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.Add("p_id_mueble", OracleDbType.Int32).Value = idMueble
                    cmd.Parameters.Add("p_precio", OracleDbType.Decimal).Value = precio
                    cmd.Parameters.Add("p_stock", OracleDbType.Int32).Value = stock
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        ' ELIMINAR MUEBLE
        Public Sub EliminarMueble(idMueble As Integer)
            Using conn = Conexion.ObtenerConexion()
                conn.Open()
                Using cmd As New OracleCommand("PKG_MUEBLES.PR_ELIMINAR_MUEBLE", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.Add("p_id_mueble", OracleDbType.Int32).Value = idMueble
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        ' LISTAR CATEGORIAS
        Public Function ListarCategorias() As List(Of Dictionary(Of String, Object))
            Dim lista As New List(Of Dictionary(Of String, Object))
            Using conn = Conexion.ObtenerConexion()
                conn.Open()
                Using cmd As New OracleCommand("PKG_MUEBLES.PR_LISTAR_CATEGORIAS", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    Dim pCursor As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    pCursor.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(pCursor)
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            lista.Add(New Dictionary(Of String, Object) From {
                                {"idCategoria", Convert.ToInt32(reader("ID_CATEGORIA"))},
                                {"nombre", reader("NOMBRE").ToString()}
                            })
                        End While
                    End Using
                End Using
            End Using
            Return lista
        End Function

        ' INSERTAR MUEBLE (usando paquete)
        Public Sub InsertarMueble(obj As Mueble)
            Using conn = Conexion.ObtenerConexion()
                conn.Open()
                Using cmd As New OracleCommand("PKG_MUEBLES.PR_CREAR_MUEBLE", conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    cmd.Parameters.Add("p_referencia", OracleDbType.Varchar2).Value = obj.Referencia
                    cmd.Parameters.Add("p_nombre", OracleDbType.Varchar2).Value = obj.Nombre
                    cmd.Parameters.Add("p_descripcion", OracleDbType.Clob).Value = If(obj.Descripcion, "")
                    cmd.Parameters.Add("p_tipo", OracleDbType.Varchar2).Value = obj.Tipo
                    cmd.Parameters.Add("p_id_categoria", OracleDbType.Int32).Value = 1
                    cmd.Parameters.Add("p_material", OracleDbType.Varchar2).Value = obj.Material
                    cmd.Parameters.Add("p_alto_cm", OracleDbType.Decimal).Value = obj.AltoCm
                    cmd.Parameters.Add("p_ancho_cm", OracleDbType.Decimal).Value = obj.AnchoCm
                    cmd.Parameters.Add("p_profundidad_cm", OracleDbType.Decimal).Value = obj.ProfundidadCm
                    cmd.Parameters.Add("p_color", OracleDbType.Varchar2).Value = obj.Color
                    cmd.Parameters.Add("p_peso_gramos", OracleDbType.Decimal).Value = obj.PesoGramos
                    cmd.Parameters.Add("p_foto_url", OracleDbType.Varchar2).Value = DBNull.Value
                    cmd.Parameters.Add("p_precio", OracleDbType.Decimal).Value = obj.Precio
                    cmd.Parameters.Add("p_stock", OracleDbType.Int32).Value = obj.Stock

                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

    End Class
End Namespace