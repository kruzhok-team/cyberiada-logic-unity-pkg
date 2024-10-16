using System;
using System.Collections.Generic;

namespace Talent.Graphs
{
    public class GraphData : IClonable<GraphData>
    {
        public string Name { get; set; }
        public string ReferenceGraphID { get; set; }

        private Dictionary<string, string> DocumentMetaData { get; set; } = new Dictionary<string, string>();

        public void AddMetaData(string key, string value)
        {
            if (key is null)
            {
                throw new NullReferenceException(nameof(key));
            }

            if (value is null)
            {
                throw new NullReferenceException(nameof(value));
            }
            
            DocumentMetaData[key] = value;
        }

        public bool TryGetMetadata(string key, out string value)
        {
            if (key is null)
            {
                throw new NullReferenceException(nameof(key));
            }
            
            return DocumentMetaData.TryGetValue(key, out value);
        }

        public void AddMetadata(string key, string value)
        {
            if (key is null)
            {
                throw new NullReferenceException(nameof(key));
            }

            if (value is null)
            {
                throw new NullReferenceException(nameof(value));
            }
            
            DocumentMetaData.Add(key, value);
        }

        /// <summary>
        /// Creates a copy of the graph data
        /// </summary>
        public GraphData GetCopy()
        {
            GraphData resultData = new GraphData { Name = Name };

            return resultData;
        }
    }
}
