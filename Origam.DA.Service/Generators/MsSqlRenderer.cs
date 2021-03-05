﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Origam.Schema;
using Origam.Schema.EntityModel;
using ResourceUtils = Origam.Schema.ResourceUtils;

public class MsSqlRenderer: SqlRenderer {
    
    public override string NameLeftBracket => "[";

    public override string NameRightBracket => "]";
    
    public override string ParameterDeclarationChar => "@";

    public override string ParameterReferenceChar => "@";

    public override string StringConcatenationChar => "+";
    
    
    public override string SelectClause(string finalQuery, int top)
    {
        return top == 0
            ? "SELECT" + finalQuery
            : "SELECT TOP " + top + finalQuery;
    }
    
    public override string ConvertGeoFromTextClause(string argument)
    {
        return "geography::STGeomFromText(" + argument + ", 4326)";
    }

    public override string ConvertGeoToTextClause(string argument)
    {
        return argument + ".STAsText()";
    }
    
    internal override string Sequence(string entityName, string primaryKeyName)
    {
        return "; SELECT @@IDENTITY AS " + primaryKeyName;
    }
    internal override string IsNull()
    {
        return "ISNULL";
    }
    internal override string CountAggregate()
    {
        return "COUNT_BIG";
    }
    
    internal override string DeclareAsSql()
    {
        return "AS";
    }
    internal override string FunctionPrefix()
    {
        return "dbo.";
    }
    internal override string VarcharSql()
    {
        return "NVARCHAR";
    }

    internal override string Length(string expresion)
    {
        return string.Format("LEN({0})", expresion);
    }
    internal override string Text(string expresion)
    {
        return string.Format("CAST ({0} AS {1} )", expresion, "NVARCHAR(MAX)");
    }
    internal override string DatePart(string datetype, string expresion)
    {
        return string.Format("DATEPART({0},{1})", datetype, expresion);
    }
    internal override string DateAdd(DateTypeSql datepart, string number, string date)
    {
        return string.Format("DATEADD({0},{1},{2})", GetAddDateSql(datepart),number,date);
    }
    
    private string GetAddDateSql(DateTypeSql datepart)
    {
        switch (datepart)
        {
            case DateTypeSql.Second:
                return "s";
            case DateTypeSql.Minute:
                return "mi";
            case DateTypeSql.Hour:
                return "hh";
            case DateTypeSql.Day:
                return "dd";
            case DateTypeSql.Month:
                return "m";
            case DateTypeSql.Year:
                return "yy";
            default:
                throw new NotSupportedException("Unsuported in AddDateSql " + datepart.ToString());
        }
    }

    internal override string DateDiff(DateTypeSql datepart, string startdate, string enddate)
    {
        return string.Format("DATEDIFF({0}, {1}, {2})", GetAddDateSql(datepart), startdate, enddate);
    }
    internal override string STDistance(string point1, string point2)
    {
        return string.Format("{0}.STDistance({1})", point1, point2);
    }
    internal override string Now()
    {
        return "GETDATE()";
    }
    internal override string FreeText(string columnsForSeach, string freetext_string, string languageForFullText)
    {
        if(string.IsNullOrEmpty(languageForFullText))
        {
            return string.Format("FREETEXT({0},{1})", columnsForSeach, freetext_string);
        }
        return string.Format("FREETEXT({0},{1},{2})", columnsForSeach,freetext_string,languageForFullText);
    }
    internal override string Contains(string columnsForSeach, string freetext_string, string languageForFullText)
    {
        if (string.IsNullOrEmpty(languageForFullText))
        {
            return string.Format("CONTAINS({0},{1})", columnsForSeach, freetext_string);
        }
        return string.Format("CONTAINS({0},{1},{2})", columnsForSeach, freetext_string, languageForFullText);
    }
    internal override string LatLon(geoLatLonSql latLon, string expresion)
    {
        switch (latLon)
        {
            case geoLatLonSql.Lat:
                return string.Format("{0}.Lat", expresion);
            case geoLatLonSql.Lon:
                return string.Format("{0}.Long", expresion);
            default:
                throw new NotSupportedException("Unsuported in Latitude or Longtitude " + latLon.ToString());
        }
    }
    internal override string Array(string expresion1, string expresion2)
    {
        return string.Format("{0} IN (SELECT ListValue FROM {1} origamListValue)", expresion1,expresion2);
    }
    internal override string CreateDataStructureHead()
    {
        return "";
    }
    internal override string DeclareBegin()
    {
        return "";
    }
    internal override string SetParameter(string name)
    {
        return string.Format("SET {0} = NULL{1}", name, Environment.NewLine);
    }
    
