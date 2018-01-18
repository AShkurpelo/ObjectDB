using System;
using System.Collections.Generic;

namespace ObjectDB
{
    internal interface IObjectDBManager
    {
        /// <summary>
        /// <para>All database objects info</para>
        /// <para>Dictionary with object name as Key and object instances count as Value</para>
        /// </summary>
        Dictionary<string, long> GetObjectInfos();
        
        /// <summary>
        /// <para>All object instances info</para>
        /// <para>Dictionary with instance name as Key and instance id in database as Value</para>
        /// </summary>
        /// <param name="objectName">object's type name</param>
        /// <returns></returns>
        Dictionary<string, Guid> GetInstanceInfos(string objectName);

        /// <summary>
        /// <para>Checks if object already exists in database</para>
        /// </summary>
        /// <param name="objectName">object's type name</param>
        /// <returns></returns>
        bool Exists(string objectName);

        /// <summary>
        /// <para></para>
        /// </summary>
        /// <param name="obj">Object instance written to database</param>
        /// <param name="instanceName">Name of instance in database</param>
        /// <returns></returns>
        bool SaveToDB(DBObject obj, string instanceName);

        DBObject FetchFromDB(string objectName, string instanceName);
        DBObject FetchFromDB(string objectName, Guid instanceId);

        bool TryFetchFromDB<T>(out T saveTo, string instanceName);
        bool TryFetchFromDB<T>(out T saveTo, Guid instanceId);
    }
}
