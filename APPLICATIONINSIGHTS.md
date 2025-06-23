# Azure Application Insights - Configuración

## 📊 ¿Qué es Application Insights?

Azure Application Insights es un servicio de monitoreo de aplicaciones que nos permite:
- **Logging estructurado** en la nube
- **Telemetría en tiempo real** de la aplicación
- **Métricas de rendimiento** automáticas
- **Detección de errores** y alertas
- **Análisis de dependencias** (SQL, HTTP, etc.)

## 🚀 Configuración

### 1. Crear Recurso en Azure

1. Ve al [Portal de Azure](https://portal.azure.com)
2. Busca "Application Insights"
3. Crea nuevo recurso
4. Copia el **Connection String**

### 2. Configurar Variable de Entorno

Agrega a tu archivo `.env.local`:

```bash
APPLICATIONINSIGHTS_CONNECTION_STRING=InstrumentationKey=tu-key;IngestionEndpoint=https://tu-region.in.applicationinsights.azure.com/;LiveEndpoint=https://tu-region.livediagnostics.monitor.azure.com/
```

### 3. ¡Listo! 🎉

La aplicación automáticamente:
- ✅ Enviará logs a Azure
- ✅ Monitoreará rendimiento
- ✅ Rastreará dependencias SQL
- ✅ Detectará errores

## 📈 ¿Qué se Monitorea?

### Automático por Application Insights:
- **Requests HTTP**: Tiempo de respuesta, códigos de estado
- **Dependencies**: Llamadas a SQL Server con timing
- **Exceptions**: Errores no manejados
- **Performance Counters**: CPU, memoria, etc.

### Personalizado en nuestro código:
- **Transacciones SQL**: Inicio, commit, rollback
- **Consultas**: Tipo de consulta, registros afectados
- **Errores SQL**: Número de error, severidad
- **Conexiones**: Estado de conexión a BD

## 🔍 Cómo Ver los Logs

1. Ve a tu recurso de Application Insights en Azure
2. Navega a **Logs**
3. Usa queries como:

```kusto
// Ver todos los logs de la aplicación
traces
| where timestamp > ago(1h)
| order by timestamp desc

// Ver errores SQL
traces
| where message contains "Error SQL"
| order by timestamp desc

// Ver transacciones
traces
| where message contains "Transacción"
| order by timestamp desc
```

## 🎯 Beneficios Implementados

- **🔄 Tracking de Transacciones**: Ver cuándo se hacen commit/rollback
- **📊 Métricas SQL**: Registros afectados, tipos de consulta
- **❌ Análisis de Errores**: Errores SQL con números y severidad específicos
- **🔍 Debugging**: Logs estructurados para debugging fácil
- **📈 Performance**: Monitoring automático de rendimiento

## 💡 Ejemplos de Logs que Verás

```
✅ Transacción ExecuteAsync completada exitosamente. Filas afectadas: 1
🔍 Ejecutando consulta GetAllAsync: ConversationSession
❌ Error SQL en ExecuteAsync | ErrorNumber: 2 | Severity: 16
🔄 Transacción ExecuteAsync revertida (rollback)
```

**¡Sin Application Insights configurado, la app funciona normal pero no envía telemetría!**