    public override string DefaultDdlDataType(OrigamDataType columnType)
    {
        switch (columnType)
        {
            case OrigamDataType.Geography:
                return "geography";
            case OrigamDataType.Memo:
                return "nvarchar(max)";
            case OrigamDataType.Object:
                return "nvarchar(max)";
            case OrigamDataType.Xml:
                return "nvarchar(max)";
            case OrigamDataType.Blob:
                return "varbinary(max)";
            default:
                return ConvertDataType(columnType, null).ToString();
        }
    }
        
    public SqlDbType ConvertDataType(OrigamDataType columnType,
        DatabaseDataType dbDataType)
    {
        if (dbDataType != null)
        {
            return (SqlDbType)Enum.Parse(typeof(SqlDbType),
                dbDataType.MappedDatabaseTypeName);
        }
        switch (columnType)
        {
            case OrigamDataType.Blob:
                return SqlDbType.Image;
            case OrigamDataType.Boolean:
                return SqlDbType.Bit;
            case OrigamDataType.Byte:
                //TODO: check right 
                return SqlDbType.TinyInt;
            case OrigamDataType.Currency:
                return SqlDbType.Money;
            case OrigamDataType.Date:
                return SqlDbType.DateTime;
            case OrigamDataType.Long:
                return SqlDbType.BigInt;
            case OrigamDataType.Xml:
            case OrigamDataType.Memo:
                return SqlDbType.NVarChar;
            case OrigamDataType.Array:
                return SqlDbType.Structured;
            case OrigamDataType.Geography:
                return SqlDbType.Text;
            case OrigamDataType.Integer:
                return SqlDbType.Int;
            case OrigamDataType.Float:
                return SqlDbType.Decimal;
            case OrigamDataType.Object:
            case OrigamDataType.String:
                return SqlDbType.NVarChar;
            case OrigamDataType.UniqueIdentifier:
                return SqlDbType.UniqueIdentifier;
            default:
                throw new NotSupportedException(ResourceUtils.GetString("UnsupportedType"));
        }
    }
    public override IDbDataParameter BuildParameter(string paramName,
        string sourceColumn, OrigamDataType dataType,
        DatabaseDataType dbDataType,
        int dataLength, bool allowNulls)
    {
        SqlParameter sqlParam = new SqlParameter(
            paramName,
            ConvertDataType(dataType, dbDataType),
            dataLength,
            sourceColumn
        );
        sqlParam.IsNullable = allowNulls;
        if (sqlParam.SqlDbType == SqlDbType.Decimal)
        {
            sqlParam.Precision = 28;
            sqlParam.Scale = 10;
        }

        // Workaround: in .net 2.0 if NText is not -1, 
        // sometimes the memo is truncated when storing to db
        if (sqlParam.SqlDbType == SqlDbType.NVarChar && dataLength == 0)
        {
            sqlParam.Size = -1;
        }

        if (sqlParam.SqlDbType == SqlDbType.NText
            || sqlParam.SqlDbType == SqlDbType.Text)
        {
            sqlParam.Size = -1;
        }

        if (sqlParam.SqlDbType == SqlDbType.Structured)
        {
            sqlParam.TypeName = "OrigamListValue";
        }

        return sqlParam;
    }
}