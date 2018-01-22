using System;
using System.Collections.Generic;

namespace ObjectDB
{
    internal interface IManager
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
        /// <para>Saving instance to database</para>
        /// </summary>
        /// <param name="instance">Object instance written to database</param>
        /// <param name="instanceName">Name of instance in database</param>
        /// <param name="overwrite">Overwrite existing in database instance</param>
        /// <returns>Succesfuly</returns>
        bool SaveToDB(DBObject instance, string instanceName, bool overwrite = true);

        /// <summary>
        /// <para>Fetching instace from database</para>
        /// </summary>
        /// <param name="objectName">Object name in database</param>
        /// <param name="instanceName">Name of instance in database</param>
        /// <returns>Instance with type of type DBObject</returns>
        DBObject FetchFromDB(string objectName, string instanceName);

        /// <summary>
        /// <para>Fetching instace from database</para>
        /// </summary>
        /// <param name="objectName">Object name in database</param>
        /// <param name="instanceId">Id of instance in database</param>
        /// <returns>Instance with type of type DBObject</returns>
        DBObject FetchFromDB(string objectName, Guid instanceId);

        /// <summary>
        /// <para>Typed fetching instace from database</para>
        /// </summary>
        /// <param name="target">Typed instance to wich fetched instace will be custed and saved</param>
        /// <param name="instanceName">Name of instance in database</param>
        /// <returns>Success</returns>
        bool TryFetchFromDB<T>(ref T target, string instanceName) where T : DBObject;

        /// <summary>
        /// <para>Typed fetching typed instace from database</para>
        /// </summary>
        /// <param name="target">Typed instance to wich fetched instace will be custed and saved</param>
        /// <param name="instanceId">Id of instance in database</param>
        /// <returns>Success</returns>
        bool TryFetchFromDB<T>(ref T target, Guid instanceId) where T : DBObject;
    }
}
