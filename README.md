# strabo-learning-ocr-transformation
This module contains all the steps needed to perform spelling correction on OCR text

## Steps for Spelling Correction
1. Run LearnWeights.sln
  * This .NET solution contains all the database operations to generate training data. This reads data from scan6i1970 table and generates both alignment data and training data. Feature vector includes previous charatcer, current character and next character of OCR. Label is the current character in ground truth.
  * Table Names: labaledData, SequenceData
  
2. Run python script for training
  * Command1: activate py34
  * Command2: python main_gbc.py 
  * Postgres odbc support is available for python 3.4 and not for python 3.5. Hence activate py34 virtual environment to read data from postgres tables.
  * Gradient Boosting Classifier is the training algorithm used. The script will generate model files and vectolrizing files for features and label which can be utilized for spelling correction.All data files(model and vectorizer) can be seen in the folder gbc_model.
  
3. Generate Jaccard candidtes and OCR from Strabo
  * Only file changed is CheckDictionaryElasticSearch.cs. Function name is writeAlignment.
  * Writes data into an Excel file. Change file names as appropriate.
  
4. Run python script for classification
  * Command: python get_score.py
  * Reads input CSV file from previous step and generates a new file containing alignments and the score.
  
  
  
  
