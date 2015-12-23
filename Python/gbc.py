__author__ = 'rmenon'
from sklearn.ensemble import GradientBoostingClassifier

import sys
from sklearn.multiclass import OneVsRestClassifier
from sklearn.metrics import accuracy_score
from sklearn.externals import joblib

import Data

def classify():
    file = open('result_gbc.txt', 'w')
    X_train, X_test, y_train, y_test = Data.get_dataset(0.3)
    clf = OneVsRestClassifier(GradientBoostingClassifier(n_estimators=100))
    clf.fit(X_train, y_train)
    joblib.dump(clf, 'gbc_model.pkl')
    y_prediction = clf.predict(X_test)
    acc = accuracy_score(y_test, y_prediction)
    print(acc)
    file.write(str(acc))
    file.close()
	



