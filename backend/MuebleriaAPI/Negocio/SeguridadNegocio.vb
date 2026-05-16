Imports MuebleriaAPI.Datos

Namespace Negocio
    Public Class SeguridadNegocio
        Private ReadOnly _datos As New SeguridadDatos()

        Public Function AutenticarUsuario(username As String, password As String) As Boolean
            If String.IsNullOrWhiteSpace(username) OrElse String.IsNullOrWhiteSpace(password) Then Return False
            Return _datos.ValidarLogin(username, password) = 1
        End Function

        Public Function ObtenerDatosUsuario(username As String) As (idCliente As Integer, idRol As Integer, nombreRol As String)
            Return _datos.ObtenerDatosUsuario(username)
        End Function

    End Class
End Namespace