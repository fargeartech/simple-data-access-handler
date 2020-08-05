Imports System.Configuration
Imports DBHandlerVB.DBHandlerVB.DbProvider
Imports DBHandlerVB.DBHandlerVB.Interfaces

Namespace DBHandlerVB.Implementation
    Public Class DBWrapperFactory
        Private connectionStringSettings As ConnectionStringSettings

        Public Sub New(ByVal connectionStringName As String)
            connectionStringSettings = ConfigurationManager.ConnectionStrings(connectionStringName)
        End Sub

        Public Function CreateDatabase() As IDBWrapper
            Dim database As IDBWrapper = Nothing
            Select Case connectionStringSettings.ProviderName.ToLower()
                Case "system.data.sqlclient"
                    database = New MsSqlDataProvider(connectionStringSettings.ConnectionString)
                Case "mysql.data.mysqlclient"
                    database = New MySQLDataProvider(connectionStringSettings.ConnectionString)
                Case "system.data.sqlite"
                    database = New SQLLiteDataProvider(connectionStringSettings.ConnectionString)
            End Select

            Return database
        End Function

        Public Function GetProviderName() As String
            Return connectionStringSettings.ProviderName
        End Function
    End Class
End Namespace
