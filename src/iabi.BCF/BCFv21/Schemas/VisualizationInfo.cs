namespace iabi.BCF.BCFv21.Schemas
{
    public partial class VisualizationInfo
    {
        public VisualizationInfo()
        {
            // This ensures that a viewpoint is always initialized with a Guid
            Guid = System.Guid.NewGuid().ToString();
        }
    }
}
