using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using LearnWeights;

namespace Strabo.Core.TextRecognition
{
    class MainClass
    {
        static void Main(String[] args)
        {
            /*
            * One time operation to load labeled data(alignments) and 
            * sequence data(training data, in the form of previous, current, next character sequence along with label)
            */
            GenerateLabeledData.loadData();
            Database db = new Database();
            DataTable dt = db.readLabeledData();
            db.writeSequenceData(dt);
           
        }
    }
}
