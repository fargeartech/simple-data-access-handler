Imports System.Data.SqlClient
Imports System.Data.SQLite
Imports MySql.Data.MySqlClient

Namespace DBHandler.DbProvider
    Public Class ParameterManager
        Public Shared Function CreateParameter(ByVal providerName As String, ByVal name As String, ByVal value As Object, ByVal dbType As DbType, ByVal Optional direction As ParameterDirection = ParameterDirection.Input) As IDbDataParameter
            Dim parameter As IDbDataParameter = Nothing

            Select Case providerName.ToLower()
                Case "system.data.sqlclient"
                    Return CreateSqlParameter(name, value, dbType, direction)
                Case "mysql.data.mysqlclient"
                    Return CreateMySqlParameter(name, value, dbType, direction)
                Case "system.data.sqlite"
                    Return CreateSQLiteParameter(name, value, dbType, direction)
                Case "system.data.oleDb"
                Case "system.data.odbc"
            End Select

            Return parameter
        End Function

        Public Shared Function CreateParameter(ByVal providerName As String, ByVal name As String, ByVal size As Integer, ByVal value As Object, ByVal dbType As DbType, ByVal Optional direction As ParameterDirection = ParameterDirection.Input) As IDbDataParameter
            Dim parameter As IDbDataParameter = Nothing

            Select Case providerName.ToLower()
                Case "system.data.sqlclient"
                    Return CreateSqlParameter(name, size, value, dbType, direction)
                Case "mysql.data.mysqlclient"
                    Return CreateMySqlParameter(name, size, value, dbType, direction)
                Case "system.data.sqlite"
                    Return CreateSQLiteParameter(name, size, value, dbType, direction)
                Case "system.data.oleDb"
                Case "system.data.odbc"
            End Select

            Return parameter
        End Function

        Private Shared Function CreateSqlParameter(ByVal name As String, ByVal value As Object, ByVal dbType As DbType, ByVal direction As ParameterDirection) As IDbDataParameter
            Return New SqlParameter With {
                .DbType = dbType,
                .ParameterName = name,
                .Direction = direction,
                .Value = value
            }
        End Function

        Private Shared Function CreateSqlParameter(ByVal name As String, ByVal size As Integer, ByVal value As Object, ByVal dbType As DbType, ByVal direction As ParameterDirection) As IDbDataParameter
            Return New SqlParameter With {
                .DbType = dbType,
                .Size = size,
                .ParameterName = name,
                .Direction = direction,
                .Value = value
            }
        End Function

        Private Shared Function CreateMySqlParameter(ByVal name As String, ByVal value As Object, ByVal dbType As DbType, ByVal direction As ParameterDirection) As IDbDataParameter
            Return New MySqlParameter With {
                .DbType = dbType,
                .ParameterName = name,
                .Direction = direction,
                .Value = value
            }
        End Function

        Private Shared Function CreateMySqlParameter(ByVal name As String, ByVal size As Integer, ByVal value As Object, ByVal dbType As DbType, ByVal direction As ParameterDirection) As IDbDataParameter
            Return New MySqlParameter With {
                .DbType = dbType,
                .Size = size,
                .ParameterName = name,
                .Direction = direction,
                .Value = value
            }
        End Function

        Private Shared Function CreateSQLiteParameter(ByVal name As String, ByVal value As Object, ByVal dbType As DbType, ByVal direction As ParameterDirection) As IDbDataParameter
            Return New SQLiteParameter With {
                .DbType = dbType,
                .ParameterName = name,
                .Direction = direction,
                .Value = value
            }
        End Function
        Private Shared Function CreateSQLiteParameter(ByVal name As String, ByVal size As Integer, ByVal value As Object, ByVal dbType As DbType, ByVal direction As ParameterDirection) As IDbDataParameter
            Return New SQLiteParameter With {
                .DbType = dbType,
                .Size = size,
                .ParameterName = name,
                .Direction = direction,
                .Value = value
            }
        End Function
    End Class
End Namespace
