﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Strabo.Core.TextRecognition
{
    class FindNWAlignment
    {
        static int refSeqCnt;
        static int alineSeqCnt;
        static int[,] scoringMatrix;
        static object[,] excelarray;
        static int excelrowNo;
        static int matchcharcount;
        static int initialWeights = -2;
        static int deleteWeights = -3;
        static int editWeights = -3;
        static int addWeights = -3; //-2
        static int matchWeights = 5;
        static int matchdefinedWeights = 1;
        public static void initSimMatrix(string refSeq, string alineSeq)
        {
            refSeqCnt = refSeq.Length + 1;
            alineSeqCnt = alineSeq.Length + 1;
            scoringMatrix = new int[alineSeqCnt, refSeqCnt];

            for (int i = 0; i < alineSeqCnt; i++)
            {
                scoringMatrix[i, 0] = initialWeights;
            }

            for (int j = 0; j < refSeqCnt; j++)
            {
                if (alineSeq.Length >= 2 && char.IsUpper(alineSeq[0]) && char.IsLower(alineSeq[1]))
                {
                    if (j <= refSeqCnt - alineSeqCnt)
                        scoringMatrix[0, j] = initialWeights - 2;
                    else
                        scoringMatrix[0, j] = initialWeights;
                }
                else
                {
                    if (j <= refSeqCnt - alineSeqCnt)
                        scoringMatrix[0, j] = 0;
                    else
                        scoringMatrix[0, j] = initialWeights;
                }

            }


            scoringMatrix[0, 0] = 0;
        }

        public static string[] findSimScore(string refSeq, string alineSeq)//dict, OCR
        {
            try
            {

                //string weightsPath = ConfigurationSettings.AppSettings["DictionaryWeightsPath"] != "" ? ConfigurationSettings.AppSettings["DictionaryWeightsPath"] : "";
                //string[] lines = System.IO.File.ReadAllLines(weightsPath);
                string[] lines = { };

                int temp = 0;
                initSimMatrix(refSeq, alineSeq);
                bool quit = true;

                refSeq = refSeq.ToLower();
                alineSeq = alineSeq.ToLower();

                for (int i = 1; i < alineSeqCnt; i++)
                {
                    int t = 0;
                    for (int j = 1; j < refSeqCnt; j++)
                    {

                        int scroeDiag = 0;
                        //if (alineSeq[i - 1] == ' ' || refSeq.Substring(j - 1, 1) == alineSeq.Substring(i - 1, 1))
                        if (refSeq.Substring(j - 1, 1) == alineSeq.Substring(i - 1, 1))
                            scroeDiag = scoringMatrix[i - 1, j - 1] + matchWeights;
                        else
                        {
                            int scoreval;
                            string prevchar1, prevchar2;

                            if (j > 1)
                                prevchar1 = refSeq.Substring(j - 2, 1);
                            else
                                prevchar1 = string.Empty;

                            if (i > 1)
                                prevchar2 = alineSeq.Substring(i - 2, 1);
                            else
                                prevchar2 = string.Empty;

                            scoreval = GetScore(lines, refSeq.Substring(j - 1, 1), prevchar1, alineSeq.Substring(i - 1, 1), prevchar2);

                            scroeDiag = scoringMatrix[i - 1, j - 1] + scoreval;

                        }

                        int tempAW = addWeights;

                        int scoreLeft = scoringMatrix[i, j - 1] + tempAW;// addWeights; //insert
                        int scoreUp = scoringMatrix[i - 1, j] + deleteWeights;// deleteWeights; //delete
                        //if(scoreUp)
                        int maxScore = Math.Max(Math.Max(scroeDiag, scoreLeft), scoreUp);

                        /*if(i==3&&j==3&&maxScore<=0&&scoringMatrix[3,1]<=0&&scoringMatrix[2,1]<=0)
                        {
                            return -1;
                        }*/
                        scoringMatrix[i, j] = maxScore;
                    }
                }
                return findAlignment(refSeq, alineSeq);
                //return scoringMatrix[alineSeqCnt - 1, refSeqCnt - 1];
            }
            catch (Exception e)
            {
                throw e;
            }


        }

        private static string[] findAlignment(string refSeq, string alineSeq) //not used
        {
            //Traceback Step
            char[] alineSeqArray = alineSeq.ToCharArray();
            char[] refSeqArray = refSeq.ToCharArray();

            string AlignmentA = string.Empty;
            string AlignmentB = string.Empty;
            int m = alineSeqCnt - 1;
            int n = refSeqCnt - 1;

            while (m > 0 || n > 0)
            {
                int scroeDiag = 0;

                if (m == 0 && n > 0)
                {
                    AlignmentA = refSeqArray[n - 1] + AlignmentA;
                    AlignmentB = "-" + AlignmentB;
                    n = n - 1;
                }
                else if (n == 0 && m > 0)
                {
                    AlignmentA = "-" + AlignmentA;
                    AlignmentB = alineSeqArray[m - 1] + AlignmentB;
                    m = m - 1;
                }
                else
                {
                    //Remembering that the scoring scheme is +3 for a match, -2 for a mismatch, and -1 for a gap and +2 for matrix match
                    if (alineSeqArray[m - 1] == refSeqArray[n - 1])
                        scroeDiag = matchWeights;
                    else
                        scroeDiag = editWeights;

                    if (m > 0 && n > 0 && scoringMatrix[m, n] == scoringMatrix[m - 1, n - 1] + scroeDiag)
                    {
                        AlignmentA = refSeqArray[n - 1] + AlignmentA;
                        AlignmentB = alineSeqArray[m - 1] + AlignmentB;
                        m = m - 1;
                        n = n - 1;
                    }
                    else if (n > 0 && scoringMatrix[m, n] == scoringMatrix[m, n - 1] + scroeDiag)
                    {
                        AlignmentA = refSeqArray[n - 1] + AlignmentA;
                        AlignmentB = "-" + AlignmentB;
                        n = n - 1;
                    }
                    else if (m > 0 && scoringMatrix[m, n] == scoringMatrix[m - 1, n] + scroeDiag)
                    {
                        AlignmentA = "-" + AlignmentA;
                        AlignmentB = alineSeqArray[m - 1] + AlignmentB;
                        m = m - 1;
                    }
                    /* else //2 char in alignseq map to 1 char in refseq
                     {
                         AlignmentA = refSeqArray[n - 1] + AlignmentA;
                         AlignmentB = alineSeqArray[m - 1] + alineSeqArray[m - 2] + AlignmentB;
                         n = n - 1;
                         m = m - 2;
                     }*/

                }
            }
            return new string[] { AlignmentA, AlignmentB };
        }

        private static int GetScore(string[] lines, string currentchar1, string prevchar1, string currentchar2, string prevchar2)
        {

            foreach (string line in lines)
            {
                string[] combinationChars = line.Split(' ');
                if (combinationChars[0] == currentchar1 && combinationChars[1] == currentchar2)
                {
                    matchcharcount = 1;
                    return matchWeights;
                }
                else
                    return editWeights;

                /*  if (combinationChars[0] == String.Concat(prevchar1, currentchar1) && combinationChars[1] == currentchar2)
                  {
                      matchcharcount = 2;
                      return matchdefinedWeights - addWeights;
                  }

                  if (combinationChars[1] == String.Concat(prevchar1, currentchar1) && combinationChars[0] == currentchar2)
                  {
                      matchcharcount = 2;
                      return matchdefinedWeights - addWeights;
                  }

                  if (combinationChars[0] == currentchar1 && combinationChars[1] == String.Concat(prevchar2, currentchar2))
                  {
                      matchcharcount = 2;
                      return matchdefinedWeights - deleteWeights;
                  }*/


            }
            return editWeights;
        }

    }
}
