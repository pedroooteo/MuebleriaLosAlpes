Imports Oracle.ManagedDataAccess.Client
Imports System.Data
Imports System.Collections.Generic

Namespace Datos
    Public Class ClienteDatos

        Public Function ObtenerPerfil(idCliente As Integer) As Object
            Using conn = Conexion.ObtenerConexion()
                conn.Open()
                Using cmd As New OracleCommand("PKG_CLIENTES.PR_OBTENER_PERFIL", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.Add("p_id_cliente", OracleDbType.Int32).Value = idCliente
                    Dim pCursor As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    pCursor.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(pCursor)
                    Using reader = cmd.ExecuteReader()
                        If reader.Read() Then
                            Return New With {
                                .idCliente = Convert.ToInt32(reader("ID_CLIENTE")),
                                .nombre = reader("NOMBRE_COMPLETO").ToString(),
                                .email = reader("EMAIL").ToString(),
                                .telResidencia = reader("TEL_RESIDENCIA").ToString(),
                                .telCelular = If(IsDBNull(reader("TEL_CELULAR")), "", reader("TEL_CELULAR").ToString()),
                                .direccion = reader("DIRECCION").ToString(),
                                .tipoDoc = reader("TIPO_DOC").ToString(),
                                .numDoc = reader("NUM_DOC").ToString(),
                                .tipoPersona = reader("TIPO_PERSONA").ToString(),
                                .nit = If(IsDBNull(reader("NIT")), "", reader("NIT").ToString()),
                                .profesion = If(IsDBNull(reader("PROFESION")), "", reader("PROFESION").ToString()),
                                .ciudad = If(IsDBNull(reader("CIUDAD")), "", reader("CIUDAD").ToString()),
                                .departamento = If(IsDBNull(reader("DEPARTAMENTO")), "", reader("DEPARTAMENTO").ToString())
                            }
                        End If
                    End Using
                End Using
            End Using
            Return Nothing
        End Function

        Public Sub RegistrarCliente(tipodoc As String, numDoc As String, nombre As String,
                                    telRes As String, telCel As String, direccion As String,
                                    idCiudad As Integer, email As String, profesion As String,
                                    tipoPersona As String, nit As String,
                                    username As String, passwordHash As String)
            Using conn = Conexion.ObtenerConexion()
                conn.Open()
                Using cmd As New OracleCommand("PKG_SEGURIDAD.PR_REGISTRAR_CLIENTE_USUARIO", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.Add("p_tipo_doc", OracleDbType.Varchar2).Value = tipodoc
                    cmd.Parameters.Add("p_num_doc", OracleDbType.Varchar2).Value = numDoc
                    cmd.Parameters.Add("p_nombre", OracleDbType.Varchar2).Value = nombre
                    cmd.Parameters.Add("p_tel_residencia", OracleDbType.Varchar2).Value = telRes
                    cmd.Parameters.Add("p_tel_celular", OracleDbType.Varchar2).Value = If(String.IsNullOrEmpty(telCel), CObj(DBNull.Value), CObj(telCel))
                    cmd.Parameters.Add("p_direccion", OracleDbType.Varchar2).Value = direccion
                    cmd.Parameters.Add("p_id_ciudad", OracleDbType.Int32).Value = idCiudad
                    cmd.Parameters.Add("p_email", OracleDbType.Varchar2).Value = email
                    cmd.Parameters.Add("p_profesion", OracleDbType.Varchar2).Value = If(String.IsNullOrEmpty(profesion), CObj(DBNull.Value), CObj(profesion))
                    cmd.Parameters.Add("p_tipo_persona", OracleDbType.Varchar2).Value = tipoPersona
                    cmd.Parameters.Add("p_nit", OracleDbType.Varchar2).Value = If(String.IsNullOrEmpty(nit), CObj(DBNull.Value), CObj(nit))
                    cmd.Parameters.Add("p_username", OracleDbType.Varchar2).Value = username
                    cmd.Parameters.Add("p_password_hash", OracleDbType.Varchar2).Value = passwordHash
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        Public Function BuscarClientes(criterio As String) As List(Of Dictionary(Of String, Object))
            Dim lista As New List(Of Dictionary(Of String, Object))
            Using conn = Conexion.ObtenerConexion()
                conn.Open()
                Using cmd As New OracleCommand("PKG_CLIENTES.PR_BUSCAR_CLIENTES", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.Add("p_criterio", OracleDbType.Varchar2).Value = If(String.IsNullOrEmpty(criterio), DBNull.Value, CObj(criterio))
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

        Public Sub EliminarCliente(idCliente As Integer)
            Using conn = Conexion.ObtenerConexion()
                conn.Open()
                Using cmd As New OracleCommand("PKG_CLIENTES.PR_ELIMINAR_CLIENTE", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.Add("p_id_cliente", OracleDbType.Int32).Value = idCliente
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

    End Class
End Namespace
