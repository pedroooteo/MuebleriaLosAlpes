Imports System.Data
Imports Oracle.ManagedDataAccess.Client

Namespace Datos
    Public Class SeguridadDatos
        
        Public Function ObtenerDatosUsuario(username As String) As (idCliente As Integer, idRol As Integer, nombreRol As String)
            Dim idCliente As Integer = 0
            Dim idRol As Integer = 0
            Dim nombreRol As String = ""
            Using conn = Conexion.ObtenerConexion()
                conn.Open()
                Using cmd As New OracleCommand(
                    "SELECT U.ID_CLIENTE, U.ID_ROL, R.NOMBRE_ROL " &
                    "FROM USUARIOS_SISTEMA U " &
                    "JOIN ROLES R ON U.ID_ROL = R.ID_ROL " &
                    "WHERE U.USERNAME = :p_username AND U.ESTADO = 'ACTIVO'", conn)
                    cmd.Parameters.Add("p_username", OracleDbType.Varchar2).Value = username
                    Using reader = cmd.ExecuteReader()
                        If reader.Read() Then
                            idCliente = If(IsDBNull(reader("ID_CLIENTE")), 0, Convert.ToInt32(reader("ID_CLIENTE")))
                            idRol = Convert.ToInt32(reader("ID_ROL"))
                            nombreRol = reader("NOMBRE_ROL").ToString()
                        End If
                    End Using
                End Using
            End Using
            Return (idCliente, idRol, nombreRol)
        End Function

        Public Function ValidarLogin(username As String, passwordHash As String) As Integer
            Dim resultado As Integer = 0
            
            Using conn = Conexion.ObtenerConexion()
                conn.Open()
                
                Using cmd As New OracleCommand("PKG_SEGURIDAD.FN_VALIDAR_LOGIN", conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    ' ¡TRUCO DE ORACLE! El Return de una función debe ir primero
                    Dim paramRetorno As New OracleParameter("ReturnValue", OracleDbType.Int32)
                    paramRetorno.Direction = ParameterDirection.ReturnValue
                    cmd.Parameters.Add(paramRetorno)

                    ' Pasamos los parámetros de entrada
                    cmd.Parameters.Add("p_username", OracleDbType.Varchar2).Value = username
                    cmd.Parameters.Add("p_password_hash", OracleDbType.Varchar2).Value = passwordHash

                    ' Ejecutamos
                    cmd.ExecuteNonQuery()
                    
                    ' Leemos el 1 o el 0 que devolvió la función
                    resultado = Convert.ToInt32(paramRetorno.Value.ToString())
                End Using
            End Using
            
            Return resultado
        End Function

    End Class
End Namespace