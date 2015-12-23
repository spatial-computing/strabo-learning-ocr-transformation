using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronPython.Hosting;

namespace Strabo.Core.TextRecognition
{
    class PythonInterface
    {
        public static void useModel()
        {
            string script = @"C:\Users\rashmina\Documents\LearnWeights\classify_gbc.py"; 
            var engine = Python.CreateEngine();
            dynamic py = engine.ExecuteFile(script);
            dynamic obj = py.Classify();
            int score = obj.gbc("\"SS\"", "\"b\"", "\"b\"", "\"b\"");
        }
    }
}
