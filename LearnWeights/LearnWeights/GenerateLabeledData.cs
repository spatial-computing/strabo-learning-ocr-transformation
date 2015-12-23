using LearnWeights;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace Strabo.Core.TextRecognition
{
    class GenerateLabeledData
    {
        //Loads labaled data from scan6i1970 to labeledData
        public static void loadData()
        {
            Database db = new Database();
            DataTable dt = db.readInputData();
            foreach (DataRow row in dt.Rows)
            {
                string[] alignments = NeedlemanWunsch.findSimScore(row["tesseractv"].ToString(), row["dictionary"].ToString());
                db.writeLabeledData(alignments[0], alignments[1]);

            }

        }
    }
}
