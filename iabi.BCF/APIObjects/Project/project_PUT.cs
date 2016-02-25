﻿using Newtonsoft.Json;

namespace iabi.BCF.APIObjects.Project
{
    /// <summary>
    /// HTTP PUT representation of the project
    /// </summary>
    [JsonObject(Title = "project")]
    public class project_PUT : project_Base
    {
        /// <summary>
        /// Name of the project to be changed
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "name")]
        public override string name { get; set; }
    }
}