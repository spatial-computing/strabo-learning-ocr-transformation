using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace LearnWeights
{
    class Database
    {
        NpgsqlConnection conn;
        public Database()
        {
            string username = resource.ResourceManager.GetString("username");
            string password = resource.ResourceManager.GetString("password");
            string connstring = String.Format("Server={0};Port={1};" + "User Id={2};Password={3};Database={4};",
                "localhost", "5432", username, password, "dictionary");
            conn = new NpgsqlConnection(connstring);
        }

        public DataTable readInputData()
        {
            conn.Open();
            string sql = "SELECT * FROM public.\"scan6i1970\"";
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, conn);
            DataSet ds = new DataSet();
            ds.Reset();
            da.Fill(ds);
            DataTable dt = new DataTable();
            dt = ds.Tables[0];
            conn.Close();
            return dt;
        }

        public DataTable readLabeledData()
        {
            conn.Open();
            string sql = "SELECT * FROM public.\"labeledData\"";
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, conn);
            DataSet ds = new DataSet();
            ds.Reset();
            da.Fill(ds);
            DataTable dt = new DataTable();
            dt = ds.Tables[0];
            conn.Close();
            return dt;
        }


        public void writeLabeledData(string refString, string alignedString)
        {
            conn.Open();
            NpgsqlCommand command = new NpgsqlCommand("INSERT INTO public.\"labeledData\" VALUES (@refString, @alignedString )", conn);
            command.Parameters.AddWithValue("@refString", "\"" + refString + "\"");
            command.Parameters.AddWithValue("@alignedString", "\"" + alignedString + "\"");
            Int32 rowsaffected = command.ExecuteNonQuery();
            conn.Close();
        }

        public void writeSequenceData(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                string reference = row["GroundTruth"].ToString();
                string aligned = row["DictionaryResult"].ToString();

                char[] referenceArray = reference.ToCharArray();
                char[] alignedArray = aligned.ToCharArray();


                for (int i = 0, j = 0; i < referenceArray.Length && j < alignedArray.Length; i++, j++)
                {
                    if (referenceArray[i] == ' ' || alignedArray[j] == ' '
                        || referenceArray[i] == '/' || alignedArray[j] == '/'
                        || referenceArray[i] == '\\' || alignedArray[j] == '\\'
                        || referenceArray[i] == '"' || alignedArray[j] == '"'
                        || referenceArray[i] == '\n' || alignedArray[j] == '\n'
                        || Char.IsDigit(referenceArray[i]) || Char.IsDigit(alignedArray[i]))
                        continue;

                    string previousChar, nextChar;
                    if (i == 1)
                        previousChar = "SS";//start string
                    else
                        previousChar = alignedArray[j - 1].ToString().Replace("\"", "").ToLower();

                    string currentChar = alignedArray[j].ToString().Replace("\"", "").ToLower();
                    string label = referenceArray[i].ToString().Replace("\"", "").ToLower();

                    if (i == referenceArray.Length - 2)
                        nextChar = "ES";//end string
                    else
                        nextChar = alignedArray[j + 1].ToString().Replace("\"", "").ToLower();

                    insertSequenceData(previousChar, currentChar, nextChar, label);

                }
            }
            updateSequenceData();
        }

        private void insertSequenceData(string previousChar, string currentChar, string nextChar, string label)
        {
            conn.Open();
            NpgsqlCommand command = new NpgsqlCommand("INSERT INTO public.\"SequenceData\" VALUES (@previousChar, @currentChar, @nextChar, @label)", conn);
            command.Parameters.AddWithValue("@previousChar", "\"" + previousChar.Replace("\"", "") + "\"");
            command.Parameters.AddWithValue("@currentChar", "\"" + currentChar.Replace("\"", "") + "\"");
            command.Parameters.AddWithValue("@nextChar", "\"" + nextChar.Replace("\"", "") + "\"");
            command.Parameters.AddWithValue("@label", "\"" + label.Replace("\"", "") + "\"");
            Int32 rowsaffected = command.ExecuteNonQuery();
            conn.Close();
            

        }

        private void updateSequenceData()
        {
            string stmt = "update public.\"SequenceData\" set \"PreviousChar\" = replace(\"PreviousChar\", '\"', ''), \"CurrentChar\" = replace(\"CurrentChar\", '\"', ''), \"NextChar\" = replace(\"NextChar\", '\"', ''), \"Label\" = replace(\"Label\", '\"', '')";
            conn.Open();
            NpgsqlCommand command = new NpgsqlCommand(stmt, conn);
            Int32 rowsaffected = command.ExecuteNonQuery();
            conn.Close();
        }
    }
}
