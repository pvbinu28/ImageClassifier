using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LiveImageClassifier
{
    [DataContract]
    public class PredictionModel
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Project { get; set; }

        [DataMember]
        public string Iteration { get; set; }

        [DataMember]
        public DateTime Created { get; set; }

        [DataMember]
        public List<Prediction> Predictions { get; set; }
    }

    [DataContract]
    public class Prediction
    {
        [DataMember]
        public double Probability { get; set; }

        [DataMember]
        public string TagId { get; set; }

        [DataMember]
        public string TagName { get; set; }

    }
}
