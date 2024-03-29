using System.Collections.Generic;

namespace Dangl.BCF.APIObjects.V21
{
    public class Topic_authorization
    {
        public List<Topic_actions> Topic_actions { get; set; }
        public List<string> Topic_status { get; set; }
    }
}